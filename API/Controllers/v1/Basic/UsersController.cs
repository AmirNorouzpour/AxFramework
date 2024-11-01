﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Hubs;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Exception;
using Common.Utilities;
using Data.Repositories;
using Data.Repositories.UserRepositories;
using Entities.Framework;
using Entities.Framework.AxCharts;
using Entities.Framework.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Services;
using Services.Services;
using Services.Services.Services;
using UAParser;
using WebFramework.Api;
using WebFramework.Filters;
using WebFramework.UserData;

namespace API.Controllers.v1.Basic
{
    /// <summary>
    /// Users Actions
    /// </summary>
    [ApiVersion("1")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _memoryCache;
        private readonly IBaseRepository<LoginLog> _loginlogRepository;
        private readonly IBaseRepository<Permission> _permissionRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Menu> _menuRepository;
        private readonly IBaseRepository<ConfigData> _configDataRepository;
        private readonly IBaseRepository<UserGroup> _userGroupRepository;
        private readonly IBaseRepository<FileAttachment> _fileRepository;
        private readonly IBaseRepository<UserMessage> _userMessageRepository;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IBaseRepository<UserConnection> _userConnectionRepository;
        private readonly IBaseRepository<AxChart> _chartRepository;
        private readonly IBaseRepository<BarChart> _barChartRepository;
        private readonly IBaseRepository<NumericWidget> _numberWidgetRepository;
        private readonly IHubContext<AxHub> _hub;
        private readonly IBaseRepository<AxGroup> _groupRepository;

        /// <inheritdoc />
        public UsersController(IUserRepository userRepository, IJwtService jwtService, IMemoryCache memoryCache, IBaseRepository<LoginLog> loginlogRepository, IBaseRepository<Permission> permissionRepository,
            IBaseRepository<UserToken> userTokenRepository, IBaseRepository<Menu> menuRepository, IBaseRepository<ConfigData> configDataRepository,
            IBaseRepository<UserGroup> userGroupRepository, IBaseRepository<FileAttachment> fileRepository, IBaseRepository<UserMessage> userMessageRepository, IUserConnectionService userConnectionService, IBaseRepository<UserConnection> userConnectionRepository, IBaseRepository<AxChart> chartRepository, IBaseRepository<BarChart> barChartRepository, IBaseRepository<NumericWidget> numberWidgetRepository, IHubContext<AxHub> hub, IBaseRepository<AxGroup> groupRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _memoryCache = memoryCache;
            _loginlogRepository = loginlogRepository;
            _permissionRepository = permissionRepository;
            _userTokenRepository = userTokenRepository;
            _menuRepository = menuRepository;
            _configDataRepository = configDataRepository;
            _userGroupRepository = userGroupRepository;
            _fileRepository = fileRepository;
            _userMessageRepository = userMessageRepository;
            _userConnectionService = userConnectionService;
            _userConnectionRepository = userConnectionRepository;
            _chartRepository = chartRepository;
            _barChartRepository = barChartRepository;
            _numberWidgetRepository = numberWidgetRepository;
            _hub = hub;
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// This method is Login 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("Login")]
        public async Task<ApiResult<AccessToken>> AxToken(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);
            var user = await _userRepository.GetFirstAsync(x => x.UserName == loginDto.Username && x.Password == passwordHash, cancellationToken);

            var address = Request.HttpContext.Connection.RemoteIpAddress;
            var computerName = address.GetDeviceName();
            var ip = address.GetIp();

            #region loginLog And configLoad

            var config = _memoryCache.GetOrCreate(CacheKeys.ConfigData, entry =>
            {
                var dataDto = _configDataRepository.TableNoTracking.ProjectTo<ConfigDataDto>().FirstOrDefault(x => x.Active);
                if (dataDto == null)
                    throw new NotFoundException("تنظیمات اولیه سامانه به درستی انجام نشده است");
                return dataDto;
            });
            var userAgent = Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            var info = uaParser.Parse(userAgent);
            var loginlog = new LoginLog
            {
                AppVersion = config.VersionName,
                Browser = info.UA.Family,
                BrowserVersion = info.UA.Major + "." + info.UA.Minor,
                UserId = user?.Id,
                CreatorUserId = user?.Id ?? 0,
                InvalidPassword = user == null ? loginDto.Password : null,
                Ip = ip,
                MachineName = computerName,
                Os = info.Device + " " + info.OS,
                UserName = loginDto.Username,
                ValidSignIn = user != null,
                InsertDateTime = DateTime.Now
            };
            await _loginlogRepository.AddAsync(loginlog, cancellationToken);
            #endregion

            if (user == null)
                return new ApiResult<AccessToken>(false, ApiResultStatusCode.UnAuthenticated, null, "نام کاربری و یا رمز عبور اشتباه است");

            var clientId = Guid.NewGuid().ToString();
            var token = await _jwtService.GenerateAsync(user, clientId);

            var userToken = new UserToken
            {
                Active = true,
                Token = token.access_token,
                UserAgent = userAgent,
                Ip = ip,
                DeviceName = computerName,
                UserId = user.Id,
                ClientId = clientId,
                CreatorUserId = user.Id,
                InsertDateTime = DateTime.Now,
                Browser = info.UA.ToString(),
                ExpireDateTime = token.expires_in.UnixTimeStampToDateTime()
            };

            //Response.Cookies.Append("AxToken", token.access_token);
            await _userTokenRepository.AddAsync(userToken, cancellationToken);

            await Task.Run(() =>
            {
                var oldTokens = _userTokenRepository.GetAll(t => t.ExpireDateTime < DateTime.Now);
                _userTokenRepository.DeleteRange(oldTokens);
            }, cancellationToken);


            var connections = _userConnectionService.GetActiveConnections();
            var barChart = _barChartRepository.GetAll(x => x.AxChartId == 5).ProjectTo<BarChartDto>().FirstOrDefault();
            if (barChart != null && barChart.Series?.Count > 0)
            {
                var date = DateTime.Now.AddDays(-15);
                var data0 = _loginlogRepository.GetAll(x => x.InsertDateTime.Date >= date.Date).ToList()
                    .GroupBy(x => x.InsertDateTime.Date).OrderBy(x => x.Key).Select(x => new
                    { Count = x.Count(), x.Key, UnScuccessCount = x.Count(t => t.ValidSignIn == false) }).ToList();
                //var data = chart.Report.Execute();
                var a = data0.Select(x => x.Count).ToList();
                var b = data0.Select(x => x.UnScuccessCount).ToList();
                barChart.Series[0] = new AxSeriesDto { Data = a, Name = "تعداد ورود به سیستم" };
                barChart.Series.Add(new AxSeriesDto { Data = b, Name = "تعداد ورود ناموفق" });
                barChart.Labels = data0.Select(x => x.Key.ToPerDateString("d MMMM")).ToList();
            }
            await _hub.Clients.Clients(connections).SendAsync("UpdateChart", barChart, cancellationToken);

            var chart = await _chartRepository.GetAll(x => x.Id == 9).Include(x => x.Report).FirstOrDefaultAsync(cancellationToken);
            var numericWidget = _numberWidgetRepository.GetAll(x => x.AxChartId == 9).ProjectTo<NumericWidgetDto>().FirstOrDefault();
            if (chart != null && numericWidget != null)
            {
                var data = chart.Report.Execute();
                numericWidget.Data = (int)data;
                numericWidget.LastUpdated = DateTime.Now.ToPerDateTimeString("yyyy/MM/dd HH:mm:ss");
            }
            await _hub.Clients.Clients(connections).SendAsync("UpdateChart", numericWidget, cancellationToken);



            await _memoryCache.GetOrCreateAsync("user" + user.Id, entry =>
              {
                  return Task.Run(() => PermissionHelper.GetKeysFromDb(_permissionRepository, _userGroupRepository, user.Id), cancellationToken);
              });

            await _userRepository.UpdateLastLoginDateAsync(user, cancellationToken);

            return token;
        }


        /// <summary>
        /// SignOut user and Remove user Token from Db
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public async Task<ApiResult> SignOut(CancellationToken cancellationToken)
        {
            var clientId = User.Identity.GetClientId();
            var userToken = await _userTokenRepository.GetFirstAsync(x => x.ClientId == clientId, cancellationToken);

            if (userToken == null)
                throw new UnauthorizedAccessException("کاربر یافت نشد");

            await _userTokenRepository.DeleteAsync(userToken, cancellationToken);

            var connections = _userConnectionService.GetActiveConnections();
            var chart = await _chartRepository.GetAll(x => x.Id == 9).Include(x => x.Report).FirstOrDefaultAsync(cancellationToken);
            var numericWidget = _numberWidgetRepository.GetAll(x => x.AxChartId == 9).ProjectTo<NumericWidgetDto>().FirstOrDefault();
            if (chart != null && numericWidget != null)
            {
                var data = chart.Report.Execute();
                numericWidget.Data = (int)data;
                numericWidget.LastUpdated = DateTime.Now.ToPerDateTimeString("yyyy/MM/dd HH:mm:ss");
            }
            await _hub.Clients.Clients(connections).SendAsync("UpdateChart", numericWidget, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Get Init Information For Main Panel
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public async Task<ApiResult<UserInfo>> GetInitData(CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstAsync(x => x.Id == UserId, cancellationToken);
            _userRepository.LoadReference(user, t => t.UserSettings);


            if (user == null)
                return new ApiResult<UserInfo>(false, ApiResultStatusCode.NotFound, null, "کاربر یافت نشد");


            var config = _memoryCache.GetOrCreate(CacheKeys.ConfigData, entry =>
            {
                var dataDto = _configDataRepository.TableNoTracking.ProjectTo<ConfigDataDto>().FirstOrDefault(x => x.Active);
                if (dataDto == null)
                    throw new NotFoundException("تنظیمات اولیه سامانه به درستی انجام نشده است");
                return dataDto;
            });


            //var menus = _menuRepository.GetAll(x => x.Active && x.ParentId == null).ProjectTo<AxSystem>();

            await _userRepository.LoadReferenceAsync(user, t => t.UserSettings, cancellationToken);
            var userInfo = new UserInfo
            {
                UserName = user.UserName,
                DateTimeNow = DateTime.Now.ToPerDateString(),
                OrganizationName = config.OrganizationName,
                OrganizationLogo = "/api/v1/General/GetOrganizationLogo",
                UserPicture = "/api/v1/users/GetUserPicture",
                UserTheme = user.UserSettings?.Theme,
                UserDisplayName = user.FullName,
                VersionName = config.VersionName,
                PCode = user.Id,
                DefaultSystemId = user.UserSettings?.DefaultSystemId,
                SystemsList = _menuRepository.GetAll(x => x.Active && x.ParentId == null).OrderBy(x => x.OrderId).ProjectTo<AxSystem>()
            };
            userInfo.UnReedMsgCount = _userMessageRepository.Count(x => x.Receivers.Any(r => r.PrimaryKey == UserId && !r.IsSeen));
            return userInfo;
        }


        /// <summary>
        /// Get Signed User Permission keys
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public ApiResult<List<string>> GetUserPermissions(CancellationToken cancellationToken)
        {
            var keys = _memoryCache.GetOrCreate("user" + UserId, entry =>
            {
                var data = PermissionHelper.GetKeysFromDb(_permissionRepository, _userGroupRepository, UserId);
                return data;
            }).ToList();
            return keys;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AxAuthorize(StateType = StateType.Ignore, Order = 0, AxOp = AxOp.UserList, ShowInMenu = true)]
        public ApiResult<IQueryable<UserSelectDto>> Get([FromQuery] DataRequest2 request)
        {
            //var d1 = DateTime.Now;
            //var result = _userRepository.GetAll().ToDataSourceResult(request.PageSize, request.PageIndex * request.PageSize, new List<Sort>(), request.Filters);

            //var users = result.Data.ProjectTo<UserSelectDto>();

            //Response.Headers.Add("X-Pagination", result.Total.ToString());
            var users = _userRepository.GetAll().OrderBy(request.Sort, request.SortType)
                .Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<UserSelectDto>();
            var d2 = DateTime.Now;
            Response.Headers.Add("X-Pagination", _userRepository.Count().ToString());
            //return Ok(users);
            return Ok(users);
        }

        /// <summary>
        /// Get User Instance By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.UserItem)]
        public ApiResult<UserDto> Get(int id)
        {
            var user = _userRepository.GetAll(x => x.Id == id).ProjectTo<UserDto>().FirstOrDefault();
            return Ok(user);
        }

        [AxAuthorize(StateType = StateType.OnlyToken)]
        [HttpPost("[action]")]
        public async Task<ApiResult> SetUserConnectionId(UserConnectionDto connectionDto, CancellationToken cancellationToken)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress;
            var clientId = User.Identity.GetClientId();
            var oldConnections = _userConnectionRepository.GetAll(x => x.UserToken.ExpireDateTime <= DateTime.Now || x.UserToken.ClientId == clientId);
            await _userConnectionRepository.DeleteRangeAsync(oldConnections, cancellationToken);

            var userToken = await _userTokenRepository.GetFirstAsync(x => x.ClientId == clientId && x.ExpireDateTime > DateTime.Now, cancellationToken);
            var ip = address.GetIp();
            var userConnection = new UserConnection
            {
                UserId = UserId,
                Ip = ip,
                ConnectionId = connectionDto.ConnectionId,
                CreatorUserId = UserId,
                UserTokenId = userToken.Id,
            };
            await _userConnectionRepository.AddAsync(userConnection, cancellationToken);
            return Ok();
        }


        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]/{userId}")]
        public async Task<IActionResult> UploadUserPic(int userId, CancellationToken cancellationToken)
        {
            if (Request.Form.Files[0] == null || Request.Form.Files[0].Length == 0)
                return Ok(new ApiResult(false, ApiResultStatusCode.BadRequest, "file not selected"));

            var files = _fileRepository.GetAll(x => x.Key == userId);
            await _fileRepository.DeleteRangeAsync(files, cancellationToken);


            await using var ms = new MemoryStream();
            await Request.Form.Files[0].CopyToAsync(ms, cancellationToken);
            var fileBytes = ms.ToArray();

            var fa = new FileAttachment
            {
                InsertDateTime = DateTime.Now,
                ContentBytes = fileBytes,
                Key = userId,
                FileName = Request.Form.Files[0].FileName,
                CreatorUserId = 1,
                ContentType = Request.Form.Files[0].ContentType,
                FileAttachmentTypeId = 1,
                Size = Request.Form.Files[0].Length,
                TypeName = "Users"
            };
            await _fileRepository.AddAsync(fa, cancellationToken);
            fa.ContentBytes = null;
            return Ok(fa);
        }


        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpGet("[action]/{userId?}")]
        public async Task<IActionResult> GetUserAvatar(CancellationToken cancellationToken, int? userId = null)
        {
            userId ??= UserId;
            var img = await _fileRepository.GetFirstAsync(x => x.FileAttachmentType.AttachmentTypeEnum == FileAttachmentTypeEnum.UserAvatar && x.TypeName == "Users" && x.Key == userId, cancellationToken);
            if (img == null)
                return NotFound();
            return File(img.ContentBytes, img.ContentType);
        }

        [HttpPost]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserInsert, Order = 1)]
        public virtual async Task<ApiResult<UserDto>> Create(UserDto dto, CancellationToken cancellationToken)
        {
            dto.Password = SecurityHelper.GetSha256Hash(dto.Password);
            await _userRepository.AddAsync(dto.ToEntity(), cancellationToken);
            var resultDto = await _userRepository.TableNoTracking.ProjectTo<UserDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserDelete, Order = 4)]
        [HttpDelete("{id}")]
        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.UserDelete)]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await _userRepository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
            await _userRepository.DeleteAsync(model, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserUpdate, Order = 3)]
        public virtual async Task<ApiResult<UserDto>> Update(UserDto dto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
            if (user == null)
                throw new NotFoundException("کاربری یافت نشد");

            await _userRepository.UpdateAsync(dto.ToEntity(user), cancellationToken);
            var resultDto = await _userRepository.TableNoTracking.ProjectTo<UserDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public virtual ApiResult<IEnumerable<UserGroupDto>> GetUsersAndGroups([FromQuery] DataRequest request)
        {
            var filter = request.Filters.FirstOrDefault()?.Value1;
            filter ??= "";
            var users = _userRepository.GetAll(x => x.IsActive && x.UserName.Contains(filter) || x.FirstName.Contains(filter) || x.FirstName.Contains(filter)).ToList().Select(x => new UserGroupDto { Id = x.Id, Type = UgType.User, Name = x.FullName, GroupLabel = "کاربر" });
            var groups = _groupRepository.GetAll(x => x.GroupName.Contains(filter)).ToList().Select(x => new UserGroupDto { Id = x.Id, Type = UgType.Group, Name = x.GroupName, GroupLabel = "گروه کاربر" });
            var result = users.Union(groups);
            return Ok(result);
        }


        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserUpdate, Order = 3)]
        public virtual async Task<ApiResult<string>> ChangePassword(UserDto dto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
            if (user == null)
                return new ApiResult<string>(false, ApiResultStatusCode.LogicError, null, "کاربری یافت نشد");

            if (dto.Password != dto.RePassword)
                return new ApiResult<string>(false, ApiResultStatusCode.LogicError, null, "رمز عبور و تکرار آن برابر نیستند");

            user.Password = SecurityHelper.GetSha256Hash(dto.Password);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return "رمز عبور تغییر کرد";
        }

    }

}
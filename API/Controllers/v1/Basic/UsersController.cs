using System;
using System.Collections.Generic;
using System.Data;
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
using Dapper;
using Dapper.Contrib.Extensions;
using Data;
using Data.Repositories;
using Entities.Framework;
using Entities.Framework.AxCharts;
using Entities.Framework.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<AxHub> _hub;
        private readonly ApplicationDbContext _dbConnection;

        /// <inheritdoc />
        public UsersController(IJwtService jwtService, IMemoryCache memoryCache, IHubContext<AxHub> hub, ApplicationDbContext dbConnection)
        {
            _jwtService = jwtService;
            _memoryCache = memoryCache;
            _hub = hub;
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// This method is Login 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]")]
        public async Task<ApiResult<AccessToken>> AxToken(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var dc = _dbConnection.CreateConnection();
            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);
            var user = await dc.QueryFirstOrDefaultAsync<User>("select Id, LastLoginDate from Users where username = @username and password = @passwordHash", new { username = loginDto.Username, passwordHash });

            var address = Request.HttpContext.Connection.RemoteIpAddress;
            var computerName = address.GetDeviceName();
            var ip = address.GetIp();

            #region loginLog And configLoad

            var config = await _memoryCache.GetOrCreate(CacheKeys.ConfigData, async entry =>
            {
                var dataDto = await dc.QueryFirstOrDefaultAsync<ConfigDataDto>("select VersionName , OrganizationName from ConfigData where Active = 1");
                return dataDto ?? throw new NotFoundException(@"تنظیمات اولیه سامانه به درستی انجام نشده است");
            });

            var userAgent = Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            var info = uaParser.Parse(userAgent);

            var loginLog = new LoginLog
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
            await dc.InsertAsync(loginLog);
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

            await Task.Run(() =>
            {
                dc.ExecuteAsync("delete from UserTokens where ExpireDateTime < getdate()");
            }, cancellationToken);

            await dc.InsertAsync(userToken);

            //var connections = _userConnectionService.GetActiveConnections();
            //var barChart = _barChartRepository.GetAll(x => x.AxChartId == 5).ProjectTo<BarChartDto>().FirstOrDefault();
            //if (barChart != null && barChart.Series?.Count > 0)
            //{
            //    var date = DateTime.Now.AddDays(-15);
            //    var data0 = _loginlogRepository.GetAll(x => x.InsertDateTime.Date >= date.Date).ToList()
            //        .GroupBy(x => x.InsertDateTime.Date).OrderBy(x => x.Key).Select(x => new
            //        { Count = x.Count(), x.Key, UnScuccessCount = x.Count(t => t.ValidSignIn == false) }).ToList();
            //    //var data = chart.Report.Execute();
            //    var a = data0.Select(x => x.Count).ToList();
            //    var b = data0.Select(x => x.UnScuccessCount).ToList();
            //    barChart.Series[0] = new AxSeriesDto { Data = a, Name = "تعداد ورود به سیستم" };
            //    barChart.Series.Add(new AxSeriesDto { Data = b, Name = "تعداد ورود ناموفق" });
            //    barChart.Labels = data0.Select(x => x.Key.ToPerDateString("d MMMM")).ToList();
            //}
            //await _hub.Clients.Clients(connections).SendAsync("UpdateChart", barChart, cancellationToken);

            //var chart = await _chartRepository.GetAll(x => x.Id == 9).Include(x => x.Report).FirstOrDefaultAsync(cancellationToken);
            //var numericWidget = _numberWidgetRepository.GetAll(x => x.AxChartId == 9).ProjectTo<NumericWidgetDto>().FirstOrDefault();
            //if (chart != null && numericWidget != null)
            //{
            //    var data = chart.Report.Execute();
            //    numericWidget.Data = (int)data;
            //    numericWidget.LastUpdated = DateTime.Now.ToPerDateTimeString("yyyy/MM/dd HH:mm:ss");
            //}
            //await _hub.Clients.Clients(connections).SendAsync("UpdateChart", numericWidget, cancellationToken);



            //await _memoryCache.GetOrCreateAsync("user" + user.Id, entry =>
            //  {
            //      return Task.Run(() => PermissionHelper.GetKeysFromDb(_permissionRepository, _userGroupRepository, user.Id), cancellationToken);
            //  });

            await dc.UpdateAsync(user);

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
            var dc = _dbConnection.CreateConnection();
            var clientId = User.Identity.GetClientId();
            var userToken = await dc.QueryFirstOrDefaultAsync<UserToken>("select * from UserTokens where ClientId = @clientId", new { clientId });

            if (userToken == null)
                throw new UnauthorizedAccessException("کاربر یافت نشد");

            await dc.DeleteAsync(userToken);

            //var connections = _userConnectionService.GetActiveConnections();
            //var chart = await _chartRepository.GetAll(x => x.Id == 9).Include(x => x.Report).FirstOrDefaultAsync(cancellationToken);
            //var numericWidget = _numberWidgetRepository.GetAll(x => x.AxChartId == 9).ProjectTo<NumericWidgetDto>().FirstOrDefault();
            //if (chart != null && numericWidget != null)
            //{
            //    var data = chart.Report.Execute();
            //    numericWidget.Data = (int)data;
            //    numericWidget.LastUpdated = DateTime.Now.ToPerDateTimeString("yyyy/MM/dd HH:mm:ss");
            //}
            //await _hub.Clients.Clients(connections).SendAsync("UpdateChart", numericWidget, cancellationToken);

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
            var dc = _dbConnection.CreateConnection();
            var userInfo = await dc.QueryFirstOrDefaultAsync<UserInfo>(@"SELECT u.Id as PCode, u.username,CONCAT(u.FIRSTNAME, ' ', u.LASTNAME) as UserDisplayName,us.Theme as UserTheme , us.DefaultSystemId FROM Users 
            u left JOIN UserSettings us ON u.Id=us.UserId where u.Id = @id", new { id = UserId });


            if (userInfo == null)
                return new ApiResult<UserInfo>(false, ApiResultStatusCode.NotFound, null, "کاربر یافت نشد");


            var config = await _memoryCache.GetOrCreate(CacheKeys.ConfigData, async entry =>
            {
                var dataDto = await dc.QueryFirstOrDefaultAsync<ConfigDataDto>("select VersionName , OrganizationName from ConfigData where Active = 1");
                return dataDto ?? throw new NotFoundException(@"تنظیمات اولیه سامانه به درستی انجام نشده است");
            });

            //var menus = _menuRepository.GetAll(x => x.Active && x.ParentId == null).ProjectTo<AxSystem>();

            userInfo.DateTimeNow = DateTime.Now.ToPerDateString();
            userInfo.OrganizationName = config.OrganizationName;
            userInfo.OrganizationLogo = "/api/v1/General/GetOrganizationLogo";
            userInfo.UserPicture = "/api/v1/users/GetUserPicture";
            userInfo.VersionName = config.VersionName;
            //userInfo.SystemsList = _menuRepository.GetAll(x => x.Active && x.ParentId == null).OrderBy(x => x.OrderId).ProjectTo<AxSystem>();
            //userInfo.UnReedMsgCount = _userMessageRepository.Count(x => x.Receivers.Any(r => r.PrimaryKey == UserId && !r.IsSeen));
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
            //var keys = _memoryCache.GetOrCreate("user" + UserId, entry =>
            //{
            //    var data = PermissionHelper.GetKeysFromDb(_permissionRepository, _userGroupRepository, UserId);
            //    return data;
            //}).ToList();
            //return keys;
            return Ok();
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
            const string query = @"SELECT [Id]
      ,[InsertDateTime]
      ,[ModifiedDateTime]
      ,[CreatorUserId]
      ,[ModifierUserId]
      ,[UserName]
      ,[IsActive]
      ,[FirstName]
      ,[LastName]
      ,[UniqueKey]
      ,[Email]
      ,[LoginType]
      ,[ExpireDateTime]
      ,[LastLoginDate]
  FROM [Users] where id =@id";
            var dc = _dbConnection.CreateConnection();
            var user = dc.QueryFirstOrDefaultAsync<UserDto>(query, new { id });
            return Ok(user);
        }

        [AxAuthorize(StateType = StateType.OnlyToken)]
        [HttpPost("[action]")]
        public async Task<ApiResult> SetUserConnectionId(UserConnectionDto connectionDto, CancellationToken cancellationToken)
        {
            //var address = Request.HttpContext.Connection.RemoteIpAddress;
            //var clientId = User.Identity.GetClientId();
            //var oldConnections = _userConnectionRepository.GetAll(x => x.UserToken.ExpireDateTime <= DateTime.Now || x.UserToken.ClientId == clientId);
            //await _userConnectionRepository.DeleteRangeAsync(oldConnections, cancellationToken);

            //var userToken = await _userTokenRepository.GetFirstAsync(x => x.ClientId == clientId && x.ExpireDateTime > DateTime.Now, cancellationToken);
            //var ip = address.GetIp();
            //var userConnection = new UserConnection
            //{
            //    UserId = UserId,
            //    Ip = ip,
            //    ConnectionId = connectionDto.ConnectionId,
            //    CreatorUserId = UserId,
            //    UserTokenId = userToken.Id,
            //};
            //await _userConnectionRepository.AddAsync(userConnection, cancellationToken);
            return Ok();
        }


        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]/{userId}")]
        public async Task<IActionResult> UploadUserPic(int userId, CancellationToken cancellationToken)
        {
            //if (Request.Form.Files[0] == null || Request.Form.Files[0].Length == 0)
            //    return Ok(new ApiResult(false, ApiResultStatusCode.BadRequest, "file not selected"));

            //var files = _fileRepository.GetAll(x => x.Key == userId);
            //await _fileRepository.DeleteRangeAsync(files, cancellationToken);


            //await using var ms = new MemoryStream();
            //await Request.Form.Files[0].CopyToAsync(ms, cancellationToken);
            //var fileBytes = ms.ToArray();

            var fa = new FileAttachment
            {
                InsertDateTime = DateTime.Now,
                //ContentBytes = fileBytes,
                Key = userId,
                FileName = Request.Form.Files[0].FileName,
                CreatorUserId = 1,
                ContentType = Request.Form.Files[0].ContentType,
                FileAttachmentTypeId = 1,
                Size = Request.Form.Files[0].Length,
                TypeName = "Users"
            };
            //await _fileRepository.AddAsync(fa, cancellationToken);
            fa.ContentBytes = null;
            return Ok(fa);
        }


        //[AxAuthorize(StateType = StateType.Ignore)]
        //[HttpGet("[action]/{userId?}")]
        //public async Task<IActionResult> GetUserAvatar(CancellationToken cancellationToken, int? userId = null)
        //{
        //    userId ??= UserId;
        //    var img = await _fileRepository.GetFirstAsync(x => x.FileAttachmentType.AttachmentTypeEnum == FileAttachmentTypeEnum.UserAvatar && x.TypeName == "Users" && x.Key == userId, cancellationToken);
        //    if (img == null)
        //        return NotFound();
        //    return File(img.ContentBytes, img.ContentType);
        //}

        [HttpPost]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserInsert, Order = 1)]
        public virtual async Task<ApiResult<UserDto>> Create(UserDto dto, CancellationToken cancellationToken)
        {
            var dc = _dbConnection.CreateConnection();
            dto.Password = SecurityHelper.GetSha256Hash(dto.Password);
            var id = await dc.InsertAsync(dto.ToEntity());
            dto.Id = id;
            return dto;
        }

        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserDelete, Order = 4)]
        [HttpDelete("{id}")]
        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.UserDelete)]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var dc = _dbConnection.CreateConnection();
            await dc.ExecuteAsync("delete from users where id = @id", new { id });
            return Ok();
        }

        [HttpPut]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserUpdate, Order = 3)]
        public virtual async Task<ApiResult<UserDto>> Update(UserDto dto, CancellationToken cancellationToken)
        {
            var dc = _dbConnection.CreateConnection();
            await dc.UpdateAsync(dto.ToEntity());
            return dto;
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public virtual ApiResult<IEnumerable<UserGroupDto>> GetUsersAndGroups([FromQuery] DataRequest request)
        {
            var dc = _dbConnection.CreateConnection();
            var filter = request.Filters.FirstOrDefault()?.Value1;
            filter ??= "";
            var query = "Select Id,1 as [Type], CONCAT(firstname,' ',LastName) as [Name] from Users Union Select Id, 2 as [Type], GroupName as [Name]  from AxGroups";
            var result = dc.Query<UserGroupDto>(query);
            return Ok(result);
        }


        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserUpdate, Order = 3)]
        public virtual async Task<ApiResult<string>> ChangePassword(UserDto dto, CancellationToken cancellationToken)
        {
            var dc = _dbConnection.CreateConnection();

            var user = await dc.QueryFirstOrDefaultAsync<User>("select * from Users where id = @id", new { id = dto.Id });
            if (user == null)
                return new ApiResult<string>(false, ApiResultStatusCode.LogicError, null, "کاربری یافت نشد");

            if (dto.Password != dto.RePassword)
                return new ApiResult<string>(false, ApiResultStatusCode.LogicError, null, "رمز عبور و تکرار آن برابر نیستند");

            user.Password = SecurityHelper.GetSha256Hash(dto.Password);
            await dc.UpdateAsync(user);
            return "رمز عبور تغییر کرد";
        }

    }

}
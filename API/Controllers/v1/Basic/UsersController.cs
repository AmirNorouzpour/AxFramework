using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
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
using Entities.MasterSignal;
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
using Guid = System.Guid;

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
        private readonly IBaseRepository<AxSignal> _signalsRepository;
        private readonly IBaseRepository<AnalysisMsg> _analysisMsgRepository;

        /// <inheritdoc />
        public UsersController(IUserRepository userRepository, IJwtService jwtService, IBaseRepository<LoginLog> loginlogRepository, IMemoryCache memoryCache,
            IBaseRepository<UserToken> userTokenRepository, IBaseRepository<Menu> menuRepository, IBaseRepository<ConfigData> configDataRepository,
            IBaseRepository<UserGroup> userGroupRepository, IBaseRepository<FileAttachment> fileRepository, IBaseRepository<UserMessage> userMessageRepository, IUserConnectionService userConnectionService, IBaseRepository<UserConnection> userConnectionRepository, IBaseRepository<AxChart> chartRepository, IBaseRepository<BarChart> barChartRepository, IBaseRepository<NumericWidget> numberWidgetRepository, IHubContext<AxHub> hub, IBaseRepository<AxGroup> groupRepository, IBaseRepository<AxSignal> signalsRepository, IBaseRepository<AnalysisMsg> analysisMsgRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _loginlogRepository = loginlogRepository;
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
            _memoryCache = memoryCache;
            _hub = hub;
            _groupRepository = groupRepository;
            _signalsRepository = signalsRepository;
            _analysisMsgRepository = analysisMsgRepository;
        }

        /// <summary>
        /// This method is Login 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AxAuthorize(StateType = StateType.UniqueKey)]
        [HttpPost("[action]")]
        public async Task<ApiResult<AccessToken>> AxToken(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);
            var user = await _userRepository.GetFirstAsync(x => x.UserName == loginDto.Username && x.Password == passwordHash, cancellationToken);

            var address = Request.HttpContext.Connection.RemoteIpAddress;
            var computerName = address.GetDeviceName();
            var ip = address.GetIp();

            #region loginLog And configLoad

            var userAgent = Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            var info = uaParser.Parse(userAgent);
            var loginlog = new LoginLog
            {
                Browser = info.UA.Family,
                BrowserVersion = info.UA.Major + "." + info.UA.Minor,
                UserId = user?.Id,
                CreatorUserId = user?.Id ?? 0,
                InvalidPassword = user == null ? loginDto.Password : null,
                Ip = ip,
                AppVersion = "FromAxTokenLogin",
                MachineName = computerName,
                Os = info.Device + " " + info.OS,
                UserName = loginDto.Username,
                ValidSignIn = user != null,
                InsertDateTime = DateTime.Now
            };
            await _loginlogRepository.AddAsync(loginlog, cancellationToken);
            #endregion

            if (user == null)
                return new ApiResult<AccessToken>(false, ApiResultStatusCode.UnAuthenticated, null, "Username or Password is incorrect!");

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

            await _userRepository.UpdateLastLoginDateAsync(user, cancellationToken);

            return token;
        }

        [AxAuthorize(StateType = StateType.UniqueKey)]
        [HttpPost("[action]")]
        public async Task<ApiResult<AccessToken>> AxToken2(LoginDto loginDto, CancellationToken cancellationToken)
        {
            if (loginDto.Username != "admin")
                return new ApiResult<AccessToken>(false, ApiResultStatusCode.UnAuthenticated, null, "Username or Password is incorrect!");

            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);
            var user = await _userRepository.GetFirstAsync(x => x.UserName == loginDto.Username && x.Password == passwordHash, cancellationToken);

            var address = Request.HttpContext.Connection.RemoteIpAddress;
            var computerName = address.GetDeviceName();
            var ip = address.GetIp();

            #region loginLog And configLoad

            var userAgent = Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            var info = uaParser.Parse(userAgent);
            var loginlog = new LoginLog
            {
                Browser = info.UA.Family,
                BrowserVersion = info.UA.Major + "." + info.UA.Minor,
                UserId = user?.Id,
                CreatorUserId = user?.Id ?? 0,
                InvalidPassword = user == null ? loginDto.Password : null,
                Ip = ip,
                AppVersion = "FromAxTokenLogin",
                MachineName = computerName,
                Os = info.Device + " " + info.OS,
                UserName = loginDto.Username,
                ValidSignIn = user != null,
                InsertDateTime = DateTime.Now
            };
            await _loginlogRepository.AddAsync(loginlog, cancellationToken);
            #endregion

            if (user == null)
                return new ApiResult<AccessToken>(false, ApiResultStatusCode.UnAuthenticated, null, "Username or Password is incorrect!");

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

            await _userRepository.UpdateLastLoginDateAsync(user, cancellationToken);

            return token;
        }


        [AxAuthorize(StateType = StateType.UniqueKey)]
        [HttpPost("[action]")]
        public async Task<ApiResult<string>> SetFBToken(FBTokenDto tokenDto, CancellationToken cancellationToken)
        {
            var key = Request.Headers["key"];
            var user = await _userRepository.GetFirstAsync(x => x.UniqueKey == key, cancellationToken);
            user.FireBaseToken = tokenDto.Token;
            await _userRepository.UpdateAsync(user, cancellationToken);
            return new ApiResult<string>(true, ApiResultStatusCode.Success, null, "Update Successful");
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
                return new ApiResult<UserInfo>(false, ApiResultStatusCode.NotFound, null, "User not found");


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

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public async Task<ApiResult<IQueryable<AxSignal>>> GetSignals()
        {
            var user = _userRepository.GetFirst(x => x.Id == UserId);
            if (user.UserName != "admin")
                return null;
            var signals = _signalsRepository.GetAll().OrderByDescending(x => x.InsertDateTime);
            return Ok(signals);
        }

        //[HttpGet("[action]")]
        //[AxAuthorize(StateType = StateType.Ignore)]
        //public async Task<FileResult> GetSymbolChart(CancellationToken cancellationToken)
        //{
        //    var bmp = new Bitmap(1800, 600);
        //    var blackPen = new Pen(Color.LimeGreen, 3);

        //    var list = new List<Point>();
        //    for (var i = 1; i <= 50; i++)
        //    {
        //        Thread.Sleep(10);
        //        list.Add(new Point(i * 75, new Random((int)DateTime.Now.Ticks).Next(30, 550)));
        //    }


        //    using (var graphics = Graphics.FromImage(bmp))
        //    {
        //        graphics.DrawCurve(blackPen, list.ToArray());
        //        pictureBox1.Image = bmp;
        //        bmp.Save(@"F:\test.png");
        //    }
        //}



        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.UniqueKey)]
        public async Task<ApiResult<UserMainData>> GetMainData(CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstAsync(x => x.Id == UserId, cancellationToken);
            var key = Request.Headers["key"];
            var data = _memoryCache.Get<GlobalResult>(CacheKeys.MainData);

            var userInfo = new UserMainData
            {
                UserName = user?.UserName,
                IsSignedIn = UserId != 0,
                UserDisplayName = user?.FullName,
                Email = user?.Email,
                ExpireDateTime = user?.ExpireDateTime,
                FGIndex = data
            };
            //userInfo.UnReedMsgCount = _userMessageRepository.Count(x => x.Receivers.Any(r => r.PrimaryKey == UserId && !r.IsSeen));

            try
            {
                var address = Request.HttpContext.Connection.RemoteIpAddress;
                var ip = address.GetIp();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var uaParser = Parser.GetDefault();
                var info = uaParser.Parse(userAgent);
                var loginlog = new LoginLog
                {
                    Browser = info.UA.Family,
                    BrowserVersion = info.UA.Major + "." + info.UA.Minor,
                    UserId = user?.Id,
                    CreatorUserId = user?.Id ?? 0,
                    Ip = ip,
                    AppVersion = "FromGetMainData",
                    Os = info.Device + " " + info.OS,
                    UserName = key.ToString(),
                    ValidSignIn = user != null,
                    InsertDateTime = DateTime.Now
                };
                await _loginlogRepository.AddAsync(loginlog, cancellationToken);
            }
            catch (Exception)
            {
                // ignored
            }

            return userInfo;
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.UniqueKey)]
        public ApiResult<List<Item>> GetNews(CancellationToken cancellationToken)
        {
            var data = _memoryCache.Get<List<Item>>(CacheKeys.NewsData);
            return data;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.UniqueKey)]
        public async Task<ApiResult<List<AxPositionDto>>> GetPositions(CancellationToken cancellationToken)
        {
            var symbols = _memoryCache.Get<List<Symbol>>(CacheKeys.SymbolsData);
            var key = Request.Headers["key"].ToString();
            var user = await _userRepository.GetFirstAsync(x => x.UniqueKey == key, cancellationToken);
            var isVip = user?.ExpireDateTime > DateTime.UtcNow;
            var list = _memoryCache.Get<List<AxPosition>>(CacheKeys.PositionsData);

            var data = list.Select(x => new AxPositionDto
            {
                EnterPrice = isVip || x.IsFree ? Format(x.EnterPrice, symbols, x.Symbol).ToString() : "",
                StopLoss = isVip || x.IsFree ? Format(x.StopLoss, symbols, x.Symbol).ToString() : "",
                Id = x.Id,
                Targets = isVip || x.IsFree ? x.Targets : "",
                Symbol = x.Symbol.Replace("USDT", "/USDT"),
                Side = isVip || x.IsFree ? x.Side.ToString() : "",
                Status = x.Status.ToString(),
                Leverage = isVip || x.IsFree ? x.Leverage : 0,
                Capital = isVip || x.IsFree ? x.Capital : "",
                Risk = x.Risk,
                Price = Format(x.Price, symbols, x.Symbol),
                DateTime = isVip || x.IsFree ? x.DateTime.ToString("yyyy/MM/dd HH:mm") : "",
                IsFree = x.IsFree,
                Max = isVip || x.IsFree ? decimal.Parse(x.Max.ToString("n2")) : 0,
                StopMoved = isVip || x.IsFree ? x.StopMoved : null,
                ProfitPercent = isVip || x.IsFree ? decimal.Parse(x.ProfitPercent.ToString("n2")) : 0,
                LeverageMode = x.SuggestedLeverage,
                MaxTargetPercent = GetMaxTargetPercent(x),
                Result = x.Result.ToString()
            }).ToList();
            return data;
        }

        private static decimal GetMaxTargetPercent(AxPosition x)
        {
            var lastTp = x.TargetsList.LastOrDefault();
            var diff = Math.Abs(lastTp - x.EnterPrice);
            var maxTargetPercent = x.Side == PositionSide.Long ? decimal.Parse((diff * 100 / x.EnterPrice * x.Leverage).ToString("n2")) : 100 - (decimal.Parse((diff * 100 / x.EnterPrice * x.Leverage).ToString("n2")));
            return maxTargetPercent;
        }


        [HttpGet("[action]/{type}")]
        [AxAuthorize(StateType = StateType.UniqueKey)]
        public async Task<ApiResult<List<AnalysisMsgDto>>> GetMsg(AnalysisMsgType type, CancellationToken cancellationToken)
        {
            var key = Request.Headers["key"].ToString();
            var user = await _userRepository.GetFirstAsync(x => x.UniqueKey == key, cancellationToken);
            var isVip = user?.ExpireDateTime > DateTime.UtcNow;
            var list = _analysisMsgRepository.GetAll(x => x.Type == type).OrderByDescending(x => x.DateTime).Take(30);
            var data = list.Select(x => new AnalysisMsgDto
            {
                Id = x.Id,
                Title = x.Title,
                Side = x.Side,
                DateTime = x.DateTime.ToString("yyyy/MM/dd HH:mm"),
                Content = x.Content,
                Tags = x.Tags,
                Type = x.Type.ToString(),
                ImageLink = "http://65.108.14.168:8081/api/v1/users/GetMsgImg/" + x.Id + "/" + x.CreatorUserId,
                Views = 0
            });
            return data.ToList();
        }

        [HttpGet("[action]/{id}/{cId}")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IActionResult> GetMsgImg(int id, int cid, CancellationToken cancellationToken)
        {
            var file = await _fileRepository.GetFirstAsync(x => x.Key == id && x.TypeName == "Analysis", cancellationToken);
            if (file == null)
                return NoContent();
            return File(file.ContentBytes, file.ContentType);
        }

        private static decimal Format(decimal input, List<Symbol> symbols, string symbol)
        {
            var s = symbols.FirstOrDefault(x => x.Title == symbol);
            if (s == null)
                return input;
            var format = "n" + s.Decimals;
            return decimal.Parse(input.ToString(format));
        }

        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]")]
        public async Task<ApiResult<int>> SetUnique(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstAsync(x => x.UniqueKey == loginDto.Key, cancellationToken);
            if (user != null)
                return user.Id;

            var u = new User
            {
                InsertDateTime = DateTime.Now,
                UniqueKey = loginDto.Key,
                IsActive = true,
                UserName = loginDto.Key,
                FirstName = loginDto.Key,
                LastName = loginDto.Key,
            };
            await _userRepository.AddAsync(u, cancellationToken);
            return u.Id;

        }

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.UniqueKey)]
        public async Task<ApiResult<int>> Register(UserDto dto, CancellationToken cancellationToken)
        {
            var key = Request.Headers["key"];
            var user = await _userRepository.GetFirstAsync(x => x.UniqueKey == key.ToString(), cancellationToken);
            if (!user.BirthDate.HasValue)
            {
                user.UserName = dto?.UserName;
                user.Email = dto?.UserName;
                user.FirstName = dto?.FirstName;
                user.LastName = dto?.LastName;
                user.Password = SecurityHelper.GetSha256Hash(dto?.Password);
                user.BirthDate = DateTime.Now;
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
            return user.Id;
        }


        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

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
        [AxAuthorize(StateType = StateType.OnlyToken, Order = 0, AxOp = AxOp.UserList, ShowInMenu = true)]
        public ApiResult<IQueryable<UserSelectDto>> Get([FromQuery] DataRequest request)
        {
            var user = _userRepository.GetFirst(x => x.Id == UserId);
            if (user.UserName != "admin")
                return null;

            var predicate = request.GetFilter<User>();
            var users = _userRepository.GetAll(predicate).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<UserSelectDto>();
            Response.Headers.Add("X-Pagination", _userRepository.Count(predicate).ToString());
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


    }

    public class UserMainData
    {
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string Email { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public object FGIndex { get; set; }
        public bool IsSignedIn { get; set; }
    }
}
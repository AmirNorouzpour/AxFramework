//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using API.Models;
//using AutoMapper.QueryableExtensions;
//using Common;
//using Data.Repositories.UserRepositories;
//using Microsoft.AspNetCore.Mvc;
//using WebFramework.Api;
//using WebFramework.Filters;

//namespace API.Controllers.v1.Report
//{
//    [ApiVersion("1")]
//    public class BasicReportController : ControllerBase
//    {
//        private readonly IUserRepository _userRepository;
//        public BasicReportController(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        [HttpGet("[action]")]
//        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UserListReport, Order = 0, ShowInMenu = true)]
//        public ApiResult<IQueryable<UserDto>> GetUserListReport(CancellationToken cancellationToken)
//        {
//            return Ok(_userRepository.GetAll().ProjectTo<UserDto>());
//        }

//        [HttpGet("[action]")]
//        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.UnSuccessfullyLoginReport, Order = 1, ShowInMenu = true)]
//        public ApiResult<List<UserDto>> GetUnSuccessfullyLoginReport(CancellationToken cancellationToken)
//        {
//            return null;
//        }

//    }
//}
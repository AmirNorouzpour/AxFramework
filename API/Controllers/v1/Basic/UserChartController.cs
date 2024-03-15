//using API.Models;
//using Common;
//using Data.Repositories;
//using Entities.Framework;
//using Microsoft.AspNetCore.Mvc;
//using WebFramework.Api;
//using WebFramework.Filters;

//namespace API.Controllers.v1.Basic
//{
//    [ApiVersion("1")]
//    public class UserChartsController : BaseController
//    {
//        private readonly IBaseRepository<UserChart> _userChartRepository;

//        public UserChartsController(IBaseRepository<UserChart> userChartRepository)
//        {
//            _userChartRepository = userChartRepository;
//        }

//        [HttpDelete("{id}")]
//        [AxAuthorize(StateType = StateType.OnlyToken)]
//        public ApiResult Delete(int id)
//        {
//            var chart = _userChartRepository.GetFirst(x => x.Id == id && x.UserId == UserId);
//            if (chart == null)
//                return new ApiResult(false, ApiResultStatusCode.NotFound, "نموداری یافت نشد");
//            _userChartRepository.Delete(chart);
//            return Ok();
//        }
//    }
//}
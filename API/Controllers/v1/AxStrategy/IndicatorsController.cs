using System.Collections.Generic;
using API.Data;
using Common;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;


namespace API.Controllers.v1.AxStrategy
{
    [ApiVersion("1")]
    public class IndicatorsController : BaseController
    {

        private readonly IBaseRepository<Log> _logRepository;

        public IndicatorsController()
        {
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public ApiResult<IEnumerable<IndicatorGroup>> GetMenuData()
        {
            var data = MenuData.GetMenuData();
            return Ok(data);
        }
    }
}

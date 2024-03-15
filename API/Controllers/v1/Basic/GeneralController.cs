using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Utilities;
using Dapper;
using Data;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class GeneralController : BaseController
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GeneralController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IActionResult> GetOrganizationLogo(CancellationToken cancellationToken)
        {
            var dc = _applicationDbContext.CreateConnection();
            var dataDto = await dc.QueryFirstOrDefaultAsync<ConfigData>("select * from ConfigData where Active = 1");

            if (dataDto == null)
                return NotFound();

            return File(dataDto.OrganizationLogo.ToArray(), GeneralUtils.GetContentType("a.png"));
        }
    }
}
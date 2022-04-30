using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Objects.Models.Futures;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class GeneralController : BaseController
    {
        private readonly IBaseRepository<ConfigData> _repository;

        public GeneralController(IBaseRepository<ConfigData> repository)
        {
            _repository = repository;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IActionResult> GetOrganizationLogo(CancellationToken cancellationToken)
        {
            var data = await _repository.GetFirstAsync(x => x.Active, cancellationToken);

            if (data == null)
                return NotFound();

            return File(data.OrganizationLogo.ToArray(), GeneralUtils.GetContentType("a.png"));
        }

    }
    public class PositionModel
    {
        public List<BinancePositionInfoUsdt> List { get; set; } = new();
        public decimal TotalWallet { get; set; }
        public decimal TotalUnPnl { get; set; }
        public decimal TotalMargin { get; set; }
        public string MobileNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal InitBalance { get; set; }
    }
}
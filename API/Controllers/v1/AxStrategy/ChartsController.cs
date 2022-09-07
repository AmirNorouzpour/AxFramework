using Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Net.Interfaces;
using WebFramework.Api;
using WebFramework.Filters;
using Binance.Net.Interfaces.Clients;
using Common.Utilities;

namespace API.Controllers.v1.AxStrategy
{
    [ApiVersion("1")]
    public class ChartsController : BaseController
    {
        private readonly IBinanceClient _client;

        public ChartsController(IBinanceClient client)
        {
            _client = client;
        }
        [HttpGet("[action]/{symbol}/{interval}")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IEnumerable<IBinanceKline>> GetKLines(string symbol, int interval)
        {
            var data = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, interval.ToInterval(), null, null, 1500);
            return data.Data;
        }
    }
}

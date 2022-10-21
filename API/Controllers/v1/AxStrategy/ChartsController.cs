using Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using WebFramework.Api;
using WebFramework.Filters;
using Binance.Net.Interfaces.Clients;
using Common.Utilities;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using CryptoExchange.Net.CommonObjects;

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
            var data = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, null, 1500);
            var endTime = data.Data?.FirstOrDefault()?.OpenTime;
            var kLines2 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime, 1500);
            var endTime2 = kLines2.Data?.FirstOrDefault()?.OpenTime;
            var kLines3 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime2, 1500);
            var endTime3 = kLines3.Data?.FirstOrDefault()?.OpenTime;
            var kLines4 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime3, 1500);
            var endTime4 = kLines3.Data?.FirstOrDefault()?.OpenTime;
            var kLines5 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime4, 1500);
            var endTime5 = kLines4.Data?.FirstOrDefault()?.OpenTime;
            var kLines6 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime5, 1500);
            var endTime6 = kLines5.Data?.FirstOrDefault()?.OpenTime;
            var kLines7 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime6, 1500);
            var endTime7 = kLines6.Data?.FirstOrDefault()?.OpenTime;
            var kLines8 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime7, 1500);
            var endTime8 = kLines7.Data?.FirstOrDefault()?.OpenTime;
            var kLines9 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime8, 1500);
            var endTime9 = kLines8.Data?.FirstOrDefault()?.OpenTime;
            var kLines10 = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync("AAVEUSDT", KlineInterval.FifteenMinutes, null, endTime9, 1500);

            //return data.Data.Select(x => new Quote { Close = x.ClosePrice, Date = x.OpenTime }).GetRsi(14);
            var res = data.Data.ToList();
            res.AddRange(kLines2.Data);
            res.AddRange(kLines3.Data);
            res.AddRange(kLines4.Data);
            res.AddRange(kLines5.Data);
            res.AddRange(kLines6.Data);
            res.AddRange(kLines7.Data);
            res.AddRange(kLines8.Data);
            res.AddRange(kLines9.Data);
            res.AddRange(kLines10.Data);
            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Common;
using Common.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace API.Hubs
{
    public class TimedHostedSymbolService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly BinanceClient _client;
        private readonly IMemoryCache _memoryCache;
        public TimedHostedSymbolService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _client = new BinanceClient();
        }
        DateTime _last = DateTime.Now;
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            await GetInitData(stoppingToken, false);
        }

        private async Task GetInitData(CancellationToken stoppingToken, bool check)
        {
            var list = new List<string>
            {
                "1INCHUSDT", "AAVEUSDT", "ADAUSDT", "MATICUSDT", "ALGOUSDT", "ALPHAUSDT", "ALICEUSDT", "ANKRUSDT", "ATOMUSDT",
                "SNXUSDT", "AVAXUSDT", "ZECUSDT", "SKLUSDT", "LRCUSDT", "DYDXUSDT", "WAVESUSDT", "OCEANUSDT", "LUNAUSDT",
                "ROSEUSDT", "ONEUSDT", "ENJUSDT", "CRVUSDT", "YFIUSDT", "THETAUSDT", "DOTUSDT", "GALAUSDT", "VETUSDT",
                "COMPUSDT", "SOLUSDT", "KAVAUSDT", "XTZUSDT", "CHZUSDT", "SUSHIUSDT", "EGLDUSDT"
            };

            var symbols = _memoryCache.Get<List<Symbol>>(CacheKeys.SymbolsData);
            foreach (var symbol in list)
            {
                var symbol0 = symbols.FirstOrDefault(x => x.Title == symbol);
                var data = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, KlineInterval.OneHour, null, null,
               201, stoppingToken);
                var axbs = new AxBs
                {
                    Candles = new List<AxBsCandle>(),
                    Symbol = symbol
                };
                var klines = data.Data.ToList();
                for (var i = 0; i < klines.Count(); i++)
                {
                    var candle = klines[i];
                    if (i == 0)
                    {
                        axbs.Candles.Add(
                            new AxBsCandle
                            {
                                Index = i + 1,
                                Open = candle.OpenPrice,
                                HaOpen = candle.OpenPrice,
                                High = candle.HighPrice,
                                HaHigh = candle.HighPrice,
                                Low = candle.LowPrice,
                                HaLow = candle.LowPrice,
                                Close = candle.ClosePrice,
                                HaClose = candle.ClosePrice,
                                Date = candle.OpenTime.ToLocalTime(),
                            }
                        );
                    }
                    else
                    {
                        var pre = axbs.Candles[i - 1];
                        axbs.Candles.Add(
                            new AxBsCandle
                            {
                                Open = candle.OpenPrice,
                                HaOpen = (pre.HaOpen + pre.HaClose) / 2,
                                High = candle.HighPrice,
                                HaHigh = Math.Max(Math.Max(candle.HighPrice, pre.HaOpen), pre.HaClose),
                                Low = candle.LowPrice,
                                HaLow = Math.Min(Math.Min(candle.LowPrice, pre.HaOpen), pre.HaClose),
                                Close = candle.ClosePrice,
                                HaClose = (candle.ClosePrice + candle.OpenPrice + candle.LowPrice + candle.HighPrice) / 4,
                                Date = candle.OpenTime.ToLocalTime(),
                                Index = i + 1,
                            }
                        );
                    }
                }

                if (check)
                {
                    var p = axbs!.Candles[^3];
                    var c = axbs!.Candles[^2];
                    if (p == null)
                        return;

                    var isRed = c.HaOpen > c.HaClose;
                    var body = Math.Abs(c.HaOpen - c.HaClose);
                    var us = isRed ? c.HaHigh - c.HaOpen : c.HaHigh - c.HaClose;
                    var ds = isRed ? c.HaClose - c.HaLow : c.HaOpen - c.HaLow;
                    var div = us / ds;
                    if (us < ds)
                        div = ds / us;

                    var axRsi = new AxRsi(14);
                    axRsi.Load(axbs.Candles.Select(candle => new AxOhlc
                    {
                        Open = candle.Open,
                        High = candle.High,
                        Low = candle.Low,
                        Close = candle.Close,
                        Date = candle.Date

                    }).ToList());

                    var listRange = axbs.Candles.GetRange(axbs.Candles.Count - 200, 200);
                    var h = listRange.Max(x => x.High);
                    var l = listRange.Min(x => x.Low);

                    var lastI = listRange.LastOrDefault()!.Index;
                    var high = listRange.LastOrDefault(x => x.High == h);
                    var low = listRange.LastOrDefault(x => x.Low == l);
                    var hi = (lastI - high!.Index);
                    var li = (lastI - low!.Index);

                    var rsis = axRsi.Calculate();
                    var r0 = rsis[^1].Rsi;
                    //var r1 = rsis[^2].Rsi;


                    if (body * 5 < us + ds && div < (decimal)1.2 && r0 is > 65 or < 35)
                        TelegramUtil.SendToTelegram($"#{symbol} closed with #HADOJI at : {rsis[^1]!.Close.ToString("n" + symbol0!.Decimals)} #RSI: {r0:n2} H:{hi} L:{li} at {DateTime.Now:dd MMM HH:mm:ss}");
                }
            }
        }

        private string _lastWork = "";
        private async void DoWork(object state)
        {
            var now = DateTime.Now;
            if (_last.Minute < 30 && now.Minute == 30 && _lastWork != now.ToString("ddHHmm"))
            {
                _lastWork = now.ToString("ddHHmm");
                await GetInitData(CancellationToken.None, true);
            }
            _last = now;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }



    public class AxBs
    {
        public string Symbol { get; set; }
        public int Tf { get; set; }
        public List<AxBsCandle> Candles { get; set; }
    }
    public class AxBsCandle
    {
        public decimal Open { get; set; }
        public decimal HaOpen { get; set; }
        public decimal High { get; set; }
        public decimal HaHigh { get; set; }
        public decimal Low { get; set; }
        public decimal HaLow { get; set; }
        public decimal Close { get; set; }
        public decimal HaClose { get; set; }
        public DateTime Date { get; set; }
        public int Index { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Common.Utilities;
using Microsoft.Extensions.Hosting;

namespace API.Hubs
{
    public class TimedHostedSymbolService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly BinanceClient _client;
        public TimedHostedSymbolService()
        {
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

            foreach (var symbol in list)
            {
                var data = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, KlineInterval.OneHour, null, null,
                    50, stoppingToken);
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
                                Open = candle.OpenPrice,
                                HaOpen = candle.OpenPrice,
                                High = candle.HighPrice,
                                HaHigh = candle.HighPrice,
                                Low = candle.LowPrice,
                                HaLow = candle.LowPrice,
                                Close = candle.ClosePrice,
                                HaClose = candle.ClosePrice
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
                                HaClose = (candle.ClosePrice + candle.OpenPrice + candle.LowPrice + candle.HighPrice) / 4
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

                    if (body * 5 < us + ds && div < (decimal)1.2)
                        TelegramUtil.SendToTelegram($"#{symbol} in #60m closed with #HADoji at {DateTime.Now:dd MMM HH:mm:ss}");
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

    }
}

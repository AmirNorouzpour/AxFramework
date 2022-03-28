﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Hosting;
using API.Models;
using Common;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace API.Hubs
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IMemoryCache _memoryCache;

        public TimedHostedService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(120));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var db = new SqlConnection("Data Source=.;Database=AxTraderDb;User Id=sa;Password=Amir1993;MultipleActiveResultSets=true"))
            {
                var symbols = db.Query<Symbol>("select * from Symbols").ToList();
                _memoryCache.Set(CacheKeys.SymbolsData, symbols);
            }

            CreateGlobalResult();
            GetNews();
        }


        private void GetNews()
        {
            try
            {
                var webClient = new WebClient();
                var result = webClient.DownloadString("https://cointelegraph.com/rss");
                var document = XDocument.Parse(result);
                var list = (from descendant in document.Descendants("item")
                            select new Item
                            {
                                Title = descendant.Element("title")!.Value,
                                Category = string.Join(",", descendant.Elements("category").Select(x => x.Value)),
                                Link = descendant.Element("link")!.Value,
                                PublicationDate = DateTime.Parse(descendant.Element("pubDate")!.Value!).ToUniversalTime(),
                                ImageLink = descendant.Element("enclosure")!.Attribute("url")!.Value,
                            }).ToList();
                _memoryCache.Set(CacheKeys.NewsData, list);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void CreateGlobalResult()
        {
            try
            {
                var symbols = _memoryCache.Get<List<Symbol>>(CacheKeys.SymbolsData);
                var btc = symbols.FirstOrDefault(x => x.Title == "BTCUSDT");
                var eth = symbols.FirstOrDefault(x => x.Title == "ETHUSDT");

                var result = new GlobalResult();
                var globalFgi = GetGlobal();
                var globalSymbol = GetGlobalSymbol("bitcoin");
                var globalEthSymbol = GetGlobalSymbol("ethereum");
                result.LastUpdated = globalFgi.last_updated;
                result.TotalMarketCap = globalFgi.total_market_cap_usd.ToString("n0") ;
                result.TotalVolume24h = globalFgi.total_24h_volume_usd.ToString("n0") ;
                result.BTCPercent24h = decimal.Parse(globalSymbol.percent_change_24h).ToString("n2") + "%";
                result.ETHPercent24h = decimal.Parse(globalEthSymbol.percent_change_24h).ToString("n2") + "%";
                result.BTCPrice = btc?.Price != null ? btc.Price.GetValueOrDefault().ToString("n2") : decimal.Parse(globalSymbol.price_usd).ToString("n2");
                result.ETHPrice = eth?.Price != null ? eth.Price.GetValueOrDefault().ToString("n2") : decimal.Parse(globalEthSymbol.price_usd).ToString("n2");
                result.Fng = Fng();
                result.Dom = GetDom()?.ToString("n2") + @"%";
                _memoryCache.Set(CacheKeys.MainData, result);
            }
            catch (Exception)
            {
                // ignored
            }
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

        private GlobalFGI GetGlobal()
        {
            using var wc = new WebClient();
            var contents = wc.DownloadString("https://api.alternative.me/v1/global/");
            var res = JsonConvert.DeserializeObject<GlobalFGI>(contents);
            return res;
        }

        private GlobalSymbol GetGlobalSymbol(string symbol)
        {
            using var wc = new WebClient();
            var contents = wc.DownloadString("https://api.alternative.me/v1/ticker/" + symbol);
            var res = JsonConvert.DeserializeObject<List<GlobalSymbol>>(contents);
            return res?.FirstOrDefault();
        }

        private List<FGModelRes> Fng()
        {
            using var wc = new WebClient();
            var contents = wc.DownloadString("https://api.alternative.me/fng/?limit=31&date_format=kr");
            var res = JsonConvert.DeserializeObject<FGModel>(contents);
            var list = new List<FGModelRes>();
            var now = res!.data.FirstOrDefault();
            list.Add(new FGModelRes { date = "Now", value = now!.value, name = now.value_classification });
            var yesterday = res.data.Skip(1).FirstOrDefault();
            list.Add(new FGModelRes { date = "Yesterday", value = yesterday!.value, name = yesterday.value_classification });
            var week = res.data.Skip(7).FirstOrDefault();
            list.Add(new FGModelRes { date = "Last Week", value = week!.value, name = week.value_classification });
            var month = res.data.Skip(30).FirstOrDefault();
            list.Add(new FGModelRes { date = "Last Month", value = month!.value, name = month.value_classification });
            return list;
        }

        private decimal? GetDom()
        {
            var url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=250&page=1&sparkline=false";
            using var wc = new WebClient();
            var contents = wc.DownloadString(url);
            var data = JsonConvert.DeserializeObject<List<DomModel>>(contents);

            decimal btcCap = 0;
            decimal altCap = 0;

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.id == "bitcoin")
                    {
                        btcCap = item.market_cap;
                        altCap += item.market_cap;
                    }
                    else
                    {
                        altCap += item.market_cap;
                    }
                }
                var dom = btcCap / altCap * 100;
                return dom;
            }

            return null;
        }


    }
}
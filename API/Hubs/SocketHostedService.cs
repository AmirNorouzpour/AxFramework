using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Binance.Net.Clients;
using Common;
using Data.Repositories;
using Entities.MasterSignal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace API.Hubs
{
    public class SocketHostedService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBaseRepository<AxPosition> _repository;
        private readonly BinanceSocketClient _socketClient;
        public SocketHostedService(IMemoryCache memoryCache, IBaseRepository<AxPosition> repository)
        {
            _memoryCache = memoryCache;
            _repository = repository;
            _socketClient = new BinanceSocketClient();
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            var opens = _repository.GetAll(x => x.Status == PositionStatus.Started || x.Status == PositionStatus.NotStarted).ToList();
            _memoryCache.Set(CacheKeys.PositionsData, opens);
            await _socketClient.UsdFuturesStreams.SubscribeToAllMarkPriceUpdatesAsync(3000, msg =>
            {
                var items = msg.Data.ToList();
                foreach (var position in opens)
                {
                    var item = items.FirstOrDefault(x => x.Symbol == position.Symbol);
                    if (item == null)
                        continue;

                    var symbols = _memoryCache.Get<List<Symbol>>(CacheKeys.SymbolsData);
                    var symbol = symbols.FirstOrDefault(x => x.Title == item.Symbol);
                    symbol!.Price = item.MarkPrice;

                    var needUpdate = position?.SetPrice(item.MarkPrice);
                    if ((DateTime.Now - position?.LastUpdate).GetValueOrDefault().TotalSeconds > 60 || needUpdate.GetValueOrDefault())
                    {
                        if (position != null)
                        {
                            _repository.Update(position);
                            position.LastUpdate = DateTime.Now;
                        }
                    }
                }
            }, stoppingToken);
        }



        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }



    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace API.Hubs
{
    public class SocketHostedService : IHostedService, IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Binance.Net.Clients.BinanceSocketClient _socketClient;
        public SocketHostedService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _socketClient = new BinanceSocketClient();
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await _socketClient.UsdFuturesStreams.SubscribeToAllMarkPriceUpdatesAsync(3000, msg =>
                 {
                     var items = msg.Data;
                     foreach (var item in items)
                     {
                         
                     }
                     var a = 0;
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

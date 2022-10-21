using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Hubs;

namespace Services.Services
{
    public class KlineSocketHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<KlineSocketHostedService> _logger;
        private Timer _timer;
        private readonly IUserConnectionService _connectionService;
        private readonly IHubContext<AxHub> _hub;

        public KlineSocketHostedService(ILogger<KlineSocketHostedService> logger, IUserConnectionService userConnectionRepository, IHubContext<AxHub> hub)
        {
            _logger = logger;
            _connectionService = userConnectionRepository;
            _hub = hub;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            DoWork(null);
            return Task.CompletedTask;
        }



        private async void DoWork(object state)
        {

            //var socketClient = new BinanceSocketClient();
            //await socketClient.UsdFuturesStreams.SubscribeToKlineUpdatesAsync("ETHUSDT", KlineInterval.OneMinute, async e =>
            // {
            //     var connections = _connectionService.GetActiveConnections();
            //     var data = e.Data.Data;
            //     var o = new KlineUpdate { Close = data.ClosePrice, Symbol = e.Data.Symbol, Final = data.Final, Open = data.OpenPrice, High = data.HighPrice, Low = data.LowPrice };
            //     await _hub.Clients.Clients(connections).SendAsync("KlineUpdate", o);
            // });
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public class KlineUpdate
    {
        public decimal Close { get; set; }
        public string Symbol { get; set; }
        public bool Final { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
    }
}

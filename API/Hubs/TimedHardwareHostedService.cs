//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using Data.Repositories;
//using Entities.Framework;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Hosting;
//using System.Linq;
//using API.Models;
//using AutoMapper.QueryableExtensions;
//using Common.Utilities;
//using Entities.Framework.AxCharts;
//using Services.Services;

//namespace API.Hubs
//{
//    public class TimedHardwareHostedService : IHostedService, IDisposable
//    {
//        private readonly IBaseRepository<HardwareDataHistory> _repository;
//        private readonly IUserConnectionService _userConnectionService;
//        private readonly IBaseRepository<LineChart> _lineRepository;
//        private readonly IHubContext<AxHub> _hub;
//        private Timer _timer;

//        public TimedHardwareHostedService(IBaseRepository<HardwareDataHistory> repository, IUserConnectionService userConnectionService, IBaseRepository<LineChart> lineRepository, IHubContext<AxHub> hub)
//        {
//            _repository = repository;
//            _userConnectionService = userConnectionService;
//            _lineRepository = lineRepository;
//            _hub = hub;
//        }

//        public Task StartAsync(CancellationToken stoppingToken)
//        {
//            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(300));
//            return Task.CompletedTask;
//        }

//        private void DoWork(object state)
//        {
//            var info = new ProcessStartInfo
//            {
//                FileName = "wmic",
//                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
//                RedirectStandardOutput = true
//            };
//            string output;
//            using (var process = Process.Start(info))
//            {
//                output = process?.StandardOutput.ReadToEnd();
//            }

//            if (output != null)
//            {
//                var lines = output.Trim().Split("\n");
//                var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
//                var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

//                var total = Math.Round(float.Parse(totalMemoryParts[1]) / 1024, 0);
//                var free = Math.Round(float.Parse(freeMemoryParts[1]) / 1024, 0);
//                var used = total - free;
//                var ram = (float)(used * 100 / total);

//                var cpuTask = GetCpuUsageForProcess();
//                var cpu = (float)cpuTask.Result;
//                if (cpu < 0.1)
//                    cpu = 1.00f;
//                _repository.Add(new HardwareDataHistory { InsertDateTime = DateTime.Now, Ram = (float)decimal.Round((decimal)ram, 2), CreatorUserId = 1, Cpu = (float)decimal.Round((decimal)cpu, 2) });

//                var connections = _userConnectionService.GetActiveConnections();
//                var lineChart = _lineRepository.GetAll(x => x.AxChartId == 10).ProjectTo<LineChartDto>().FirstOrDefault();
//                var data = _repository.GetAll(x => x.InsertDateTime >= DateTime.Now.AddHours(-2))
//                    .OrderBy(x => x.InsertDateTime).ToList();
//                if (lineChart != null)
//                {
//                    lineChart.Series = new List<AxSeriesDto>();
//                    for (var i = 0; i < 2; i++)
//                    {
//                        if (i == 0)
//                        {
//                            lineChart.Series.Add(new AxSeriesDto { Name = "RAM", Id = i });
//                            lineChart.Series[i].Data = data.Select(c => c.Ram);
//                        }
//                        if (i == 1)
//                        {
//                            lineChart.Series.Add(new AxSeriesDto { Name = "CPU", Id = i });
//                            lineChart.Series[i].Data = data.Select(c => c.Cpu);
//                        }
//                    }
//                    lineChart.Labels = data.Select(x => x.InsertDateTime.ToPerDateString("HH:mm")).ToList();
//                }
//                _hub.Clients.Clients(connections).SendAsync("UpdateChart", lineChart);
//            }
//        }

//        public Task StopAsync(CancellationToken stoppingToken)
//        {
//            _timer?.Change(Timeout.Infinite, 0);
//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }

//        private async Task<double> GetCpuUsageForProcess()
//        {
//            var startTime = DateTime.UtcNow;
//            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
//            await Task.Delay(500);

//            var endTime = DateTime.UtcNow;
//            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
//            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
//            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
//            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
//            return cpuUsageTotal * 100;
//        }


//    }
//}

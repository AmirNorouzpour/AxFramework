//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Data;
//using Data.Repositories;
//using Entities.Framework;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Pluralize.NET.Core;

//namespace Services.Services
//{
//    public class TimedAuditLogHostedService : IHostedService, IDisposable
//    {
//        private int _executionCount;
//        private readonly ILogger<TimedAuditLogHostedService> _logger;
//        private readonly IBaseRepository<Audit> _auditRepository;
//        private Timer _timer;

//        public TimedAuditLogHostedService(ILogger<TimedAuditLogHostedService> logger, IBaseRepository<Audit> auditRepository)
//        {
//            _logger = logger;
//            _auditRepository = auditRepository;
//        }

//        public Task StartAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("Timed Hosted Service running.");
//            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
//            return Task.CompletedTask;
//        }

//        private void DoWork(object state)
//        {
//            var count = Interlocked.Increment(ref _executionCount);
//            var audits = AuditSingleton.Audits;
//            if (audits.Count > 0)
//            {
//                var notAddAudits = audits.Where(x => x.AuditType != AuditType.Add);
//                _auditRepository.AddRange(notAddAudits);
//                var addAudits = audits.Where(x => x.AuditType == AuditType.Add);
//                foreach (var audit in addAudits)
//                {
//                    var p = new Pluralizer();
//                    var tableName = p.Pluralize(audit.TableName);
//                    using var connection = new SqlConnection(ConnSingleton.Instance.Value);
//                    connection.Open();
//                    using var command = connection.CreateCommand();
//                    command.CommandText = $"select Id from {tableName} where InsertDateTime = '{audit.EntityInsertDateTime:yyyy-MM-dd HH:mm:ss.fffffff}'";
//                    var result = command.ExecuteScalar();
//                    connection.Close();
//                    if (result != null)
//                    {
//                        audit.PrimaryKey = (int) result;
//                        audit.Value = audit.Value.Replace("Id : 0", "Id : " + audit.PrimaryKey);
//                    }

//                    _auditRepository.Add(audit);
//                }
//                AuditSingleton.Audits.Clear();
//            }
//            _logger.LogInformation(
//                "Timed Hosted Service is working. Count: {Count}", count);
//        }

//        public Task StopAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("Timed Hosted Service is stopping.");

//            _timer?.Change(Timeout.Infinite, 0);

//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }
//    }
//}

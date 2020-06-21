using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ResourceMonitor.Services
{
    /// <summary>
    /// 后台同步最新的下载规则
    /// </summary>
    public class SyncRulesHostedService : IHostedService, IDisposable
    {
        private int _executionCount = 0;
        private readonly ILogger<SyncRulesHostedService> _logger;
        private Timer _timer;

        public SyncRulesHostedService(ILogger<SyncRulesHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncRulesHostedService running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(10));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation("SyncRulesHostedService is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncRulesHostedService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
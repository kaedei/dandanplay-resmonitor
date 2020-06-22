using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ResourceMonitor.Models.DandanplayApi;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    /// <summary>
    /// 检查新的资源
    /// </summary>
    public class CheckNewResourcesBackgroundService : BackgroundService
    {
        private readonly IRulesContainer _rulesContainer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckNewResourcesBackgroundService> _logger;

        public CheckNewResourcesBackgroundService(IRulesContainer rulesContainer,
            IConfiguration configuration,
            ILogger<CheckNewResourcesBackgroundService> logger)
        {
            _rulesContainer = rulesContainer;
            _configuration = configuration;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(CheckNewResourcesBackgroundService)} 开始在后台运行");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("开始解析所有规则");
                
                //等待规则同步完毕
                while (_rulesContainer.IsUpdating)
                {
                    _logger.LogInformation("自动下载规则正在同步中，等待同步完毕...");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }

                //按顺序解析每个规则
                var ruleList = _rulesContainer.LocalRules;
                foreach (var rule in ruleList)
                {
                    await DownloadRule(rule);
                }

                //默认每 15 分钟检查一次
                _logger.LogInformation("全部规则解析完毕，等待 15 分钟后再次执行");
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private async Task DownloadRule(AutoDownloadRule rule)
        {
            _logger.LogInformation($"正在解析规则 {rule}");
            //获得需要下载的新资源

            //对比当前已有的下载任务，只下载新的资源

            //解析磁力链至种子文件

            //连接下载器，添加远程任务
        }
    }
}
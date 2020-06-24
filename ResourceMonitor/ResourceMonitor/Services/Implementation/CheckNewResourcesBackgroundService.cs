using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceMonitor.Models.DandanplayApi;
using ResourceMonitor.Models.ResApi;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    /// <summary>
    /// 检查新的资源
    /// </summary>
    public class CheckNewResourcesBackgroundService : BackgroundService
    {
        private readonly IRulesContainer _rulesContainer;
        private readonly IResApi _resApi;
        private readonly IDownloader _downloader;
        private readonly ITorrentService _torrentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckNewResourcesBackgroundService> _logger;
        private readonly int _checkPeriod;

        public CheckNewResourcesBackgroundService(IRulesContainer rulesContainer,
            IResApi resApi, IDownloader downloader, ITorrentService torrentService,
            IConfiguration configuration,
            ILogger<CheckNewResourcesBackgroundService> logger)
        {
            _rulesContainer = rulesContainer;
            _resApi = resApi;
            _downloader = downloader;
            _torrentService = torrentService;
            _configuration = configuration;
            _logger = logger;
            _checkPeriod = int.Parse(configuration["CheckPeriod"]);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"[检查新资源] 开始在后台运行");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("开始解析所有规则");

                //等待规则同步完毕
                while (_rulesContainer.IsUpdating)
                {
                    _logger.LogInformation("自动下载规则正在同步中，等待同步完毕...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

                //按顺序解析每个规则
                var ruleList = _rulesContainer.LocalRules;
                foreach (var rule in ruleList)
                {
                    await DownloadRule(rule);
                }

                //每 _checkPeriod 分钟检查一次
                _logger.LogInformation($"全部规则解析完毕，等待 {_checkPeriod} 分钟后再次执行");
                await Task.Delay(TimeSpan.FromMinutes(_checkPeriod), stoppingToken);
            }

            _logger.LogInformation("[检查新资源] 结束运行");
        }

        private async Task DownloadRule(AutoDownloadRule rule)
        {
            _logger.LogInformation($"正在解析规则 {rule}");
            //获得需要下载的新资源
            ResourceList resourceList;
            try
            {
                resourceList = await _resApi.List(rule.keyword, rule.subgroupId, rule.typeId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"解析规则失败：{rule}");
                return;
            }

            //如果设置了 startTime (只下载指定时间之后的资源)
            var filtered = resourceList.Resources
                .Where(res => rule.startTime == null || res.PublishDateTime >= rule.startTime)
                .ToArray();

            //进一步根据条件过滤
            filtered = FilterResources(filtered, rule.chooseNewerIfDuplicate, rule.limitFileSize, 
                _configuration["LogDetails"] == "true");
            //获取前 n 个结果
            filtered = filtered.Take(rule.maxCount).ToArray();

            foreach (var resourceInfo in filtered)
            {
                try
                {
                    //对比当前已有的下载任务，只下载新的资源
                    var magnet = _torrentService.NormalizeMagnetUrl(resourceInfo.Magnet);
                    var hash = _torrentService.GetHash(magnet);
                    bool exists = await _downloader.IfTaskExists(hash, magnet);
                    if (exists)
                    {
                        _logger.LogDebug($"任务已经存在，跳过创建 {resourceInfo.Title} {hash}");
                        continue;
                    }

                    //解析磁力链至种子文件
                    var torrentBytes = await _torrentService.DownloadTorrent(magnet);

                    if (!_torrentService.IsTorrentFileValid(torrentBytes))
                    {
                        _logger.LogWarning($"解析磁力链得到的种子文件无效 {magnet}");
                        continue;
                    }

                    if (torrentBytes == null || torrentBytes.Length <= 0)
                    {
                        _logger.LogWarning($"解析磁力链失败 {magnet}");
                        continue;
                    }

                    //连接下载器，添加远程任务
                    await _downloader.AddNewTorrentTask(torrentBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"向下载器添加任务失败 {resourceInfo.Title}");
                }
            }
        }

        /// <summary>
        /// 根据规则过滤资源列表
        /// </summary>
        private ResourceInfo[] FilterResources(IEnumerable<ResourceInfo> resources, bool chooseNewerIfDuplicate,
            int limitFileSize, bool logDetails = false)
        {
            if (resources == null) return new ResourceInfo[0];
            var filtered = resources.ToList();
            if (chooseNewerIfDuplicate)
            {
                filtered = filtered.GroupBy(f => f.Title)
                    .Select(g => g.ToArray())
                    .Select(gArr => gArr.OrderByDescending(res => res.PublishDateTime).First())
                    .ToList();
                if (logDetails)
                {
                    foreach (var info in filtered)
                    {
                        _logger.LogDebug($"Filtered By Duplicate. title={info.Title} magnet={info.Magnet}");
                    }
                }

                //处理 v2 v3 的问题
                var duplicated = new List<ResourceInfo>();
                foreach (var resource in filtered)
                {
                    if (duplicated.Contains(resource)) continue;
                    var normalizedTitle =
                        Regex.Replace(resource.Title, @"\W|v3|v2", string.Empty, RegexOptions.IgnoreCase);
                    if (resource.Title.Contains("v3") || resource.Title.Contains("V3"))
                    {
                        var replacements = filtered.Where(res =>
                            res.Title != resource.Title &&
                            normalizedTitle ==
                            Regex.Replace(res.Title, @"\W|v2", string.Empty, RegexOptions.IgnoreCase));
                        duplicated.AddRange(replacements);
                    }
                    else if (resource.Title.Contains("v2") || resource.Title.Contains("V2"))
                    {
                        var replacements = filtered.Where(res =>
                            res.Title != resource.Title &&
                            normalizedTitle == Regex.Replace(res.Title, @"\W", string.Empty, RegexOptions.IgnoreCase));
                        duplicated.AddRange(replacements);
                    }
                }

                filtered = filtered.Except(duplicated).ToList();

                if (logDetails)
                {
                    foreach (var info in filtered)
                    {
                        _logger.LogDebug($"Filtered By V2V3Duplicate. title={info.Title} magnet={info.Magnet}");
                    }
                }
            }

            if (limitFileSize > 0)
            {
                filtered = filtered.Where(res => res.FileSizeInMB < limitFileSize).ToList();
                if (logDetails)
                {
                    foreach (var info in filtered)
                    {
                        _logger.LogDebug(
                            $"Filtered By LimitFileSize {limitFileSize}. title={info.Title} size={info.FileSizeInMB} magnet={info.Magnet}");
                    }
                }
            }


            filtered = filtered.OrderByDescending(res => res.PublishDateTime).ToList();
            if (logDetails)
            {
                foreach (var info in filtered)
                {
                    _logger.LogDebug(
                        $"Filtered By PublishDate. title={info.Title} date={info.PublishDateTime} magnet={info.Magnet}");
                }
            }

            return filtered.ToArray();
        }
    }
}
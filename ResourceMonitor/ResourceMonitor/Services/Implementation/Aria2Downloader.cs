using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AriaNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResourceMonitor.Models.Downloader;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    public class Aria2Downloader : IDownloader
    {
        private readonly ILogger<Aria2Downloader> _logger;
        private readonly AriaManager _manager;

        public Aria2Downloader(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<Aria2Downloader> logger)
        {
            _logger = logger;
            _manager = new AriaManager(configuration["Aria2:Url"],
                configuration["Aria2:Token"],
                httpClientFactory.CreateClient());
        }
        
        public async Task<bool> TryConnect()
        {
            _logger.LogInformation("尝试连接到 Aria2");
            try
            {
                var ver = await _manager.GetVersion();
                _logger.LogInformation(
                    $"连接到 Aria2 成功。版本：{ver.Version}。已启用的特性：{string.Join(", ", ver.EnabledFeatures)}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "连接到 Aria2 失败");
                return false;
            }
        }

        public async Task<List<DownloaderTask>> GetAllTasks()
        {
            _logger.LogDebug("获取 Aria2 所有任务的状态");
            var status = await _manager.GetAllStatus();
            return status.Select(s => new DownloaderTask
            {
                Id = s.InfoHash,
                Magnet = s.InfoHash
            }).ToList();
        }

        public async Task<bool> IfTaskExists(string id, string url)
        {
            var allTasks = await GetAllTasks();
            return allTasks.Any(task => task.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task AddNewTorrentTask(byte[] torrentBytes)
        {
            var gid = await _manager.AddTorrent(torrentBytes);
            await _manager.SaveSession();
            _logger.LogInformation($"向 Aria2 添加任务成功。gid={gid}");
        }
    }
}
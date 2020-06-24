using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResourceMonitor.Models.Downloader;
using ResourceMonitor.Services.Declaration;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;

namespace ResourceMonitor.Services.Implementation
{
    public class TransmissionDownloader : IDownloader
    {
        private readonly ILogger<TransmissionDownloader> _logger;
        private readonly Client _client;


        public TransmissionDownloader(IConfiguration configuration, ILogger<TransmissionDownloader> logger)
        {
            _logger = logger;
            _client = new Client(
                configuration["Transmission:Url"],
                Guid.NewGuid().ToString("N"),
                configuration["Transmission:Login"],
                configuration["Transmission:Password"]);
        }

        public async Task<bool> TryConnect()
        {
            _logger.LogInformation("尝试连接到 Transmission");
            try
            {
                var info = await _client.GetSessionInformationAsync();
                _logger.LogInformation($"连接到 Transmission 成功。版本 {info.Version}。RPC版本 {info.RpcVersion}。");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "连接到 Transmission 失败");
                return false;
            }
        }

        public async Task<List<DownloaderTask>> GetAllTasks()
        {
            _logger.LogDebug("获取 Transmission 所有任务的状态");
            var tasks = await _client.TorrentGetAsync(TorrentFields.ALL_FIELDS);
            return tasks?.Torrents?.Select(t => new DownloaderTask
            {
                Id = t.HashString,
                Magnet = t.MagnetLink,
                Name = t.Name
            }).ToList() ?? new List<DownloaderTask>(0);
        }

        public async Task<bool> IfTaskExists(string id, string url)
        {
            var allTasks = await GetAllTasks();
            return allTasks.Any(t => t.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task AddNewTorrentTask(byte[] torrentBytes)
        {
            var newTorrentInfo = await _client.TorrentAddAsync(new NewTorrent
            {
                Metainfo = Convert.ToBase64String(torrentBytes),
                Paused = false
            });
            if (newTorrentInfo == null || newTorrentInfo.ID == 0)
            {
                throw new Exception("在 Transmission 上建立任务失败");
            }

            _logger.LogInformation($"在 Transmission 上建立任务成功。ID={newTorrentInfo.ID}");
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OctoTorrent;
using OctoTorrent.Common;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    public class TorrentService : ITorrentService
    {
        private readonly IMagnetApi _magnetApi;
        private readonly ILogger<TorrentService> _logger;

        public TorrentService(IMagnetApi magnetApi, ILogger<TorrentService> logger)
        {
            _magnetApi = magnetApi;
            _logger = logger;
        }

        public bool IsTorrentFileValid(byte[] torrentBytes)
        {
            return Torrent.TryLoad(torrentBytes, out var _);
        }

        public string NormalizeMagnetUrl(string magnet)
        {
            return "magnet:?xt=urn:btih:" + GetHash(magnet);
        }

        public string GetHash(string magnet)
        {
            return InfoHash.FromMagnetLink(magnet).ToHex().ToLowerInvariant();
        }

        public async Task<byte[]> DownloadTorrent(string magnet)
        {
            try
            {
                var content = await _magnetApi.ParseMagnet(magnet);
                var torrent = await content.ReadAsByteArrayAsync();
                return torrent;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "下载种子文件时出错");
                return null;
            }
        }
    }
}
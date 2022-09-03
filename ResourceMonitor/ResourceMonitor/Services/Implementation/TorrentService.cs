using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonoTorrent;
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
            var match = Regex.Match(magnet, @"\w{40}", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return InfoHash.FromHex(match.Value).ToHex().ToLowerInvariant();
            }
            match = Regex.Match(magnet, @"\w{32}", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return InfoHash.FromBase32(match.Value).ToHex().ToLowerInvariant();
            }
            throw new ArgumentException(magnet);
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
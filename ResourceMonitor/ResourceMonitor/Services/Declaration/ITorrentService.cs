using System.Threading.Tasks;

namespace ResourceMonitor.Services.Declaration
{
    public interface ITorrentService
    {
        bool IsTorrentFileValid(byte[] torrentBytes);

        string NormalizeMagnetUrl(string magnet);
        
        string GetHash(string magnet);

        Task<byte[]> DownloadTorrent(string magnet);
    }
}
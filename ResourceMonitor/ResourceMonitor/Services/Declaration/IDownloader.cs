using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceMonitor.Models.Downloader;

namespace ResourceMonitor.Services.Declaration
{
    public interface IDownloader
    {
        Task<bool> TryConnect();
        
        Task<List<DownloaderTask>> GetAllTasks();

        Task<bool> IfTaskExists(string id, string url);

        Task<DownloaderTask> AddNewTorrentTask(byte[] torrentBytes);
    }
}
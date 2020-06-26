using ResourceMonitor.Models.DandanplayApi;

namespace ResourceMonitor.Models.Web
{
    public class ServerStatusViewModel
    {
        public string version { get; set; }
        public string downloader { get; set; }
        public AutoDownloadRule[] rules { get; set; }
    }
}
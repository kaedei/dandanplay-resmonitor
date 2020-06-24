namespace ResourceMonitor.Models.Downloader
{
    public class DownloaderTask
    {
        public string Id { get; set; }
        public string Magnet { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Magnet)}: {Magnet}, {nameof(Name)}: {Name}";
        }
    }
}
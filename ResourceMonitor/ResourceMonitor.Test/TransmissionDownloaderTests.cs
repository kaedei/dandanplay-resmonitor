using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceMonitor.Services.Implementation;

namespace ResourceMonitor.Test
{
    [TestClass]
    public class TransmissionDownloaderTests
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Transmission:Url", "http://localhost:9091/transmission/rpc"},
                {"Transmission:Login", "admin"},
                {"Transmission:Password", "admin"}
            })
            .Build();

        [TestMethod]
        public async Task TryConnect()
        {
            var downloader = new TransmissionDownloader(configuration, new NullLogger<TransmissionDownloader>());
            Assert.IsTrue(await downloader.TryConnect());
        }

        [TestMethod]
        public async Task GetAllTasks()
        {
            var downloader = new TransmissionDownloader(configuration, new NullLogger<TransmissionDownloader>());
            var tasks = await downloader.GetAllTasks();
            foreach (var task in tasks)
            {
                Debug.WriteLine(task.ToString());
            }
        }

        [TestMethod]
        public async Task AddNewTorrentTask()
        {
            var downloader = new TransmissionDownloader(configuration, new NullLogger<TransmissionDownloader>());
            var torrentBytes = File.ReadAllBytes("valid.torrent");
            await downloader.AddNewTorrentTask(torrentBytes);
        }
    }
}
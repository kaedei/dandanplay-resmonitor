using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ResourceMonitor.Services.Implementation;

namespace ResourceMonitor.Test
{
    [TestClass]
    public class Aria2DownloaderTests
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Aria2:Url", "http://localhost:6800/jsonrpc"},
                {"Aria2:Token", "token"}
            })
            .Build();
        
        [TestMethod]
        public async Task TryConnect()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(c => c.CreateClient()).Returns(new HttpClient());
            var downloader = new Aria2Downloader(configuration, mockFactory.Object , new NullLogger<Aria2Downloader>());
            var result = await downloader.TryConnect();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AddNewTorrentTask()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(c => c.CreateClient()).Returns(new HttpClient());
            var downloader = new Aria2Downloader(configuration, mockFactory.Object, new NullLogger<Aria2Downloader>());
            var torrentBytes = File.ReadAllBytes("valid.torrent");
            await downloader.AddNewTorrentTask(torrentBytes);
        }
    }
}
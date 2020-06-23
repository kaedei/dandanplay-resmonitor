using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceMonitor.Services.Implementation;

namespace ResourceMonitor.Test
{
    [TestClass]
    public class Aria2DownloaderTests
    {
        [TestMethod]
        public async Task TryConnect()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Aria2:Url", "http://localhost:6800/jsonrpc"},
                    {"Aria2:Token", "token"}
                })
                .Build();
            var downloader = new Aria2Downloader(configuration, new HttpClient(), new NullLogger<Aria2Downloader>());
            var result = await downloader.TryConnect();
            Assert.IsTrue(result);
        }
    }
}
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using ResourceMonitor.Services.Declaration;
using ResourceMonitor.Services.Implementation;

namespace ResourceMonitor.Test
{
    [TestClass]
    public class TorrentServiceTests
    {
        [TestMethod]
        public void IsTorrentFileValid_Pass()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var torrentBytes = File.ReadAllBytes("valid.torrent");
            Assert.IsTrue(service.IsTorrentFileValid(torrentBytes));
        }

        [TestMethod]
        public void IsTorrentFileValid_NotPass()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var torrentBytes = File.ReadAllBytes("valid.torrent");
            //修改一些文件内容
            for (int i = 100; i < 200; i++)
            {
                torrentBytes[i] = 0;
            }
            Assert.IsFalse(service.IsTorrentFileValid(torrentBytes));
        }

        [TestMethod]
        public void NormalizeMagnetUrl_Pass_40()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:08c2c30ca85cf78ed147488a431d9aee50824a7b";
            var after = service.NormalizeMagnetUrl(magnet);
            Assert.AreEqual(magnet, after);
        }
        
        [TestMethod]
        public void NormalizeMagnetUrl_Pass_32()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:BDBMGDFILT3Y5UKHJCFEGHM25ZIIEST3";
            var after = service.NormalizeMagnetUrl(magnet);
            Assert.AreEqual("magnet:?xt=urn:btih:08c2c30ca85cf78ed147488a431d9aee50824a7b", after);
        }
        
        [TestMethod]
        public void NormalizeMagnetUrl_Pass_Tracker()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet =
                "magnet:?xt=urn:btih:4cda49aa1c28db946e89ecb6e18482c8d347b41d&tr=udp://9.rarbg.to:2710/announce&tr=udp://9.rarbg.me:2710/announce&tr=http://tr.cili001.com:8070/announce&tr=http://tracker.trackerfix.com:80/announce&tr=udp://open.demonii.com:1337&tr=udp://tracker.opentrackr.org:1337/announce&tr=udp://p4p.arenabg.com:1337&tr=wss://tracker.openwebtorrent.com&tr=wss://tracker.btorrent.xyz&tr=wss://tracker.fastcast.nz";
            var after = service.NormalizeMagnetUrl(magnet);
            Assert.AreEqual("magnet:?xt=urn:btih:4cda49aa1c28db946e89ecb6e18482c8d347b41d", after);
        }

        [TestMethod]
        public void GetHash_Pass_32()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:BDBMGDFILT3Y5UKHJCFEGHM25ZIIEST3";
            var hash = service.GetHash(magnet);
            Assert.AreEqual("08c2c30ca85cf78ed147488a431d9aee50824a7b", hash);
        }
        
        [TestMethod]
        public void GetHash_Pass_40()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:08c2c30ca85cf78ed147488a431d9aee50824a7b";
            var hash = service.GetHash(magnet);
            Assert.AreEqual("08c2c30ca85cf78ed147488a431d9aee50824a7b", hash);
        }
        
        [TestMethod]
        public void GetHash_NotPass_UpperCase()
        {
            var service = new TorrentService(null, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:08c2c30ca85cf78ed147488a431d9aee50824a7b";
            var hash = service.GetHash(magnet);
            Assert.AreNotEqual("08C2C30CA85CF78ED147488A431D9AEE50824A7B", hash);
        }

        //[TestMethod]
        public async Task DownloadTorrent_Pass_Online()
        {
            var api = RestService.For<IMagnetApi>("https://m2t.chinacloudsites.cn");
            var service = new TorrentService(api, new NullLogger<TorrentService>());
            var magnet = "magnet:?xt=urn:btih:08c2c30ca85cf78ed147488a431d9aee50824a7b";
            var torrentBytes = await service.DownloadTorrent(magnet);
            Assert.IsTrue(service.IsTorrentFileValid(torrentBytes));
        }
    }
}
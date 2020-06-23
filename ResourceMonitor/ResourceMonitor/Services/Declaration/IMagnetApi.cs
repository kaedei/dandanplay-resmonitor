using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace ResourceMonitor.Services.Declaration
{
    public interface IMagnetApi
    {
        [Get("/Magnet/Parse")]
        Task<HttpContent> ParseMagnet(string magnet);
    }
}
using System.Threading.Tasks;
using Refit;
using ResourceMonitor.Models.DandanplayApi;

namespace ResourceMonitor.Services.Declaration
{
    public interface IDandanplayApi
    {
        [Post("/api/v2/login")]
        Task<LoginResponse> Login([Body] LoginRequest request);

        [Get("/api/v2/login/renew")]
        Task<LoginResponse> Renew([Header("Authorization")] string authorization);

        [Post("/api/v2/sync/autodownload")]
        Task<AutoDownloadRuleListResponse> SyncAutoDownloadRules([Body] AutoDownloadRuleSyncRequest request,
            [Header("Authorization")] string authorization);
    }
}
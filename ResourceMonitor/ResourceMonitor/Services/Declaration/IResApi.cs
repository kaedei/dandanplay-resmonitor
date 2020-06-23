using System.Threading.Tasks;
using Refit;
using ResourceMonitor.Models.ResApi;

namespace ResourceMonitor.Services.Declaration
{
    public interface IResApi
    {
        [Get("/list")]
        Task<ResourceList> List(string keyword, int? subgroupId, int? typeId);
    }
}
using System.Threading.Tasks;
using Refit;

namespace ResourceMonitor.Services.Declaration
{
    public interface IResApi
    {
        [Get("/list")]
        Task List(string keyword, int? subgroupId, int? typeId);
    }
}
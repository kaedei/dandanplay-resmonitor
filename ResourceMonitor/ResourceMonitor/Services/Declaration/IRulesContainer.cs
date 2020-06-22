using System.Collections.Immutable;
using ResourceMonitor.Models.DandanplayApi;

namespace ResourceMonitor.Services.Declaration
{
    public interface IRulesContainer
    {
        bool IsUpdating { get; set; }
        ImmutableList<AutoDownloadRule> LocalRules { get; }
        void SyncWithServerRules(AutoDownloadRuleListResponse serverRules);
    }
}
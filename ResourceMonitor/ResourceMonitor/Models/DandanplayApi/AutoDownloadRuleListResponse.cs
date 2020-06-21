namespace ResourceMonitor.Models.DandanplayApi
{
    public class AutoDownloadRuleListResponse : ResponseBase
    {
        public AutoDownloadRule[] rules { get; set; }
        public string[] removedRuleIds { get; set; }
    }
}
namespace ResourceMonitor.Models.DandanplayApi
{
    public class ResponseBase
    {
        public int errorCode { get; set; }
        public bool success { get; set; }
        public string errorMessage { get; set; }
    }
}
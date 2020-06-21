namespace ResourceMonitor.Models.DandanplayApi
{
    public class LoginRequest
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string appId { get; set; }
        public long unixTimestamp { get; set; }
        public string hash { get; set; }
    }
}
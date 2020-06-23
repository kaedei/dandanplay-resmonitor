using Newtonsoft.Json;

namespace AriaNet.Attributes
{
    [JsonObject]
    public class AriaGlobalStatus
    {
        [JsonProperty("downloadSpeed")]
        public int DownloadSpeed { get; set; }
        
        [JsonProperty("numActive")]
        public int ActiveTaskCount { get; set; }
        
        [JsonProperty("numStopped")]
        public int StoppedTaskCount { get; set; }
        
        [JsonProperty("numWaiting")]
        public int WaitingTaskCount { get; set; }
        
        [JsonProperty("uploadSpeed")]
        public int UploadSpeed { get; set; }
    }
}
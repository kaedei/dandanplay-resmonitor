using Newtonsoft.Json;

namespace AriaNet.Attributes
{
    [JsonObject]
    public class AriaSession
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}
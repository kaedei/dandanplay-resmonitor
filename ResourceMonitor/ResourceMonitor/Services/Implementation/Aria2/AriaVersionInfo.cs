using System.Collections.Generic;
using Newtonsoft.Json;

namespace AriaNet.Attributes
{
    [JsonObject]
    public class AriaVersionInfo
    {
        [JsonProperty("enabledFeatures")]
        public List<string> EnabledFeatures { get; set; }
        
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
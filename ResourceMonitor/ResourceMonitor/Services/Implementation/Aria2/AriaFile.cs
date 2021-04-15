using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AriaNet.Attributes
{
    public class AriaFile
    {
        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("completedLength")]
        public string CompletedLength { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("selected")]
        public string Selected { get; set; }

        //Not Implement this now. Ref: https://aria2.github.io/manual/en/html/aria2c.html#aria2.getUris
        //[JsonProperty("uris")]
        //public List<string> Uris { get; set; }
    }
}

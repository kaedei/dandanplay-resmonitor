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

        [JsonProperty("uris")]
        public List<string> Uris { get; set; }
    }
}

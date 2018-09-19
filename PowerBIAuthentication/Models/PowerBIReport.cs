using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBIAuthentication.Models
{
    public class PowerBIReport
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }

        [JsonProperty("embedUrl")]
        public string EmbedUrl { get; set; }

        [JsonProperty("isOwnedByMe")]
        public bool IsOwnedByMe { get; set; }

        [JsonProperty("datasetId")]
        public string DatasetId { get; set; }
    }
}

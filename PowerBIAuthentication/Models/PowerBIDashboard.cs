using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBIAuthentication.Models
{
    public class PowerBIDashboard
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("embedUrl")]
        public string EmbedUrl { get; set; }

        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; set; }
    }
}

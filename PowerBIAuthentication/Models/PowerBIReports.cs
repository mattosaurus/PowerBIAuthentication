using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBIAuthentication.Models
{
    public class PowerBIReports
    {
        [JsonProperty("value")]
        public List<PowerBIReport> Reports { get; set; }
    }
}

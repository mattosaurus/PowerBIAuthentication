using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBIAuthentication.Models
{
    public class PowerBIDashboards
    {
        [JsonProperty("value")]
        public List<PowerBIDashboard> Dashboards { get; set; }
    }
}

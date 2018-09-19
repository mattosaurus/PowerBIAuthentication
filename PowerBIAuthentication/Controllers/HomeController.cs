using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PowerBIAuthentication.Extensions;
using PowerBIAuthentication.Models;

namespace PowerBIAuthentication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ITokenAcquisition _tokenAcquisition;

        public HomeController(IConfiguration config, ITokenAcquisition tokenAcquisition)
        {
            _config = config;
            _tokenAcquisition = tokenAcquisition;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult PowerBi(string dashboardId)
        {
            if (!string.IsNullOrEmpty(dashboardId))
            {
                HttpContext.Session.SetString("DashboardId", dashboardId);
                TempData["DashboardId"] = dashboardId;
            }

            // Retrieve token from cache
            string accessToken = _tokenAcquisition.GetAccessTokenOnBehalfOfUser(HttpContext, User, new string[] { "https://analysis.windows.net/powerbi/api/Dataset.Read.All" }).Result;

            string responseContent = string.Empty;

            //Configure dashboards request
            System.Net.WebRequest request = System.Net.WebRequest.Create(String.Format("{0}/dashboards", _config["PowerBi:ApiUri"].ToString())) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

            PowerBIDashboards PBIDashboards = new PowerBIDashboards();

            //Get dashboards response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();

                    //Deserialize JSON string
                    PBIDashboards = JsonConvert.DeserializeObject<PowerBIDashboards>(responseContent);
                }
            }

            ViewData.Put<PowerBIDashboards>("PBIDashboards", PBIDashboards);
            TempData["AccessToken"] = accessToken;

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

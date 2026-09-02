using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Core.API.Clean.AdditionalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PingController : ControllerBase
    {
        public PingController()
        {
        }

        [HttpGet("Environment")]
        public JsonResult GetEnvironment()
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "ProcessName", Process.GetCurrentProcess().ProcessName },
                { "HostTime", DateTime.Now.ToString() },
                { "ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "UNRESOLVED" }
            };

            //string value = JsonConvert.SerializeObject(keyValuePairs);

            //Log.Information(value);

            return new JsonResult(keyValuePairs);
        }
    }
}
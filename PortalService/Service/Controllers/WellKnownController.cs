using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Beloning.Services.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WellKnownController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public WellKnownController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult GetWellKnown()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var wk = System.IO.File.ReadAllText(contentRootPath + "/STS/well-known-openid-configuration.json");
            JToken token = JToken.Parse(wk);
            JObject json = JObject.Parse(token.ToString());
            return Ok(json);
        }

        [HttpGet("Keys")]
        public IActionResult WellKnownKeys()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var keys = System.IO.File.ReadAllText(contentRootPath + "/STS/keys.json");
            JToken token = JToken.Parse(keys);
            JObject json = JObject.Parse(token.ToString());
            return Ok(json);
        }

    }
}

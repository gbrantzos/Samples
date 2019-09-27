using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebAppCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> logger;
        private readonly IOptionsSnapshot<SasConnection> options;

        public ValuesController(ILogger<ValuesController> logger, IOptionsSnapshot<SasConnection> options)
        {
            this.logger = logger;
            var aa = options.Get("Pharmex");

            logger.LogInformation("Pharmex host {host}:{port}", aa.Host, aa.Port);
            logger.LogInformation("Default host {host}:{port}", options.Value.Host, options.Value.Port);

            logger.LogInformation("GbWorks host {host}:{port}", options.Get("GbWorks").Host, options.Get("GbWorks").Port);
            logger.LogInformation("Kafea host {host}:{port}", options.Get("Kafea").Host, options.Get("Kafea").Port);

        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            logger.LogTrace("Trace");
            logger.LogDebug("Debug");
            logger.LogInformation("Info");
            logger.LogWarning("Warn");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

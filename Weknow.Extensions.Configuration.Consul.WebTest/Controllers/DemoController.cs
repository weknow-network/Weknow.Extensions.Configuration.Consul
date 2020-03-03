using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Weknow.Extensions.Configuration.Consul;

namespace ConsulConfigurtionProviderSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<DemoSetting> _optionsSnapshot1;

        public DemoController(
            ILogger<DemoController> logger,
            IOptionsSnapshot<DemoSetting> optionsSnapshot1)
        {
            _logger = logger;
            _optionsSnapshot1 = optionsSnapshot1;
        }

        // GET api/values  
        [HttpGet]
        public async Task<string> GetAsync()
        {
            await Task.Delay(400);
            return $@"IOptionsSnapshot: {_optionsSnapshot1.Value.Key}" ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Weknow.Extensions.Configuration.Consul.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<DemoSetting> _setting;

        public TestController(
            ILogger<TestController> logger,
            IOptionsSnapshot<DemoSetting> setting)
        {
            _logger = logger;
            _setting = setting;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            await Task.Delay(300);
            return _setting.Value.Key;
        }
    }
}

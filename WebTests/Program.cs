using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsulConfigurtionProviderSample
{
    public static class Program
    {
        private const string CONSUL_URL_KEY = "consul-url";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string consulUrl = args?.Where(m => m.StartsWith(CONSUL_URL_KEY))
                                    ?.Select(m => m.Substring(CONSUL_URL_KEY.Length + 1))
                                    ?.FirstOrDefault() ?? 
                                    Environment.GetEnvironmentVariable(CONSUL_URL_KEY) ??
                                    "http://127.0.0.1:8500";

            return Host.CreateDefaultBuilder(args)
                .AddConsulConfiguration()
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
        }
    }
}

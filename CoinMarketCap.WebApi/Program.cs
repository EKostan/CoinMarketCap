using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CoinMarketCap.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseKestrel(o =>
                {
                    o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(180);
                    o.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(180);
                    //o.Limits.MaxRequestBodySize = null;
                })
                .Build();
    }
}

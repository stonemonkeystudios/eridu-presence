
using Microsoft.AspNetCore.Server.Kestrel.Core;
using HQDotNet.Presence;

namespace Eridu.Presence {
    class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder
                        .UseKestrel(options => {
                            // WORKAROUND: Accept HTTP/2 only to allow insecure HTTP/2 connections during development.
                            options.ListenAnyIP(5003, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                            options.AllowAlternateSchemes = true;
                        })
                        .UseStartup<Startup>();
                });
    }
}
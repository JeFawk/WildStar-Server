// Copyright (c) Arctium.

using System;
using System.IO;
using System.Net;
using Arctium.API.Misc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Arctium.API
{
    public class APIServer
    {
        public static void Main(string[] args)
        {
            // Initialize the API server configuration file.
            ApiConfig.Initialize("configs/API.conf");

            // Switch to HTTP 2.
            if (ApiConfig.Protocol == HttpProtocols.Http2)
                AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Server.Kestrel.Experimental.Http2", isEnabled: true);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Parse(ApiConfig.BindHost), ApiConfig.BindPort, listenOptions =>
                    {
                        // Set the HTTP protocol.
                        // Default: Http1
                        listenOptions.Protocols = ApiConfig.Protocol;

                        // Enable Https if enabled in config & the given certificate exists.
                        if (ApiConfig.Tls && File.Exists(ApiConfig.TlsCertificate))
                            listenOptions.UseHttps(ApiConfig.TlsCertificate);
                    });
                })
                .Build();
        }
    }
}

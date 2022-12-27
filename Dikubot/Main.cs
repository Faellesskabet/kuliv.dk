using System;
using System.Reflection;
using Dikubot.Webapp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dikubot;

public class main
{
    public static bool IS_DEV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    public static void Main(string[] args)
    {
        //We start the webserver in this thread
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                // Add other providers for JSON, etc.

                if (hostContext.HostingEnvironment.IsDevelopment()) builder.AddUserSecrets<main>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                //this is simply finding the file location of our webserver files
                string location = Assembly.GetEntryAssembly().Location;
                location = location.Substring(0, location.IndexOf("bin")) + "Webapp";

                webBuilder.UseContentRoot(location);
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();
                webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
            });
    }
}
using dikubot.discord;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dikubot
{
    public class Program
    {
        public static Thread discordThread;
        public static bool IS_DEV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("DEVELOPMENT");
        public static void Main(string[] args)
        {
            DiscordBot discordBot = new DiscordBot();
            discordThread = new Thread(new ThreadStart(discordBot.run));
            discordThread.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var p = System.Reflection.Assembly.GetEntryAssembly().Location;
                    p = p.Substring(0, p.IndexOf("bin")) + "webapp";

                    webBuilder.UseContentRoot(p);
                    webBuilder.UseStartup<Startup>();
                });
    }
}

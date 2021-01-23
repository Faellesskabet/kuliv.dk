using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Channels;
using Dikubot.Database.Models;
using Dikubot.Webapp;
using Dikubot.Discord;
using Microsoft.Extensions.Logging;

namespace Dikubot
{
    public class main
    {
        public readonly static DiscordBot DiscordBot = new DiscordBot();
        public readonly static Thread DiscordThread = new Thread(new ThreadStart(DiscordBot.run));
        public static bool IS_DEV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development");
        public static void Main(string[] args)
        {
            DiscordThread.Start();
            
            //UserModel x = new UserModel();
            //x.Email = "test";
            //x.Name = "test";
            //new UserServices().Create(x);
            
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var p = System.Reflection.Assembly.GetEntryAssembly().Location;
                    p = p.Substring(0, p.IndexOf("bin")) + "Webapp";

                    webBuilder.UseContentRoot(p);
                    webBuilder.UseStartup<Startup>();
                });
    }
}

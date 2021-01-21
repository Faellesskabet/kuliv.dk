using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Channels;
using Dikubot.Database.Models;
using Dikubot.Webapp;
using Dikubot.Discord;

namespace Dikubot
{
    public class main
    {
        public static Thread discordThread;
        public static bool IS_DEV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("DEVELOPMENT");
        public static void Main(string[] args)
        {
            DiscordBot discordBot = new DiscordBot();
            discordThread = new Thread(new ThreadStart(discordBot.run));
            discordThread.Start();

            UserModel x = new UserModel();
            x.Email = "test";
            x.Major = "test";
            new UserServices().Create(x);
            
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var p = System.Reflection.Assembly.GetEntryAssembly().Location;
                    p = p.Substring(0, p.IndexOf("bin")) + "Webapp";

                    webBuilder.UseContentRoot(p);
                    webBuilder.UseStartup<Startup>();
                });
    }
}

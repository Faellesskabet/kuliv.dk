using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Blazored.LocalStorage;
using Data;
using Dikubot.DataLayer.Caching;
using Dikubot.DataLayer.Cronjob.Cronjobs;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.Discord;
using Dikubot.Webapp.Authentication;
using Dikubot.Webapp.Authentication.Discord.OAuth2;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;
using Syncfusion.Blazor;


namespace Dikubot.Webapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            var initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

            services.AddScoped<NotifyStateService>();
            services.AddSingleton<Cache<MessageModel, IMessage>>();
            services.AddSingleton<CacheNewsMessagesTask>();
            
            services.AddSingleton<DiscordBot>();
            
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/webapp/Pages");
           
            services.AddServerSideBlazor(options =>
            {
                options.DetailedErrors = true;
                options.DisconnectedCircuitMaxRetained = 100;
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
                options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
                options.MaxBufferedUnacknowledgedRenderBatches = 10;
            });
            
            services.AddServerSideBlazor()
                .AddHubOptions(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    options.EnableDetailedErrors = true;
                    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                    options.MaximumParallelInvocationsPerClient = 1;
                    options.MaximumReceiveMessageSize = 32 * 1024;
                    options.StreamBufferCapacity = 10;
                });
            
            
            //Do NICE STUFF - with login :D
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMudServices();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //Kalender
            services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            
            services.AddResponseCaching();
            services.AddRouting();
            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);
            
            // Authentication
            services.AddScoped<AuthenticationStateProvider, Authenticator>();
            services.AddScoped<UserService>();
            services.AddScoped<BrowserService>();
            services.AddScoped<MetadataService>();

            services.AddAuthentication(options =>
                {
                    ///CookieAuthenticationDefaults.AuthenticationScheme
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/error";
                    
                    
                }).AddDiscord(options =>
                {
                    options.AccessDeniedPath = "/error";
                    options.ClientId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
                    options.Scope.Add("identify guilds guilds.join");
                    options.SaveTokens = true;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetService<DiscordBot>().Run();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            //Kalendar
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Environment.GetEnvironmentVariable("SYNCFUSION_API_KEY"));
            
            
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
using System;
using Blazored.LocalStorage;
using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.Settings.Tags;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Events;
using Dikubot.DataLayer.Database.Guild.Models.Channel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Database.Guild.Models.Course;
using Dikubot.DataLayer.Database.Guild.Models.Equipment;
using Dikubot.DataLayer.Database.Guild.Models.Group;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Database.Guild.Models.JoinRole;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord;
using Dikubot.Webapp.Authentication;
using Dikubot.Webapp.Data;
using Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Syncfusion.Blazor;

namespace Dikubot
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

            services.AddSingleton<DiscordBot>();

            // Mongo database
            services.AddSingleton<DatabaseService>();
            
            // Global mongo services
            services.AddSingleton<UserGlobalMongoService>();
            services.AddSingleton<TagMongoService>();
            services.AddSingleton<SessionMongoService>();
            services.AddSingleton<GuildSettingsMongoService>();
            
            // User Service
            services.AddSingleton<UserService>();
            
            // Guild mongo services
            services.AddSingleton<UserGuildMongoService>();
            services.AddSingleton<RoleMongoService>();
            services.AddSingleton<JoinRoleMongoService>();
            services.AddSingleton<GuildMongoService>();
            services.AddSingleton<EducationMongoService>();
            services.AddSingleton<CourseMongoService>();
            services.AddSingleton<TextChannelMongoService>();
            services.AddSingleton<VoiceChannelMongoService>();
            services.AddSingleton<CalendarMongoService>();
            services.AddSingleton<EventsMongoService>();
            services.AddSingleton<EquipmentMongoService>();

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

            services.AddMudServices();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //Kalender
            services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            
            services.AddResponseCaching();
            services.AddRouting();
            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);
            services.AddScoped<AuthenticationStateProvider, Authenticator>();
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
                }).AddDiscord(options =>
                {
                    options.ClientId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
                    options.Scope.Add("identify guilds guilds.join");
                    options.SaveTokens = true;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Data;
using Dikubot.DataLayer.Caching;
using Dikubot.DataLayer.Cronjob;
using Dikubot.DataLayer.Cronjob.Cronjobs;
using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Global.Calendar;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.DataLayer.Database.Global.Event;
using Dikubot.DataLayer.Database.Global.Facebook;
using Dikubot.DataLayer.Database.Global.GuildCalendars;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.Tags;
using Dikubot.DataLayer.Database.Global.Union;
using Dikubot.DataLayer.Database.Global.Request;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel.Messages;
using Dikubot.DataLayer.Permissions;
using Dikubot.Discord;
using Dikubot.Discord.EventListeners;
using Dikubot.Discord.EventListeners.Permissions;
using Dikubot.Webapp.Authentication;
using Dikubot.Webapp.Data;
using Dikubot.Webapp.Extensions.Discovery.Links;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using WayfJwtConnector;

namespace Dikubot.Webapp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

        // DISCORD SERVICES
        DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 1000,
            GatewayIntents = GatewayIntents.All
        };

        services.AddSingleton(discordSocketConfig).AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>();
        services.AddSingleton<DiscordBot>();
        services.AddSingleton<PermissionListeners>();
        services.AddSingleton<ExpandableVoiceChatListener>();
        services.AddSingleton<GreetingListener>();
        services.AddSingleton<GuildDownloadListeners>();
        services.AddSingleton<MessageListener>();

        // DATABASE SERVICES
        services.AddSingleton<Database>();
        services.AddSingleton<IGuildMongoFactory, GuildMongoFactory>();
        services.AddSingleton<DiscordUserGlobalMongoService>();
        services.AddSingleton<SessionMongoService>();
        services.AddSingleton<GuildSettingsMongoService>();
        services.AddSingleton<UnionMongoService>();
        services.AddSingleton<UnionRequestMongoService>();
        services.AddSingleton<TagMongoService>();
        services.AddSingleton<GuildCalendarMongoService>();
        services.AddSingleton<EventsMongoService>();
        services.AddSingleton<EventRequestMongoService>();
        services.AddSingleton<CalendarMongoService>();
        services.AddSingleton<FacebookPageService>();
        services.AddSingleton<FacebookRequestMongoService>();
        

        services.AddSingleton<IPermissionServiceFactory, PermissionServiceFactory>();

        // CRONJOBS
        services.AddSingleton<Scheduler>();
        services.AddSingleton<BackupDatabaseTask>();
        services.AddSingleton<ForceNameChangeTask>();
        services.AddSingleton<UpdateVerifiedTask>();
        services.AddSingleton<CacheNewsMessagesTask>();
        services.AddSingleton<CronJobService>();

        // CACHE
        services.AddSingleton<Cache<MessageModel, IMessage>>();

        // WEB SERVICES
        services.AddScoped<NotifyStateService>();
        services.AddScoped<LocalizationService>();
        services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/webapp/Pages");
        services.AddScoped<FacebookService>();
        services.AddScoped<JsonService>();
        services.AddSingleton<EmojiService>();
        
        
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


        services.AddHttpContextAccessor();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddMudServices();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });

        services.AddResponseCaching();
        services.AddRouting();
        services.AddBlazoredLocalStorage(config =>
            config.JsonSerializerOptions.WriteIndented = true);

        services.AddScoped<AuthenticationStateProvider, Authenticator>();
        services.AddScoped<UserService>();
        services.AddScoped<BrowserService>();
        services.AddScoped<MetadataService>();
        
        services.AddWayf(Configuration.GetSection("Wayf"));
        services.AddAuthentication(options =>
            {
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
        app.ApplicationServices.GetService<DiscordSocketClient>().Ready += () =>
        {
            app.ApplicationServices.GetService<CronJobService>().Schedule();
            return Task.CompletedTask;
        };
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
        SyncfusionLicenseProvider.RegisterLicense(Environment.GetEnvironmentVariable("SYNCFUSION_API_KEY"));


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
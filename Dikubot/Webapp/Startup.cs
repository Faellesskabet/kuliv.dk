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
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorLoginDiscord.Data;
using Dikubot.Webapp.Authentication;
using Discord.OAuth2;
using Microsoft.AspNetCore.Http;
using MudBlazor.Services;

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
            
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/webapp/Pages");
           
            
            
            //Do NICE STUFF - with login :D
            services.AddHttpContextAccessor();
            
            services.AddMudServices();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddResponseCaching();
            services.AddRouting();
            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);
            services.AddScoped<AuthenticationStateProvider, Authenticator>();

            
            //AddAuthentication
            services.AddSingleton<UserService>();
            
            services.AddAuthentication(options =>
            {
                ///CookieAuthenticationDefaults.AuthenticationScheme
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordDefaults.AuthenticationScheme;
            })
                .AddCookie().
                AddDiscord("Discord",options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID");
                options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
                options.Scope.Add("identify guilds guilds.join");
                options.Prompt = "";
                options.SaveTokens = true;
                options.Events.OnCreatingTicket = ctx =>
                {
                    /*ctx.Identity.AddClaim(new Claim("Discord:CurrentGuild:ID",
                        Environment.GetEnvironmentVariable("OPTIONS_MAIN_GUILD_ID") ?? "string.Empty"));
                    ctx.Identity.AddClaim(new Claim("User:Verify",true.ToString(),typeof(bool).ToString(),"http://kuliv.dk"));*/
                    //var test = ctx.User;
                    /*
                    if(ctx.Identity != null && ctx.Identity.FindFirst(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value.Equals("581865014063792197"))
                        ctx.Identity.AddClaim(new Claim(ClaimTypes.Role,"ADMIN"));
                        */
                    //TODO: giv role claims insted of just inRole().
                    //options.ClaimActions.Add(new  JsonKeyClaimAction(ClaimTypes.Role,ClaimTypes.Role, "ADMIN"));
                    return Task.CompletedTask;
                };
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
            
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            
            
            

            //Cache life span set to 1 second if it's a dev build.
            TimeSpan maxAge = main.IS_DEV ? TimeSpan.FromSeconds(1) : TimeSpan.FromDays(2);

            //We do a bunch of caching here
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = maxAge
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] {"Accept-Encoding"};

                await next();
            });

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
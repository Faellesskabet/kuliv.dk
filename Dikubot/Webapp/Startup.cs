using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using Blazored.LocalStorage;
using Dikubot.Webapp.Authentication;
using Microsoft.AspNetCore.Http;
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
            
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/webapp/Pages");
            services.AddMudServices();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //Kalender
            services.AddSyncfusionBlazor(); 
           
            services.AddResponseCaching();
            services.AddRouting();
            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);
            services.AddScoped<AuthenticationStateProvider, Authenticator>();

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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using ResourceMonitor.Services;
using ResourceMonitor.Services.Declaration;
using ResourceMonitor.Services.Implementation;

namespace ResourceMonitor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<SyncRulesBackgroundService>();
            services.AddHostedService<CheckNewResourcesBackgroundService>();

            //User-Agent: dandanplay/resmonitor 1.2.3.4
            var userAgent = string.Format(Configuration["Api:UserAgent"],
                Assembly.GetExecutingAssembly().GetName().Version.ToString(4));

            services.AddRefitClient<IDandanplayApi>()
                .ConfigureHttpClient(c =>
                {
                    c.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    c.BaseAddress = new Uri(Configuration["Api:ApiBaseUrl"]);
                });

            services.AddRefitClient<IResApi>()
                .ConfigureHttpClient(c =>
                {
                    c.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    c.BaseAddress = new Uri(Configuration["Api:ResBaseUrl"]);
                });

            services.AddRefitClient<IMagnetApi>()
                .ConfigureHttpClient(c =>
                {
                    c.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    c.BaseAddress = new Uri(Configuration["Api:MagnetBaseUrl"]);
                });

            services.AddSingleton<IRulesContainer, RulesContainer>();
            services.AddTransient<ITorrentService, TorrentService>();

            if (Configuration["Downloader"] == "aria2")
            {
                services.AddHttpClient<IDownloader, Aria2Downloader>();
                services.AddSingleton<IDownloader, Aria2Downloader>();
            }
            else if (Configuration["Downloader"] == "transmission")
            {
            }

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
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
            services.AddHttpClient();
            switch (Configuration["Downloader"]?.ToLowerInvariant())
            {
                case "aria2":
                    services.AddSingleton<IDownloader, Aria2Downloader>();
                    break;
                case "transmission":
                    services.AddSingleton<IDownloader, TransmissionDownloader>();
                    break;
            }

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

            services.AddHostedService<SyncRulesBackgroundService>();
            services.AddHostedService<CheckNewResourcesBackgroundService>();
            
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            
            logger.LogInformation("初始化配置");
            foreach (var kv in Configuration.AsEnumerable())
            {
                logger.LogDebug($"\t{kv.Key}={kv.Value}");
            }
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
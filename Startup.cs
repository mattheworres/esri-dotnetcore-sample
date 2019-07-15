using System;
using System.Linq;
using System.Net.Http;
using Ags.ResourceProxy.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace esri_dotnetcore_sampleapp
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
            services.AddSingleton<IProxyConfigService, ProxyConfigService>((a) =>
            {
                var config = new ProxyConfigService(a.GetService<IHostingEnvironment>(), "esriProxy.config.json");
                return config;
            });
            services.AddSingleton<IProxyService, ProxyService>();

            var serviceProvider = services.BuildServiceProvider();

            var agsProxyConfig = serviceProvider.GetService<IProxyConfigService>();
            // Loop through the config and add Named Clients for use with IHttpClientFactory
            agsProxyConfig.Config.ServerUrls.ToList().ForEach(su =>
            {
                services.AddHttpClient(su.Url)
                    .ConfigurePrimaryHttpMessageHandler(h =>
                    {
                        return new HttpClientHandler
                        {
                            AllowAutoRedirect = false,
                            Credentials = agsProxyConfig.GetCredentials(agsProxyConfig.GetProxyServerUrlConfig((su.Url)))
                        };
                    });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseWhen(context =>
            {
                return context.Request.Path.Value.ToLower().StartsWith(@"/proxy/proxy.ashx", StringComparison.OrdinalIgnoreCase);
                //&& context.User.Identity.IsAuthenticated; // Add this back in to keep unauthenticated users from utilzing the proxy.
            },
            builder =>
                builder.UseAgsProxyServer(
                app.ApplicationServices.GetService<IProxyConfigService>(),
                app.ApplicationServices.GetService<IProxyService>(),
                app.ApplicationServices.GetService<IMemoryCache>())
            );

            app.UseMvc();
        }
    }
}

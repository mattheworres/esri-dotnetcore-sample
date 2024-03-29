﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace esri_dotnetcore_sampleapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5050")
                .UseStartup<Startup>();
    }
}

// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace SimpleKafkaConsumer
{
    public class Program
    {
        private static readonly string HostingEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{HostingEnv}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            // Use appsettings and environment variables for logging config
            var logBuilder = new LoggerConfiguration()
               .ReadFrom.Configuration(Configuration)
               .Enrich.FromLogContext();

            // JSON formatting in Prod, Text in Dev:
            if (HostingEnv == "Production")
            {
                logBuilder.WriteTo.Console(new JsonFormatter());
            }
            else
            {
                logBuilder.WriteTo.Console();
            }

            // Create logger here so fatal logs can be captured
            Log.Logger = logBuilder.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}

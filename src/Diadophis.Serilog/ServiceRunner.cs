// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace Diadophis.Logging.Serilog
{
    // TODO: Needs work so this abstraction is less blunt.  But works for now
    public static class ServiceRunner
    {
        private static readonly string HostingEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{HostingEnv}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Run(Action action)
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
                logBuilder.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                );
            }

            // Create logger here so fatal logs can be captured
            Log.Logger = logBuilder.CreateLogger();

            try
            {
                Log.Information("Starting host");
                action();
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
    }
}

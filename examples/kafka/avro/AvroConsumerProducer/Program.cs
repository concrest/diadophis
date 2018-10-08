// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Diadophis.Logging.Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AvroConsumerProducer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return ServiceRunner.Run(() =>
            {
                CreateWebHostBuilder(args).Build().Run();
            });
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Diadophis.RabbitMq
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseRabbitMqConsumer<TRabbitConfig>(this IWebHostBuilder builder,
            Action<TRabbitConfig> configureConsumer)
            where TRabbitConfig : class, IRabbitMqConfig, new()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.ConfigureServices(services =>
            {
                services.Configure(configureConsumer);
                services.AddHostedService<RabbitMqConsumerService<TRabbitConfig>>();                
            });
        }
    }
}

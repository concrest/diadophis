// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

namespace Diadophis.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqConsumer<TRabbitConfig>(this IServiceCollection services,
            IConfiguration config)
            where TRabbitConfig : class, IRabbitMqConfig, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<TRabbitConfig>(config);

            services.TryAddTransient<IPipelineBuilder, PipelineBuilder>();
            services.TryAddTransient<IRabbitMqPipelineProvider, RabbitMqPipelineProvider>();

            services.TryAddTransient(CreateConnectionFactory);

            services.AddHostedService<RabbitMqConsumerService<TRabbitConfig>>();

            return services;
            
        }

        private static IConnectionFactory CreateConnectionFactory(IServiceProvider serviceProvider)
            => new ConnectionFactory { DispatchConsumersAsync = true };
    }
}

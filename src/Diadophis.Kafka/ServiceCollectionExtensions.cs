// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Diadophis.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumer<TKafkaConfig>(this IServiceCollection services,
            IConfiguration config)
            where TKafkaConfig : class, IKafkaConfig, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<TKafkaConfig>(config);

            services.TryAddTransient<IPipelineBuilder, PipelineBuilder>();
            services.TryAddTransient<IKafkaPipelineProvider, KafkaPipelineProvider>();

            // TODO: Register Confluent.Kafka dependencies

            services.AddHostedService<KafkaConsumerService<TKafkaConfig>>();

            return services;
        }
    }
}

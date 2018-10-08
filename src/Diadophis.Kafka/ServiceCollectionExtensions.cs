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
        public static IServiceCollection AddKafkaConsumer<TConsumerConfig, TKey, TValue>(this IServiceCollection services,
            IConfiguration config)
            where TConsumerConfig : class, IKafkaConsumerConfig<TKey, TValue>, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<TConsumerConfig>(config);

            services.TryAddTransient<IPipelineBuilder, PipelineBuilder>();
            services.TryAddTransient<IKafkaPipelineProvider<TKey, TValue>, KafkaPipelineProvider<TKey, TValue>>();

            services.AddHostedService<KafkaConsumerService<TConsumerConfig, TKey, TValue>>();

            return services;
        }

        public static IServiceCollection AddKafkaProducer<TProducerConfig, TKey, TValue>(this IServiceCollection services,
            IConfiguration config)
            where TProducerConfig : class, IKafkaProducerConfig<TKey, TValue>, new()
        {
            // TODO: wire this up.

            // Check thread safety on producers - Jave version is thread safe, is the C# one?
            // Can we add an IProducer<> to the container as a singleton after calling 
            // CreateStrategy on IKafkaProducerConfig?
            return services;
        }
    }
}

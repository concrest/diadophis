// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Diadophis.Kafka
{
    internal class KafkaPipelineProvider<TKey, TValue> : IKafkaPipelineProvider<TKey, TValue>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaPipelineProvider<TKey, TValue>> _logger;

        private MessageDelegate _pipeline;

        public KafkaPipelineProvider(IServiceProvider serviceProvider,
            ILogger<KafkaPipelineProvider<TKey, TValue>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Initialise(IKafkaConsumerConfig<TKey, TValue> config)
        {
            _logger.LogTrace(LoggingEvents.Initialise, "Initialising provider");

            try
            {
                var builder = _serviceProvider.GetRequiredService<IPipelineBuilder>();
                config.ConfigurePipeline(builder);
                _pipeline = builder.Build();
            }
            catch (Exception ex)
            {
                // Exception during pipeline building - likely due to missing IoC wire up

                _logger.LogError(
                    LoggingEvents.InitialiseError,
                    ex,
                    "Initialisation error"
                );

                // No recovery possible.
                throw;
            }
        }

        public Task InvokePipeline(ConsumeResult<TKey, TValue> consumeResult)
        {
            return _pipeline.Invoke(new KafkaMessageContext<TKey, TValue>(_serviceProvider, consumeResult));
        }

        private static class LoggingEvents
        {
            // 100 series - Configuration and setup
            internal static readonly EventId Initialise = new EventId(101, nameof(Initialise));
            internal static readonly EventId InitialiseError = new EventId(102, nameof(InitialiseError));
        }
    }
}
// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diadophis.RabbitMq
{
    internal class RabbitMqConsumerService<TConfig> : IHostedService
        where TConfig : class, IRabbitMqConfig, new()
    {
        private static class LoggingEvents
        {
            // 100 series - Configuration and setup
            internal static readonly EventId Configure = new EventId(101, nameof(Configure));

            // 200 series - Starting and Stopping
            internal static readonly EventId StartAsync = new EventId(201, nameof(StartAsync));
            internal static readonly EventId StopAsync = new EventId(202, nameof(StopAsync));
            internal static readonly EventId BuildPipeline = new EventId(203, nameof(BuildPipeline));
            internal static readonly EventId BuildPipelineError = new EventId(204, nameof(BuildPipelineError));

            // 300 series - Consuming messages
            internal static readonly EventId ReceivedMessage = new EventId(202, nameof(ReceivedMessage));
            
        }

        private readonly TConfig _config;
        private readonly ILogger<RabbitMqConsumerService<TConfig>> _logger;
        //private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;

        private MessageDelegate _pipeline;

        public RabbitMqConsumerService(
            IOptions<TConfig> config,
            ILogger<RabbitMqConsumerService<TConfig>> logger,
            IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _logger.BeginScope("Using configuration from {ConfigType}", _config.GetType().FullName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingEvents.StartAsync,"Starting RabbitMqConsumerService");

            BuildPipeline();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingEvents.StopAsync, "Stopping RabbitMqConsumerService");

            return Task.CompletedTask;
        }

        private void BuildPipeline()
        {
            _logger.LogDebug(LoggingEvents.BuildPipeline, "Building pipeline from type");
            
            try
            {
                var builder = _serviceProvider.GetRequiredService<PipelineBuilder>();
                _config.ConfigurePipeline(builder);
                _pipeline = builder.Build();
            }
            catch (Exception ex)
            {
                // Exception during pipeline building - probably due to missing IoC wire up

                _logger.LogError(
                    LoggingEvents.BuildPipelineError,
                    ex,
                    "Error building pipeline"                    
                );

                // No recovery possible.
                throw;
            }
        }
    }
}
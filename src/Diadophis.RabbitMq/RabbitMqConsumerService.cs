// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
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

            // 300 series - Consuming messages
            internal static readonly EventId ReceivedMessage = new EventId(202, nameof(ReceivedMessage));
        }

        private readonly IOptions<TConfig> _config;
        private readonly ILogger<RabbitMqConsumerService<TConfig>> _logger;

        public RabbitMqConsumerService(
            IOptions<TConfig> config,
            ILogger<RabbitMqConsumerService<TConfig>> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                LoggingEvents.StartAsync, 
                "Starting RabbitMqConsumerService {Config}",
                _config.Value
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(LoggingEvents.StopAsync, 
                "Stopping RabbitMqConsumerService {Config}",
                _config.Value
            );

            return Task.CompletedTask;
        }
    }
}
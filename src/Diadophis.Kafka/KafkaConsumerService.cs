// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diadophis.Kafka
{
    internal class KafkaConsumerService<TConfig> : IHostedService, IDisposable
        where TConfig : class, IKafkaConfig, new()
    {
        private bool _isDisposed = false;

        private readonly ILogger<KafkaConsumerService<TConfig>> _logger;
        private readonly TConfig _config;

        public KafkaConsumerService(
            ILogger<KafkaConsumerService<TConfig>> logger,
            IOptions<TConfig> config)
        {
            _logger = logger;
            _config = config.Value;

            _logger.BeginScope("Using configuration from {ConfigType}", _config.GetType().FullName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync, "Starting KafkaConsumerService");

            // TODO: Start consuming messages

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync, "Stopping KafkaConsumerService");

            // TODO: Stop consuming messages

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            _logger.LogTrace(LoggingEvents.Disposing, "Disposing");

            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        private static class LoggingEvents
        {
            // 100 series - Starting and Stopping
            internal static readonly EventId StartAsync = new EventId(101, nameof(StartAsync));
            internal static readonly EventId StopAsync = new EventId(102, nameof(StopAsync));
            internal static readonly EventId Disposing = new EventId(103, nameof(Disposing));

            // 200 series - Consuming messages
            internal static readonly EventId ConsumeMessageStart = new EventId(201, nameof(ConsumeMessageStart));
            internal static readonly EventId ConsumeMessageEnd = new EventId(202, nameof(ConsumeMessageEnd));
            internal static readonly EventId ConsumeMessageError = new EventId(203, nameof(ConsumeMessageError));
        }
    }
}
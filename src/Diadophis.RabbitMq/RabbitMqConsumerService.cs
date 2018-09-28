// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    internal class RabbitMqConsumerService<TConfig> : IHostedService, IDisposable
        where TConfig : class, IRabbitMqConfig, new()
    {
        private static class LoggingEvents
        {
            // 100 series - Configuration and setup
            internal static readonly EventId Configure = new EventId(101, nameof(Configure));

            // 200 series - Starting and Stopping
            internal static readonly EventId StartAsync         = new EventId(201, nameof(StartAsync));
            internal static readonly EventId StopAsync          = new EventId(202, nameof(StopAsync));
            internal static readonly EventId BuildPipeline      = new EventId(203, nameof(BuildPipeline));
            internal static readonly EventId BuildPipelineError = new EventId(204, nameof(BuildPipelineError));
            internal static readonly EventId Disposing          = new EventId(205, nameof(Disposing));

            // 300 series - Consuming messages
            internal static readonly EventId ConsumeMessageStart  = new EventId(301, nameof(ConsumeMessageStart));
            internal static readonly EventId ConsumeMessageEnd    = new EventId(302, nameof(ConsumeMessageEnd));
            internal static readonly EventId ConsumerMessageError = new EventId(303, nameof(ConsumerMessageError));
            internal static readonly EventId CallbackException    = new EventId(304, nameof(CallbackException));
        }

        private readonly TConfig _config;
        private readonly ILogger<RabbitMqConsumerService<TConfig>> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;

        private bool _isDisposed = false;
        private MessageDelegate _pipeline;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqConsumerService(
            IOptions<TConfig> config,
            ILogger<RabbitMqConsumerService<TConfig>> logger,
            IServiceProvider serviceProvider,
            IConnectionFactory connectionFactory)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _connectionFactory = connectionFactory;

            _logger.BeginScope("Using configuration from {ConfigType}", _config.GetType().FullName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync,"Starting RabbitMqConsumerService");

            BuildPipeline();

            _connectionFactory.Uri = new Uri(_config.ConnectionUri);

            _connection = _connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.CallbackException += (sender, args) =>
            {
                _logger.LogWarning(LoggingEvents.CallbackException,
                    args.Exception,
                    "Exception raised on CallbackException event handler");
            };

            _config.ConfigureChannel(_channel);
            
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;

            _channel.BasicConsume(_config.QueueName, _config.AutoAck, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StopAsync, "Stopping RabbitMqConsumerService");

            // TODO: Disconnect channel?

            return Task.CompletedTask;
        }

        private void BuildPipeline()
        {
            _logger.LogTrace(LoggingEvents.BuildPipeline, "Building pipeline from type");
            
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

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs message)
        {
            _logger.LogDebug(LoggingEvents.ConsumeMessageStart, "Started consuming message");
            try
            {
                await _pipeline.Invoke(new RabbitMqMessageContext(_serviceProvider, message));

                _logger.LogDebug(LoggingEvents.ConsumeMessageEnd, "Finished consuming message");
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.ConsumerMessageError,
                    ex,
                    "Error consuming message");

                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _channel?.Dispose();
                    _channel = null;

                    _connection?.Dispose();
                    _connection = null;
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            _logger.LogTrace(LoggingEvents.Disposing, "Disposing");

            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
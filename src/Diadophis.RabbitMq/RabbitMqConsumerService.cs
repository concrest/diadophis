// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly TConfig _config;
        private readonly ILogger<RabbitMqConsumerService<TConfig>> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IRabbitMqPipelineProvider _pipelineProvider;

        private bool _isDisposed = false;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqConsumerService(
            IOptions<TConfig> config,
            ILogger<RabbitMqConsumerService<TConfig>> logger,
            IRabbitMqPipelineProvider pipelineProvider,
            IConnectionFactory connectionFactory)
        {
            _config = config.Value;
            _logger = logger;
            _pipelineProvider = pipelineProvider;
            _connectionFactory = connectionFactory;

            _logger.BeginScope("ConfigType:{ConfigType}", _config.GetType().FullName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync, "Starting RabbitMqConsumerService");

            _pipelineProvider.Initialise(_config);

            StartConsumingMessages();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StopAsync, "Stopping RabbitMqConsumerService");
                        
            _channel?.Close();
            _connection?.Close();

            return Task.CompletedTask;
        }

        private void StartConsumingMessages()
        {
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
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs message)
        {
            _logger.LogDebug(LoggingEvents.ConsumeMessageStart, "Started consuming message");
            try
            {
                await _pipelineProvider.InvokePipeline(_channel, message);

                _logger.LogDebug(LoggingEvents.ConsumeMessageEnd, 
                    "Finished consuming message");
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.ConsumeMessageError,
                    ex,
                    "Error consuming message");

                // TODO: What should we do with unhandled exceptions?
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
            internal static readonly EventId CallbackException = new EventId(204, nameof(CallbackException));
        }
    }
}
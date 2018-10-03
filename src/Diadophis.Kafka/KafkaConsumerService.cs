﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diadophis.Kafka
{
    internal class KafkaConsumerService<TConfig> : IHostedService, IDisposable
        where TConfig : class, IKafkaConfig, new()
    {
        private const int StopTaskWaitTimeoutMillis = 5000;

        private bool _isDisposed = false;

        private readonly ILogger<KafkaConsumerService<TConfig>> _logger;
        private readonly TConfig _config;
        private readonly IKafkaPipelineProvider _pipelineProvider;
        
        private Consumer<Ignore, string> _consumer;
        private Task _consumerTask;

        public KafkaConsumerService(
            ILogger<KafkaConsumerService<TConfig>> logger,
            IOptions<TConfig> config,
            IKafkaPipelineProvider pipelineProvider)
        {
            _logger = logger;
            _config = config.Value;
            _pipelineProvider = pipelineProvider;

            _logger.BeginScope("ConfigType:{ConfigType}", _config.GetType().FullName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync, "Starting KafkaConsumerService");

            _pipelineProvider.Initialise(_config);

            _consumerTask = Task.Run(() => StartKafkaConsumer(cancellationToken));

            return Task.CompletedTask;
        }

        private async Task StartKafkaConsumer(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _config.BrokerUrls,
                GroupId = _config.ConsumerGroupId
            };

            _config.ConfigureConsumer(config);

            // TODO: Needs refactoring to use DI
            // But how to specify key and value type since different consumers will need different schemas (when using Avro)
            _consumer = new Consumer<Ignore, string>(config);

            _consumer.OnError += (_, e) =>
            {
                _logger.LogErrorEvent(LoggingEvents.ConsumerOnErrorEvent, e);
            };

            _consumer.Subscribe(_config.Topics);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    // TODO: Figure out how to create an extension method for this
                    _logger.LogDebug(LoggingEvents.ConsumeMessageStart,
                        "Started consuming message. Topic: {Topic}, Key: {Key}, Value: {Value}, Offset: {Offset}",
                        consumeResult.Topic,
                        consumeResult.Key,
                        consumeResult.Value,
                        consumeResult.Offset);

                    // TODO: How to make this async/await all the way down?
                    await _pipelineProvider.InvokePipeline<Ignore, string>(consumeResult);

                    // TODO: Figure out how to create an extension method for this
                    _logger.LogDebug(LoggingEvents.ConsumeMessageEnd,
                        "Started consuming message. Topic: {Topic}, Key: {Key}, Value: {Value}, Offset: {Offset}",
                        consumeResult.Topic, 
                        consumeResult.Key,
                        consumeResult.Value,
                        consumeResult.Offset);
                }
                catch (ConsumeException ce)
                {
                    _logger.LogError(LoggingEvents.ConsumeMessageException,
                        ce,
                        "Error consuming message");

                    // Example on GitHub swallows these?
                }
                catch (Exception ex)
                {
                    _logger.LogError(LoggingEvents.ConsumeMessageException,
                        ex,
                        "Error consuming message");

                    // TODO: What should we do with unhandled exceptions? Ignore?
                    //throw;
                }
            }

            _logger.LogDebug(LoggingEvents.ExitedConsumerLoop, "Exited consumer loop");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.StartAsync, "Stopping KafkaConsumerService");

            if (_consumerTask != null)
            {
                if (!Task.WaitAll(new[] { _consumerTask }, StopTaskWaitTimeoutMillis))
                {
                    _logger.LogWarning(LoggingEvents.ConsumerTaskNotStopped,
                        "Consumer task didn't stop after {StopTaskWaitTimeoutMillis}",
                        StopTaskWaitTimeoutMillis);
                }
            }

            _consumer?.Close();

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _consumer?.Dispose();
                    _consumer = null;
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
            internal static readonly EventId ConsumeMessageException = new EventId(203, nameof(ConsumeMessageException));
            internal static readonly EventId ConsumerOnErrorEvent = new EventId(204, nameof(ConsumerOnErrorEvent));
            internal static readonly EventId EndOfPartition = new EventId(205, nameof(EndOfPartition));
            internal static readonly EventId ExitedConsumerLoop = new EventId(206, nameof(ExitedConsumerLoop));
            internal static readonly EventId ConsumerTaskNotStopped = new EventId(207, nameof(ConsumerTaskNotStopped));
        }
    }
}
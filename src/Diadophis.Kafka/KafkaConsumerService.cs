// Copyright 2018 Concrest
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
    internal class KafkaConsumerService<TConfig, TKey, TValue> : BackgroundService
        where TConfig : class, IKafkaConsumerConfig<TKey, TValue>, new()
    {
        // Same 100 millisecond timeout as in https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/src/Confluent.Kafka/Consumer.cs
        private static readonly TimeSpan ConsumeTimeout = TimeSpan.FromMilliseconds(100);

        private readonly ILogger<KafkaConsumerService<TConfig, TKey, TValue>> _logger;
        private readonly TConfig _config;
        private readonly IKafkaPipelineProvider<TKey, TValue> _pipelineProvider;
        
        public KafkaConsumerService(
            ILogger<KafkaConsumerService<TConfig, TKey, TValue>> logger,
            IOptions<TConfig> config,
            IKafkaPipelineProvider<TKey, TValue> pipelineProvider)
        {
            _logger = logger;
            _config = config.Value;
            _pipelineProvider = pipelineProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogStart(LoggingEvents.StartAsync, _config);

            _pipelineProvider.Initialise(_config);

            // TODO: Would it make sense to pass the CancellationToken to Task.Run as well?
            return Task.Run(async () =>
            {
                try
                {
                    await StartKafkaConsumer(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(LoggingEvents.FatalConsumerException,
                            ex,
                            "Fatal error initialising consumer.  Messages are not being processed");
                }
            });
        }

        private async Task StartKafkaConsumer(CancellationToken cancellationToken)
        {
            using (var consumerStrategy = _config.CreateConsumerStrategy())
            {
                consumerStrategy.Consumer.OnError += (_, e) =>
                {
                    _logger.LogErrorEvent(LoggingEvents.ConsumerOnErrorEvent, e);
                };

                consumerStrategy.Consumer.Subscribe(_config.Topic);

                // TODO: Would we need a hook here for resuming from a set offset?
                // Does Assign need to get called after Subscribe?

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // .Consume(CancellationToken) uses an infinite loop and ThrowIfCancellationRequested
                        // So use the timeout and loop back until IsCancellationRequested 
                        var consumeResult = consumerStrategy.Consumer.Consume(ConsumeTimeout);
                        if (consumeResult == null)
                        {
                            // Nothing this time.
                            continue;
                        }

                        _logger.LogKafkaMessage(LoggingEvents.ConsumeMessageStart, "Received message", consumeResult);

                        // TODO: Clarify error handling behaviour

                        // Consumers will need different error handling strategies - some will ignore and move on, 
                        // some will need to retry. Some will be managing their own offset, for example if the message
                        // is being handled in a transaction then the offset can be committed to that same data store
                        // as part of that transaction.  The consumer would then need to start from that offset, so the 
                        // service needs to be flexible enough to allow that.  For now, we're letting Kafka manage
                        // the offsets and errors in processing a message are just logged.
                        await _pipelineProvider.InvokePipeline(consumeResult);

                        _logger.LogKafkaMessage(LoggingEvents.ConsumeMessageEnd, "Finished with message", consumeResult);
                    }
                    catch (ConsumeException cex)
                    {
                        _logger.LogKafkaException(LoggingEvents.ConsumeMessageException, cex);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.ConsumeMessageException,
                            ex,
                            "Error consuming message");
                    }
                }

                consumerStrategy.Close();

                _logger.LogInformation(LoggingEvents.StopAsync, "Stopping KafkaConsumerService");
            }
        }

        private static class LoggingEvents
        {
            // 100 series - Starting and Stopping
            internal static readonly EventId StartAsync = new EventId(101, nameof(StartAsync));
            internal static readonly EventId StopAsync = new EventId(102, nameof(StopAsync));
            internal static readonly EventId Disposing = new EventId(103, nameof(Disposing));
            internal static readonly EventId FatalConsumerException = new EventId(104, nameof(FatalConsumerException));

            // 200 series - Consuming messages
            internal static readonly EventId ConsumeMessageStart = new EventId(201, nameof(ConsumeMessageStart));
            internal static readonly EventId ConsumeMessageEnd = new EventId(202, nameof(ConsumeMessageEnd));
            internal static readonly EventId ConsumeMessageException = new EventId(203, nameof(ConsumeMessageException));
            internal static readonly EventId ConsumerOnErrorEvent = new EventId(204, nameof(ConsumerOnErrorEvent));
            internal static readonly EventId ExitedConsumerLoop = new EventId(205, nameof(ExitedConsumerLoop));
        }
    }
}
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
    internal class KafkaConsumerService<TConfig> : BackgroundService
        where TConfig : class, IKafkaConfig, new()
    {
        // Same 100 millisecond timeout as in https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/src/Confluent.Kafka/Consumer.cs
        private static readonly TimeSpan ConsumeTimeout = TimeSpan.FromMilliseconds(100);

        private readonly ILogger<KafkaConsumerService<TConfig>> _logger;
        private readonly TConfig _config;
        private readonly IKafkaPipelineProvider _pipelineProvider;
        
        public KafkaConsumerService(
            ILogger<KafkaConsumerService<TConfig>> logger,
            IOptions<TConfig> config,
            IKafkaPipelineProvider pipelineProvider)
        {
            _logger = logger;
            _config = config.Value;
            _pipelineProvider = pipelineProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogStart(LoggingEvents.StartAsync, _config);

            _pipelineProvider.Initialise(_config);

            return Task.Run(() => StartKafkaConsumer(stoppingToken));
        }


        private async Task StartKafkaConsumer(CancellationToken cancellationToken)
        {
            /*
             * Design is wrong.  Need to be able to configure the consumer, plus control how to instantiate the
             * consumer.  For Avro, need to pass in Avro serde options, and Schema Registry config.
             * 
             * Maybe all this should be in the interface call?
             * 
             * Need better abstraction around the consumer!
             * 
             */


            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config.BrokerUrls,
                GroupId = _config.ConsumerGroupId
            };

            _config.ConfigureConsumer(consumerConfig);

            // TODO: Needs refactoring to use DI
            // But how to specify key and value type since different consumers will need different schemas (when using Avro)
            using (var consumer = new Consumer<Ignore, string>(consumerConfig))
            {

                consumer.OnError += (_, e) =>
                {
                    _logger.LogErrorEvent(LoggingEvents.ConsumerOnErrorEvent, e);
                };

                consumer.Subscribe(_config.Topics);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // .Consume(CancellationToken) uses an infinite loop and ThrowIfCancellationRequested
                        // So use the timeout and loop back until IsCancellationRequested 
                        var consumeResult = consumer.Consume(ConsumeTimeout);
                        if (consumeResult == null)
                        {
                            // Nothing this time.
                            continue;
                        }

                        _logger.LogKafkaMessage(LoggingEvents.ConsumeMessageStart, "Received message", consumeResult);

                        await _pipelineProvider.InvokePipeline(consumeResult);

                        _logger.LogKafkaMessage(LoggingEvents.ConsumeMessageEnd, "Finished with message", consumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.ConsumeMessageException,
                            ex,
                            "Error consuming message");
                    }
                }

                consumer.Close();

                _logger.LogDebug(LoggingEvents.ExitedConsumerLoop, "Exited consumer loop");
            }
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
            internal static readonly EventId ExitedConsumerLoop = new EventId(205, nameof(ExitedConsumerLoop));
        }
    }
}
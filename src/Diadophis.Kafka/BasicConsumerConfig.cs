// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    /// <summary>
    /// Basic consumer configuration settings.
    /// </summary>
    public abstract class BasicConsumerConfig<TKey, TValue> : IKafkaConsumerConfig<TKey, TValue>
    {
        public string BrokerUrls { get ; set; }
        public string ConsumerGroupId { get; set; }
        public string Topic { get; set; }

        public abstract void ConfigurePipeline(IPipelineBuilder pipe);

        public virtual IConsumerStrategy<TKey, TValue> CreateConsumerStrategy()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = BrokerUrls,
                GroupId = ConsumerGroupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetResetType.Latest
            };

            return new BasicConsumerStrategy<TKey, TValue>(config);
        }
    }
}

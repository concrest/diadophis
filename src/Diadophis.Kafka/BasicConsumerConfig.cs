// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    /// <summary>
    /// Basic consumer configuration settings.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class BasicConsumerConfig<TKey, TValue> : IKafkaConfig
    {
        public string BrokerUrls { get ; set; }
        public string ConsumerGroupId { get; set; }
        public string[] Topics { get; set; }

        /// <summary>
        /// Configures the consumer to start reading from the latest offset 
        /// in the absence of a previously committed offset.
        /// Sets EnableAutoCommit to true
        /// </summary>
        /// <param name="config"></param>
        public virtual void ConfigureConsumer(ConsumerConfig config)
        {
            config.EnableAutoCommit = true;
            config.AutoOffsetReset = AutoOffsetResetType.Latest;
        }

        public abstract void ConfigurePipeline(IPipelineBuilder pipe);
        
    }
}

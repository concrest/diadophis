// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    /// <summary>
    /// Basic producer configuration settings.
    /// </summary>
    public class BasicProducerConfig<TKey, TValue> : IKafkaProducerConfig<TKey, TValue>
    {
        public string BrokerUrls { get ; set; }
        public string Topic { get; set; }

        public virtual IProducerStrategy<TKey, TValue> CreateProducerStrategy()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = BrokerUrls
            };

            return new BasicProducerStrategy<TKey, TValue>(config);
        }
    }
}

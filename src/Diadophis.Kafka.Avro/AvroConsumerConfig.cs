// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Diadophis.Kafka.Avro
{
    public abstract class AvroConsumerConfig<TKey, TValue> : BasicConsumerConfig<TKey, TValue>
    {
        public string SchemaRegistryUrls { get; set; }

        public override IConsumerStrategy<TKey, TValue> CreateConsumerStrategy()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = BrokerUrls,
                GroupId = ConsumerGroupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetResetType.Latest
            };

            var avroConfig = new AvroSerdeProviderConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                SchemaRegistryUrl = SchemaRegistryUrls,
                // optional schema registry client properties:
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10
            };

            return new AvroConsumerStrategy<TKey, TValue>(config, avroConfig);
        }
    }
}

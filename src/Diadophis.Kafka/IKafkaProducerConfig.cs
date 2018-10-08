// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Kafka
{
    public interface IKafkaProducerConfig<TKey, TValue>
    {
        string BrokerUrls { get; set; }
        string Topic { get; set; }

        IProducerStrategy<TKey, TValue> CreateProducerStrategy();
    }
}
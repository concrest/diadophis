﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Kafka
{
    public interface IKafkaConfig<TKey, TValue>
    {
        string BrokerUrls { get; set; }
        string ConsumerGroupId { get; set; }
        string[] Topics { get; set; }

        IConsumerStrategy<TKey, TValue> CreateConsumerStrategy();

        void ConfigurePipeline(IPipelineBuilder pipe);
    }
}
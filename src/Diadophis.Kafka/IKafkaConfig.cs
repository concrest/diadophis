// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IKafkaConfig
    {
        string BrokerUrls { get; set; }
        string ConsumerGroupId { get; set; }
        string[] Topics { get; set; }

        void ConfigureConsumer(ConsumerConfig config);

        void ConfigurePipeline(IPipelineBuilder pipe);
        
    }
}
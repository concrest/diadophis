// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Kafka.Test
{
    internal class FakeKafkaConfig : IKafkaConfig<string, string>
    {
        public string BrokerUrls { get; set; } = "broker:9090";
        public string ConsumerGroupId { get; set; } = "consumer";
        public string[] Topics { get; set; } = new[] { "topic" };

        public void ConfigurePipeline(IPipelineBuilder pipe)
        {   
        }

        public IConsumerStrategy<string, string> CreateConsumerStrategy()
        {
            return new FakeConsumerStrategy();
        }
    }
}
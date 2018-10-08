// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diadophis.Kafka.Test
{
    internal class TestKafkaConfig : IKafkaConsumerConfig<string, string>
    {
        public List<MessageContext> MessagesReceived { get; } = new List<MessageContext>();

        public string BrokerUrls { get; set; } = "broker:9090";
        public string ConsumerGroupId { get; set; } = "consumer";
        public string Topic { get; set; } = "topic";

        public void ConfigurePipeline(IPipelineBuilder builder)
        {
            builder.Run(m =>
            {
                MessagesReceived.Add(m);
                return Task.CompletedTask;
            });
        }

        public IConsumerStrategy<string, string> CreateConsumerStrategy()
        {
            return new FakeConsumerStrategy();
        }
    }
}
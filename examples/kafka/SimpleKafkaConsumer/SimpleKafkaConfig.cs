// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis;
using Diadophis.Kafka;

namespace SimpleKafkaConsumer
{
    internal class SimpleKafkaConfig : IKafkaConfig
    {
        public string BrokerUrls { get; set; }

        public string ConsumerGroupId => "my-consumer-group-id";

        public IEnumerable<string> Topics => new[] { "topic1" };

        public void ConfigureConsumer(ConsumerConfig config)
        {
            config.EnableAutoCommit = true;
            config.AutoOffsetReset = AutoOffsetResetType.Earliest;
        }

        public void ConfigurePipeline(IPipelineBuilder pipe)
        {
            // TODO: middleware here
            pipe.Run(msg =>
            {
                return Task.CompletedTask;
            });
        }
    }
}

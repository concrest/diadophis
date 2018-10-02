// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Diadophis;
using Diadophis.Kafka;

namespace SimpleKafkaConsumer
{
    internal class SimpleKafkaConfig : IKafkaConfig
    {
        public string BrokerUrls { get; set; }

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

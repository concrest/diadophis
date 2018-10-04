// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis;
using Diadophis.Kafka;

namespace SimpleKafkaConsumer
{
    internal class SimpleKafkaConfig : IKafkaConfig
    {
        // BrokerUrls, ConsumerGroupId, Topics are set from appsettings.json or appsettings.{Environment}.json
        // Alternatively from environment variables:
        //
        // Simple strings: 
        //         SimpleKafkaConfig__BrokerUrls
        //         SimpleKafkaConfig__ConsumerGroupId
        // Arrays:
        //         SimpleKafkaConfig__Topics__0
        //         [SimpleKafkaConfig__Topics__1]
        //
        // Note: SimpleKafkaConfig as the prefix is from the appsettings.json parent property, used in the call to 
        // AddKafkaConsumer from the Startup class
        //
        // See:
        //   - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1
        //   - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.1

        public string BrokerUrls { get; set; }

        public string ConsumerGroupId { get; set; }

        public string[] Topics { get; set; }

        public void ConfigureConsumer(ConsumerConfig config)
        {
            config.EnableAutoCommit = true;
            config.AutoOffsetReset = AutoOffsetResetType.Earliest;
        }

        public void ConfigurePipeline(IPipelineBuilder pipe)
        {
            pipe.UseMiddleware<ExampleMiddleware>();
        }
    }
}

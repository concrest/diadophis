// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Kafka.Test
{
    internal class FakeBasicConsumerConfig : BasicConsumerConfig<string, string>
    {
        public FakeBasicConsumerConfig()
        {
            BrokerUrls = "broker1";
            ConsumerGroupId = "consumer";
            Topics = new[] { "topic" };
        }

        public override void ConfigurePipeline(IPipelineBuilder pipe)
        {
            // configure fake pipeline...
        }
    }
}

// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Kafka
{
    public interface IKafkaConfig
    {
        string BrokerUrls { get; set; }
        void ConfigurePipeline(IPipelineBuilder pipe);
    }
}
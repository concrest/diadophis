// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.RabbitMq
{
    public interface IRabbitMqConfig
    {
        string Uri { get; }
        void ConfigurePipeline(IPipelineBuilder builder);
    }
}
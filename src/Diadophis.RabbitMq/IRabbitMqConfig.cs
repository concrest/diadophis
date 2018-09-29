// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using RabbitMQ.Client;

namespace Diadophis.RabbitMq
{
    public interface IRabbitMqConfig
    {
        string ConnectionUri { get; }
        string QueueName { get; }
        bool AutoAck { get; }

        void ConfigurePipeline(IPipelineBuilder builder);
        void ConfigureChannel(IModel channel);
    }
}
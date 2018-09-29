// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Diadophis.RabbitMq.Test
{
    class TestRabbitMqConfig : IRabbitMqConfig
    {
        public List<MessageContext> MessagesReceived { get; } = new List<MessageContext>();

        public string ConnectionUri { get; } = "amqp://queue:12345";

        public string QueueName { get; } = "queue";

        public bool AutoAck => false;

        public void ConfigureChannel(IModel channel)
        {
        }

        public void ConfigurePipeline(IPipelineBuilder builder)
        {
            builder.Run(m =>
            {
                MessagesReceived.Add(m);
                return Task.CompletedTask;
            });
        }
    }
}

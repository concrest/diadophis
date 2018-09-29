// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Diadophis.RabbitMq.Test
{
    public class RabbitMqMessageContextTests
    {
        [Fact]
        public void MessageContext_has_RabbitMq_message_property()
        {
            var rabbitMessage = new BasicDeliverEventArgs();

            var sut = new RabbitMqMessageContext(Mock.Of<IServiceProvider>(), Mock.Of<IModel>(), rabbitMessage);

            var actual = sut.GetRabbitMqMessage();

            Assert.Same(actual, rabbitMessage);
        }

        [Fact]
        public void MessageContext_has_RabbitMqchannel_property()
        {
            var channel = Mock.Of<IModel>();

            var sut = new RabbitMqMessageContext(Mock.Of<IServiceProvider>(), channel, new BasicDeliverEventArgs());

            var actual = sut.GetRabbitMqChannel();

            Assert.Same(actual, channel);
        }
    }
}

// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
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

            var sut = new RabbitMqMessageContext(Mock.Of<IServiceProvider>(), rabbitMessage);

            var actual = sut.GetRabbitMqMessage();

            Assert.Same(actual, rabbitMessage);
        }
    }
}

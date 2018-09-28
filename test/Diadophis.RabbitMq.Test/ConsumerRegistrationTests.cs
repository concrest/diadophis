// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Diadophis.RabbitMq.Test
{
    public class ConsumerRegistrationTests
    {
        private readonly IServiceCollection _serviceCollection;

        public ConsumerRegistrationTests()
        {
            _serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void Pipeline_is_transient_if_not_already_registered()
        {
            _serviceCollection.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>());

            var actual = _serviceCollection.Single(sd => sd.ServiceType == typeof(PipelineBuilder));

            Assert.Equal(ServiceLifetime.Transient, actual.Lifetime);
        }

        [Fact]
        public void RabbitMq_client_Connection_factory_is_transient_if_not_already_registered()
        {
            _serviceCollection.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>());

            var actual = _serviceCollection.Single(sd => sd.ServiceType == typeof(IConnectionFactory));

            Assert.Equal(ServiceLifetime.Transient, actual.Lifetime);
        }

        [Fact]
        public void Consumer_service_is_a_hosted_service()
        {
            _serviceCollection.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>());

            var actual = _serviceCollection.Single(sd => sd.ImplementationType == typeof(RabbitMqConsumerService<TestRabbitMqConfig>));

            Assert.Equal(typeof(IHostedService), actual.ServiceType);
        }

        [Fact]
        public void Cannot_call_on_a_null_service_collection()
        {
            ServiceCollection isNull = null;

            var actual = Assert.Throws<ArgumentNullException>(
                () => isNull.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>())
            );

            Assert.Equal("services", actual.ParamName);
        }
    }
}

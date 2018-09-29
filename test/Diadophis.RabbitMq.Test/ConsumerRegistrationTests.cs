// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(TransientServiceRegistrationTestCases))]
        public void Registration_wires_up_transient_services(ServiceRegistrationTestCase testCase)
        {
            _serviceCollection.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>());

            var actual = _serviceCollection.Single(sd => sd.ServiceType == testCase.ServiceType);

            Assert.Equal(ServiceLifetime.Transient, actual.Lifetime);
        }

        [Theory]
        [MemberData(nameof(TransientServiceRegistrationTestCases))]
        public void Registration_does_not_wire_up_multiple_services(ServiceRegistrationTestCase testCase)
        {

            _serviceCollection.Add(new ServiceDescriptor(testCase.ServiceType, testCase.Instance));
            _serviceCollection.UseRabbitMqConsumer<TestRabbitMqConfig>(Mock.Of<IConfiguration>());

            var actual = _serviceCollection.Count(sd => sd.ServiceType == testCase.ServiceType);

            Assert.Equal(1, actual);
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

        public static IEnumerable<object[]> TransientServiceRegistrationTestCases()
        {
            yield return new object[] { new ServiceRegistrationTestCase<IRabbitMqPipelineProvider>() };
            yield return new object[] { new ServiceRegistrationTestCase<IConnectionFactory>() };
            yield return new object[] { new ServiceRegistrationTestCase<IPipelineBuilder>() };
        }

        private class ServiceRegistrationTestCase<TServiceType> : ServiceRegistrationTestCase
            where TServiceType : class
        {
            public ServiceRegistrationTestCase()
            {
                ServiceType = typeof(TServiceType);
                Instance = Mock.Of<TServiceType>();
            }
        }

        public class ServiceRegistrationTestCase
        {
            public Type ServiceType { get; set; }
            public object Instance { get; set; }
        }
    }
}

// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Diadophis.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Diadophis.RabbitMq.Test
{
    public class PipelineProviderTests
    {
        private readonly RabbitMqPipelineProvider _sut;

        private readonly FakeServiceProvider _serviceProvider;
        private readonly FakeLogger<RabbitMqPipelineProvider> _logger;

        public PipelineProviderTests()
        {
            _serviceProvider = new FakeServiceProvider();
            _logger = new FakeLogger<RabbitMqPipelineProvider>();

            _sut = new RabbitMqPipelineProvider(_serviceProvider, _logger);
        }

        [Fact]
        public void Initialisation_errors_are_logged()
        {
            // There's no setup for IPipelineBuilder in IoC, so this should throw
            Assert.Throws<InvalidOperationException>(() => _sut.Initialise(Mock.Of<IRabbitMqConfig>()));

            Assert.Contains(_logger.LogEntries, l => l.EventId.Id == 102 && l.LogLevel == LogLevel.Error);
        }

        [Fact]
        public async Task Invoking_pipeline_calls_configured_middleware()
        {
            TestRabbitMqConfig testConfig = InitialiseSut();

            var message = new BasicDeliverEventArgs();
            var channel = Mock.Of<IModel>();

            await _sut.InvokePipeline(channel, message);

            var actual = testConfig.MessagesReceived.Single();

            Assert.Same(message, actual.GetRabbitMqMessage());
            Assert.Same(channel, actual.GetRabbitMqChannel());
        }

        private TestRabbitMqConfig InitialiseSut()
        {
            _serviceProvider.Container.Add(
                typeof(IPipelineBuilder),
                new PipelineBuilder(_serviceProvider, new FakeLogger<PipelineBuilder>())
            );

            var testConfig = new TestRabbitMqConfig();
            _sut.Initialise(testConfig);

            return testConfig;
        }
    }
}

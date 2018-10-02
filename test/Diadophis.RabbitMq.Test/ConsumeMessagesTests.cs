// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Diadophis.RabbitMq.Test
{
    public class ConsumeMessagesTests
    {
        private readonly CancellationToken Token = new CancellationToken();

        private readonly RabbitMqConsumerService<TestRabbitMqConfig> _sut;

        private readonly FakeLogger<RabbitMqConsumerService<TestRabbitMqConfig>> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IModel _channel;

        public ConsumeMessagesTests()
        {
            _logger = new FakeLogger<RabbitMqConsumerService<TestRabbitMqConfig>>();

            _channel = Mock.Of<IModel>();

            _connectionFactory = Mock.Of<IConnectionFactory>(
                f => f.CreateConnection() == Mock.Of<IConnection>(
                    c => c.CreateModel() == _channel
                )
            );

            _sut = new RabbitMqConsumerService<TestRabbitMqConfig>(
                Mock.Of<IOptions<TestRabbitMqConfig>>(o => o.Value == new TestRabbitMqConfig()),
                _logger,
                Mock.Of<IRabbitMqPipelineProvider>(),
                _connectionFactory
                );
        }

        [Fact]
        public async Task Consumer_callback_exceptions_are_logged()
        {
            await _sut.StartAsync(Token);

            Mock.Get(_channel).Raise(c => c.CallbackException += null, new CallbackExceptionEventArgs(new ArgumentNullException()));

            Assert.Contains(_logger.LogEntries, e => e.EventId.Id == 204 && e.LogLevel == LogLevel.Warning);
        }

        [Fact]
        public async Task Stop_without_start_is_no_op()
        {
            await _sut.StopAsync(Token);

            Assert.Contains(_logger.LogEntries, e => e.EventId.Id == 102 && e.LogLevel == LogLevel.Information);
        }

        [Fact]
        public void Dispose_without_start_is_no_op()
        {
            _sut.Dispose();

            Assert.Contains(_logger.LogEntries, e => e.EventId.Id == 103 && e.LogLevel == LogLevel.Trace);
        }

        [Fact]
        public async Task Startup_logs_informational_message()
        {
            await _sut.StartAsync(Token);

            Assert.Contains(_logger.LogEntries, e => e.EventId.Id == 101 && e.LogLevel == LogLevel.Information);
        }
    }
}

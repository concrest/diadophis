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
        public async Task Consumer_starts_receiving_messages_after_service_start()
        {
            
            await _sut.StartAsync(Token);

            // TODO: Assertions

        }

        [Fact]
        public async Task Consumer_callback_exceptions_are_logged()
        {
            await _sut.StartAsync(Token);

            Mock.Get(_channel).Raise(c => c.CallbackException += null, new CallbackExceptionEventArgs(new ArgumentNullException()));

            Assert.Contains(_logger.LogEntries, e => e.EventId.Id == 304 && e.LogLevel == LogLevel.Warning);
        }
    }
}

// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Diadophis.Fakes;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Diadophis.Kafka.Test
{
    public class LoggingTests
    {
        private readonly FakeLogger<string> _sut = new FakeLogger<string>();
        private static readonly EventId _eventId = new EventId(1);

        [Fact]
        public void Startup_logs_configured_information()
        {
            _sut.LogStart(_eventId, new FakeKafkaConfig());

            var entry = _sut.LogEntries.Single(l => l.LogLevel == LogLevel.Information && l.EventId.Id == 1);

            Assert.Equal("broker:9090", entry.GetValue("BrokerUrls"));
            Assert.Equal("consumer", entry.GetValue("ConsumerGroupId"));
            Assert.Equal("topic", ((string[])entry.GetValue("Topics"))[0]);
        }

        [Fact]
        public void Logging_kafka_message_has_message_information()
        {
            var message = new ConsumeResult<string, string>
            {
                Topic = "topic", 
                Message = new Message<string, string>
                {
                    Key = "key",
                    Value = "value"
                },
                Offset = 21
            };

            _sut.LogKafkaMessage(_eventId, "Test", message);

            var entry = _sut.LogEntries.Single(l => l.LogLevel == LogLevel.Debug && l.EventId.Id == 1);

            Assert.Equal("topic", entry.GetValue("Topic"));
            Assert.Equal("key", entry.GetValue("Key"));
            Assert.Equal("value", entry.GetValue("Value"));
            Assert.Equal(new Offset(21), entry.GetValue("Offset"));
        }

        [Fact]
        public void Logging_error_event_has_message_information()
        {
            var errorEvent = new ErrorEvent(new Error(ErrorCode.BrokerNotAvailable, "Nope"), true);

            _sut.LogErrorEvent(_eventId, errorEvent);

            var entry = _sut.LogEntries.Single(l => l.LogLevel == LogLevel.Error && l.EventId.Id == 1);

            Assert.Equal(ErrorCode.BrokerNotAvailable, entry.GetValue("Code"));
            Assert.Equal(true, entry.GetValue("IsFatal"));
            Assert.Equal("Nope", entry.GetValue("Reason"));
        }

        [Fact]
        public void Logging_kafka_exception_has_message_information()
        {
            var ex = new KafkaException(new Error(ErrorCode.BrokerNotAvailable, "Nope"));
            _sut.LogKafkaException(_eventId, ex);

            var entry = _sut.LogEntries.Single(l => l.LogLevel == LogLevel.Error && l.EventId.Id == 1);

            Assert.Equal(ErrorCode.BrokerNotAvailable, entry.GetValue("Code"));
            Assert.Equal("Nope", entry.GetValue("Reason"));
        }

        [Theory]
        [MemberData(nameof(NullLoggerCalls))]
        public void Extension_methods_on_null_logger_throw_exception(Action<ILogger> action)
        {
            ILogger logger = null;

            var exception = Assert.Throws<ArgumentNullException>(() => action(logger));
            Assert.Equal("logger", exception.ParamName);
        }

        public static IEnumerable<object[]> NullLoggerCalls()
        {
            yield return new object[] { new Action<ILogger>(l => l.LogStart<string, string>(_eventId, null)) };
            yield return new object[] { new Action<ILogger>(l => l.LogKafkaMessage<string, string>(_eventId, null, null)) };
            yield return new object[] { new Action<ILogger>(l => l.LogErrorEvent(_eventId, null)) };
            yield return new object[] { new Action<ILogger>(l => l.LogKafkaException(_eventId, null)) };
        }
    }
}

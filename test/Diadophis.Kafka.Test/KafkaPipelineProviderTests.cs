using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Diadophis.Kafka.Test
{
    public class KafkaPipelineProviderTests
    {
        private readonly KafkaPipelineProvider<string, string> _sut;

        private readonly FakeServiceProvider _serviceProvider;
        private readonly FakeLogger<KafkaPipelineProvider<string, string>> _logger;

        public KafkaPipelineProviderTests()
        {
            _serviceProvider = new FakeServiceProvider();
            _logger = new FakeLogger<KafkaPipelineProvider<string, string>>();

            _sut = new KafkaPipelineProvider<string, string>(_serviceProvider, _logger);
        }

        [Fact]
        public void Initialisation_errors_are_logged()
        {
            // There's no setup for IPipelineBuilder in IoC, so this should throw
            Assert.Throws<InvalidOperationException>(() => _sut.Initialise(Mock.Of<IKafkaConfig<string, string>>()));

            Assert.Contains(_logger.LogEntries, l => l.EventId.Id == 102 && l.LogLevel == LogLevel.Error);
        }

        [Fact]
        public async Task Invoking_pipeline_calls_configured_middleware()
        {
            TestKafkaConfig testConfig = InitialiseSut();

            var kafkaMessage = new ConsumeResult<string, string>();

            await _sut.InvokePipeline(kafkaMessage);

            var actual = testConfig.MessagesReceived.Single();

            Assert.Same(kafkaMessage, actual.GetKafkaMessage<string, string>());
        }

        private TestKafkaConfig InitialiseSut()
        {
            _serviceProvider.Container.Add(
                typeof(IPipelineBuilder),
                new PipelineBuilder(_serviceProvider, new FakeLogger<PipelineBuilder>())
            );

            var testConfig = new TestKafkaConfig();
            _sut.Initialise(testConfig);

            return testConfig;
        }
    }
}

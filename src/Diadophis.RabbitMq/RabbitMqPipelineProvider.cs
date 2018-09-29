using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    public class RabbitMqPipelineProvider : IRabbitMqPipelineProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqPipelineProvider> _logger;

        private MessageDelegate _pipeline;

        private static class LoggingEvents
        {
            // 100 series - Configuration and setup
            internal static readonly EventId Initialise = new EventId(101, nameof(Initialise));
            internal static readonly EventId InitialiseError = new EventId(102, nameof(InitialiseError));
        }

        public RabbitMqPipelineProvider(IServiceProvider serviceProvider,
            ILogger<RabbitMqPipelineProvider> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Initialise(IRabbitMqConfig config)
        {
            _logger.LogTrace(LoggingEvents.Initialise, "Initialising provider");

            try
            {
                var builder = _serviceProvider.GetRequiredService<IPipelineBuilder>();
                config.ConfigurePipeline(builder);
                _pipeline = builder.Build();
            }
            catch (Exception ex)
            {
                // Exception during pipeline building - likely due to missing IoC wire up

                _logger.LogError(
                    LoggingEvents.InitialiseError,
                    ex,
                    "Initialisation error"
                );

                // No recovery possible.
                throw;
            }
        }

        public Task InvokePipeline(BasicDeliverEventArgs message)
        {
            return _pipeline.Invoke(new RabbitMqMessageContext(_serviceProvider, message));
        }
    }
}

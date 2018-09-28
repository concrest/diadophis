using System;
using System.Threading.Tasks;
using Diadophis;
using Diadophis.RabbitMq;
using RabbitMQ.Client;

namespace SimpleRabbitMqService
{
    internal class SimpleRabbitMqConfig : IRabbitMqConfig
    {
        private const string ExchangeName = "my_exchange_name";
        private const string Type = ExchangeType.Topic;
        private const bool ExchangeDurable = true;
        private const bool ExchangeAutoDelete = false;

        private const bool QueueDurable = true;
        private const bool Exclusive = false;
        private const bool QueueAutoDelete = false;

        private const uint PrefetchSize = 0;
        private const ushort PrefetchCount = 10;

        private const string RoutingKey = "my_routing_key";

        
        public string ConnectionUri { get; set; }

        public string QueueName { get; } = "my_queue_name";

        public bool AutoAck => true;

        public void ConfigureChannel(IModel channel)
        {
            channel.ExchangeDeclare(ExchangeName, Type, ExchangeDurable, ExchangeAutoDelete, null);
            channel.QueueDeclare(QueueName, QueueDurable, Exclusive, QueueAutoDelete, null);

            channel.BasicQos(PrefetchSize, PrefetchCount, false);

            channel.QueueBind(QueueName, ExchangeName, RoutingKey);
        }

        public void ConfigurePipeline(IPipelineBuilder pipe)
        {
            // Middleware type example - terminal if the message is empty
            pipe.UseMiddleware<ExampleMiddleware>();

            // Raw delegate examples below, but recommendation is Middleware classes
            // since they support dependency injection

            // wrap the next components with this bit of middleware:
            pipe.Use(WrappingExample);

            // Optional terminal example - passes along if a condition passes
            pipe.Use(FilteringExample);

            // Run adds terminal middleware - won't pass on the context any further
            pipe.Run(TerminalExample);

            // Any middleware here (after the Run) will never be invoked!
        }

        private MessageDelegate WrappingExample(MessageDelegate next)
        {
            return context =>
            {
                        // Example of "wrapping" the call to the next component
                        // maybe for timing, exception handling, etc.
                        var someBeforeState = new object();
                try
                {
                    return next(context);
                }
                finally
                {
                    var someAfterState = new object();

                            // Do something with the before and after state here
                        }
            };
        }

        private MessageDelegate FilteringExample(MessageDelegate next)
        {
            // Example of optionally passing to the next component based on the context
            return context =>
            {
                var someCondition = false;
                if (someCondition) // if some check fails
                        {
                            // We don't invoke the next one
                            return Task.CompletedTask;
                }

                        // Could optionally set additional data on the context here

                        // Then pass on to the next
                        return next(context);
            };
        }

        private Task TerminalExample(MessageContext context)
        {
            // Example of middleware that terminates the pipeline
            // Normally use when no further processing is needed, or only 1 item of middleware is required
            return Task.CompletedTask;
        }
    }
}
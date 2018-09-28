using System.Threading.Tasks;
using Diadophis;
using Diadophis.RabbitMq;
using Microsoft.Extensions.Logging;

namespace SimpleRabbitMqService
{
    public class ExampleMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly ILogger<ExampleMiddleware> _logger;
        private readonly IConstructorDependency _dependency;

        /// <summary>
        /// Constructor for middleware must have the first argument be a MessageDelegate for the 
        /// next delegate to invoke.
        /// Other arguments will be created using Dependency Injection, so must be registered 
        /// with the IoC container as part of application startup
        /// </summary>
        /// <param name="next">The next delegate in the pipeline</param>
        /// <param name="dependency">A class level dependency, created once per middleware object</param>
        public ExampleMiddleware(MessageDelegate next, ILogger<ExampleMiddleware> logger, IConstructorDependency dependency)
        {
            _next = next;
            _logger = logger;
            _dependency = dependency;
        }

        /// <summary>
        /// First argument to the InvokeAsync method must be the MessageContext
        /// being passed along the pipeline.
        /// Othe arguments for this method are where it makes sense to create new objects
        /// per message rather than 1 object created for the constructor injection.
        /// As in constructor dependency inject, dependencies must be wired up with the IoC
        /// container as part of application startup
        /// </summary>
        /// <param name="context">The message context being passed along the pipeline</param>
        /// <param name="invokeDependency">A dependency to instantiate per message rather than per middleware object</param>
        /// <returns></returns>
        public Task InvokeAsync(MessageContext context, IInvokeDependency invokeDependency)
        {
            _logger.LogInformation("InvokeAsync called");

            // You can access the message from RabbitMQ here:
            var messageFromRabbit = context.GetRabbitMqMessage();

            // Add any processing, ETL, transforms, filtering etc.

            // Call some dependencies, maybe?
            _dependency.RunSomeMethod(invokeDependency.GetSomeValue());

            return _next(context);
        }
    }
}
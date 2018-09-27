using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleRabbitMqService
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseMyDependencies(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                // Wire up middleware dependencies.

                // Singletons are created once - the same object is used for each 
                services.AddSingleton<IConstructorDependency, ConstructorDependency>();

                // Transient services get created whenever they are needed
                services.AddTransient<IInvokeDependency, InvokeDependency>();
            });
        }
    }
}

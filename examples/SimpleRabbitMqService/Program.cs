using Diadophis.RabbitMq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SimpleRabbitMqService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseRabbitMqConsumer<SimpleRabbitMqConfig>(config =>
                {
                    config.Foo = 123;
                })
                .UseRabbitMqConsumer<SecondRabbitMqConfig>(config =>
                {
                    config.Foo = 79;
                });
    }
}

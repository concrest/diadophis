using Diadophis.RabbitMq;

namespace SimpleRabbitMqService
{
    internal class SimpleRabbitMqConfig : IRabbitMqConfig
    {
        public string Uri => "foo";

        public int Foo { get; set; } = 1;

        public override string ToString()
        {
            return $"Foo: {Foo}";
        }
    }
}
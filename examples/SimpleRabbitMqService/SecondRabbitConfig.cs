using Diadophis.RabbitMq;

namespace SimpleRabbitMqService
{
    internal class SecondRabbitMqConfig : IRabbitMqConfig
    {
        public string Uri => "bar";

        public int Foo { get; set; } = 1;

        public override string ToString()
        {
            return $"Foo: {Foo}";
        }
    }
}
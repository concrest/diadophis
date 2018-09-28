using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    public static class MessageContextExtensions
    {
        private static readonly string MessagePropertyName = typeof(BasicDeliverEventArgs).FullName;
        
        public static void SetRabbitMqMessage(this MessageContext context, BasicDeliverEventArgs message)
        {
            context.SetProperty(MessagePropertyName, message);
        }

        public static BasicDeliverEventArgs GetRabbitMqMessage(this MessageContext context)
        {
            return context.GetProperty<BasicDeliverEventArgs>(MessagePropertyName);
        }
    }
}

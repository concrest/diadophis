using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    public interface IRabbitMqPipelineProvider
    {
        void Initialise(IRabbitMqConfig config);
        Task InvokePipeline(BasicDeliverEventArgs message);
    }
}
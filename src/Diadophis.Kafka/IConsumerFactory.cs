using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IConsumerFactory
    {
        IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config);
    }
}

/*
 How about a new interface that exposes the IConsumer behaviour and is IDisposable, but would allow both 
 IConsumer and things like AvroSerdeProvider to be managed as one unit?  This factory could be responsible for
 creating that object.  Maybe that's a strategy rather than a factory.  
 
    IWrappedConsumer<Key,Value> IConsumerStrategy.CreateConsumer()?

 IKafkaConfig has an IConsumerStrategy<TKey, TValue> property, so the consumer service just calls that instead of 
 the current ConfigureConsumer.  CreateConsumer needs no arguments since the implementation of the interface 
 will deal with that.  In this way the interface would work for simple consumers, and Avro consumers.
 Would need to still figure out how to support multiple schemas per topic, but that will come later.
 */
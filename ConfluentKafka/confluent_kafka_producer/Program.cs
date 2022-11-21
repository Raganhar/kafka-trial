// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;
using shared_stuff;

var config = new ProducerConfig { BootstrapServers = ConstStrings.kafkaUrl };

// If serializers are not specified, default serializers from
// `Confluent.Kafka.Serializers` will be automatically used where
// available. Note: by default strings are encoded as UTF8.
using (var p = new ProducerBuilder<Null, string>(config).Build())
{
    try
    {
     
        foreach (var i in Enumerable.Range(0,100))
        {
            var dr = await p.ProduceAsync(ConstStrings.topic, new Message<Null, string> { Value = "test" });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");    
        }
    }
    catch (ProduceException<Null, string> e)
    {
        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
    }
}
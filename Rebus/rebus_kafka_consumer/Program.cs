using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rebus_kafka_consumer;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Kafka;
using Rebus.Routing.TypeBased;
using shared_stuff;


public class Program
{
    static void Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                {
                    services.AutoRegisterHandlersFromAssemblyOf<TestMessageHandler>();
                    services.AddRebus(x =>
                        x.Transport(t =>
                                t.UseKafka(ConstStrings.kafkaUrl, "scaleout.consumers", "commonGroupForScaleout"))
                            .Options(o => o.SetMaxParallelism(2))
                            .Logging(l => l.Console()));
                }
            )
            .Build();

        using var bus = host.Services.GetRequiredService<IBus>();
        bus.Subscribe<TestMessage>();
        Console.WriteLine(
            $"If your Kafka \"num.partitions\" > 1 to start the second instance of \"Scaleout.Consumers\"");
        Console.WriteLine("Waiting for messages. Press any key to exit.");
        Console.ReadKey();
        bus.Unsubscribe<TestMessage>().Wait(); // only for test
    }
}
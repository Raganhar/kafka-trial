// See https://aka.ms/new-console-template for more information

using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using shared_stuff;

public class Program
{
    public static async Task Main(string[] args)
    {
        string brokerList = ConstStrings.kafkaUrl; // args[0];
        string topicName = ConstStrings.topic; // args[1];
        await CreateTopic(brokerList, topicName);

        var config = new ProducerConfig { BootstrapServers = brokerList, Partitioner = Partitioner.ConsistentRandom, };

        using (var producer = new ProducerBuilder<string, string>(config)
                   .Build())
        {
            Console.WriteLine("\n-----------------------------------------------------------------------");
            Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
            Console.WriteLine("-----------------------------------------------------------------------");

            var cancelled = false;
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cancelled = true;
            };

            while (!cancelled)
            {
                try
                {
                    // Note: Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received (at the 
                    // expense of low throughput).
                    foreach (var i in Enumerable.Range(0, 50))
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topicName,
                            new Message<string, string>
                            {
                                Key = $"Key: {i}", Value = i.ToString(),
                                Headers = AddHeaders(new Dictionary<string, string> { { "headerName", "headerValue" } })
                            });

                        Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }

            // Since we are producing synchronously, at this point there will be no messages
            // in-flight and no delivery reports waiting to be acknowledged, so there is no
            // need to call producer.Flush before disposing the producer.
        }
    }

    private static Headers AddHeaders(Dictionary<string, string> headerName)
    {
        var addHeaders = new Headers();
        foreach (var header in headerName.Select(x => new Header(x.Key, Encoding.UTF8.GetBytes(x.Value))))
        {
            addHeaders.Add(header);
        }

        return addHeaders;
    }

    public static Dictionary<string, string> GetHeaders()
    {
        return null;
    }

    private static async Task CreateTopic(string brokerList, string topicName)
    {
        using (var adminClient =
               new AdminClientBuilder(new AdminClientConfig { BootstrapServers = brokerList }).Build())
        {
            await RecreateTopic(adminClient, topicName);
            await IncreasePartitionCountTo(adminClient, topicName, 4);
        }
    }

    private static async Task RecreateTopic(IAdminClient adminClient, string topicName)
    {
        try
        {
            await adminClient.DeleteTopicsAsync(new[] { topicName });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Thread.Sleep(1000);
        try
        {
            await adminClient.CreateTopicsAsync(new TopicSpecification[]
            {
                new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 3 }
            }, new CreateTopicsOptions { });
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }

    private static async Task IncreasePartitionCountTo(IAdminClient adminClient, string topicName, int count)
    {
        await adminClient.CreatePartitionsAsync(new List<PartitionsSpecification>
        {
            new PartitionsSpecification
            {
                Topic = topicName,
                IncreaseTo = count,
            }
        });
    }
}
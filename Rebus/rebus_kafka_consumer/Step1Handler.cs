using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using shared_stuff;

namespace rebus_kafka_consumer;
public class Step1Handler : IHandleMessages<Step1Event>
{
    public Step1Handler()
    {
    }
    public async Task Handle(Step1Event currentDateTime)
    {
        Console.WriteLine("Step1", currentDateTime.DateTime, currentDateTime.Counter);
        // _logger.LogInformation("The time is {0} for counter {1}", currentDateTime.DateTime, currentDateTime.Counter);
        await Task.Delay(5000);
        Console.WriteLine("Step1 finished", currentDateTime.DateTime, currentDateTime.Counter);
        // _logger.LogInformation("Finished with {0} for counter {1}", currentDateTime.DateTime, currentDateTime.Counter);
    }
}
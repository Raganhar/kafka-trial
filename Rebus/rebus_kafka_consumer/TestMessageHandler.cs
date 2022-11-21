using Rebus.Bus;
using Rebus.Handlers;
using shared_stuff;

namespace rebus_kafka_consumer;

public class TestMessageHandler : IHandleMessages<TestMessage>
{
    /// <inheritdoc />
    public async Task Handle(TestMessage evnt)
    {
        await Task.Delay(1000);
        Console.WriteLine($"Received Message : \"{evnt.MessageNumber}\" in thread {Thread.CurrentThread.ManagedThreadId}. Send Confirmation...");
        await _bus.Publish(new Confirmation { MessageNumber = evnt.MessageNumber });
    }

    public static int Amount;
    private readonly IBus _bus;
    public TestMessageHandler(IBus bus)
    {
        _bus = bus;
    }
}
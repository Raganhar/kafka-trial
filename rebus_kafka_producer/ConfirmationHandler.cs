using Rebus.Handlers;
using shared_stuff;

namespace rebus_kafka_producer;

class ConfirmationHandler : IHandleMessages<Confirmation>
{
    /// <inheritdoc />
    public async Task Handle(Confirmation evnt)
    {
        await Task.Delay(1000);
        Console.WriteLine($"Received confirmation for : \"{evnt.MessageNumber}\" in thread {Thread.CurrentThread.ManagedThreadId}");
        _counter.Add(evnt.MessageNumber);
    }

    private readonly Counter _counter;
    public ConfirmationHandler(Counter counter)
    {
        _counter = counter;
    }
}
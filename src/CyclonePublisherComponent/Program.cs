using CyclonePublisherComponent;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var publisher = new CycloneDdsPublisher(topicName: "SimpleTopic", domainId: 0);
using var subscriber = new CycloneDdsSubscriber(topicName: "SimpleTopic", domainId: 0);

Console.WriteLine("Running DDS publisher and subscriber on separate threads. Press Ctrl+C to stop.");

var publishTask = Task.Factory.StartNew(async () =>
{
    await publisher.PublishPeriodicallyAsync(
        "Hello from C# Cyclone DDS publisher",
        TimeSpan.FromSeconds(1),
        cts.Token);
}, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

var receiveTask = subscriber.ReceiveLoopAsync(
    message => Console.WriteLine($"[RECV] {message.TimestampUnixMs}: {message.Message}"),
    TimeSpan.FromMilliseconds(100),
    cts.Token);

try
{
    await Task.WhenAll(publishTask, receiveTask);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Stopping DDS sender/receiver threads.");
}

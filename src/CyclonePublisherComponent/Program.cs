using CyclonePublisherComponent;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var publisher = new CycloneDdsPublisher(topicName: "SimpleTopic", domainId: 0);
Console.WriteLine("Publishing to DDS topic 'SimpleTopic'. Press Ctrl+C to stop.");

try
{
    await publisher.PublishPeriodicallyAsync("Hello from C# Cyclone DDS publisher", TimeSpan.FromSeconds(1), cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Stopping publisher.");
}

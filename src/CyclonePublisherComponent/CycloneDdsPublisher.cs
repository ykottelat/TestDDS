using CycloneDDS;

namespace CyclonePublisherComponent;

/// <summary>
/// Publishes <see cref="SimpleMessage"/> samples to Cyclone DDS using the pjanec/CycloneDds.NET implementation.
/// </summary>
public sealed class CycloneDdsPublisher : IDisposable
{
    private readonly DomainParticipant _participant;
    private readonly Topic<SimpleMessage> _topic;
    private readonly Publisher _publisher;
    private readonly DataWriter<SimpleMessage> _writer;

    public CycloneDdsPublisher(string topicName = "SimpleTopic", int domainId = 0)
    {
        _participant = new DomainParticipant(domainId);
        _topic = new Topic<SimpleMessage>(_participant, topicName);
        _publisher = new Publisher(_participant);
        _writer = new DataWriter<SimpleMessage>(_publisher, _topic);
    }

    public void Publish(string message)
    {
        var sample = new SimpleMessage
        {
            Message = message,
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _writer.Write(sample);
    }

    public async Task PublishPeriodicallyAsync(
        string messagePrefix,
        TimeSpan interval,
        CancellationToken cancellationToken = default)
    {
        var i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            Publish($"{messagePrefix} #{++i}");
            await Task.Delay(interval, cancellationToken);
        }
    }

    public void Dispose()
    {
        _writer.Dispose();
        _publisher.Dispose();
        _topic.Dispose();
        _participant.Dispose();
    }
}

using DDS;

namespace CyclonePublisherComponent;

/// <summary>
/// Publishes <see cref="SimpleMessage"/> samples to Cyclone DDS.
/// </summary>
public sealed class CycloneDdsPublisher : IDisposable
{
    private readonly IDomainParticipant _participant;
    private readonly ITopic<SimpleMessage> _topic;
    private readonly IPublisher _publisher;
    private readonly IDataWriter<SimpleMessage> _writer;

    public CycloneDdsPublisher(string topicName = "SimpleTopic", int domainId = 0)
    {
        _participant = DomainParticipantFactory.Instance.CreateParticipant(domainId)
            ?? throw new InvalidOperationException("Unable to create DDS DomainParticipant.");

        _topic = _participant.CreateTopic<SimpleMessage>(topicName)
            ?? throw new InvalidOperationException($"Unable to create DDS topic '{topicName}'.");

        _publisher = _participant.CreatePublisher()
            ?? throw new InvalidOperationException("Unable to create DDS Publisher.");

        _writer = _publisher.CreateDataWriter(_topic)
            ?? throw new InvalidOperationException("Unable to create DDS DataWriter.");
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

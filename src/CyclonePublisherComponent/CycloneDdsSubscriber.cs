using CycloneDDS;

namespace CyclonePublisherComponent;

/// <summary>
/// Receives <see cref="SimpleMessage"/> samples from Cyclone DDS.
/// </summary>
public sealed class CycloneDdsSubscriber : IDisposable
{
    private readonly DomainParticipant _participant;
    private readonly Topic<SimpleMessage> _topic;
    private readonly Subscriber _subscriber;
    private readonly DataReader<SimpleMessage> _reader;

    public CycloneDdsSubscriber(string topicName = "SimpleTopic", int domainId = 0)
    {
        _participant = new DomainParticipant(domainId);
        _topic = new Topic<SimpleMessage>(_participant, topicName);
        _subscriber = new Subscriber(_participant);
        _reader = new DataReader<SimpleMessage>(_subscriber, _topic);
    }

    public Task ReceiveLoopAsync(
        Action<SimpleMessage> onMessage,
        TimeSpan pollInterval,
        CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var sample in _reader.Take())
                {
                    onMessage(sample);
                }

                Thread.Sleep(pollInterval);
            }
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Dispose()
    {
        _reader.Dispose();
        _subscriber.Dispose();
        _topic.Dispose();
        _participant.Dispose();
    }
}

namespace CyclonePublisherComponent;

/// <summary>
/// Simple topic payload published to the DDS bus.
/// </summary>
[Serializable]
public sealed class SimpleMessage
{
    public string Message { get; set; } = string.Empty;

    public long TimestampUnixMs { get; set; }
}

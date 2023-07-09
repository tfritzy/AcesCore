

using System.Text.Json.Serialization;

public abstract class Event
{
    [JsonPropertyName("type")]
    public abstract EventType Type { get; }
}
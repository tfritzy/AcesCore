

using System.Text.Json.Serialization;

namespace AcesCore
{
    public abstract class Event
    {
        [JsonPropertyName("type")]
        public abstract EventType Type { get; }
    }
}


using Newtonsoft.Json;

namespace AcesCore
{
    public class JoinGameEvent : Event
    {
        public override EventType Type => EventType.JoinGame;

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        public JoinGameEvent(string displayName)
        {
            DisplayName = displayName;
        }

        public JoinGameEvent()
        {
        }
    }
}
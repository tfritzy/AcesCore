using Newtonsoft.Json;

namespace AcesCore
{
    public class DrawFromDeckEvent : Event
    {
        public override EventType Type => EventType.DrawFromDeck;

        [JsonProperty("playerId")]
        public string PlayerId;

        public DrawFromDeckEvent(string playerId)
        {
            PlayerId = playerId;
        }

        public DrawFromDeckEvent()
        {
        }
    }
}
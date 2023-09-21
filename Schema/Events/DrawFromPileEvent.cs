using Newtonsoft.Json;

namespace AcesCore
{
    public class DrawFromPileEvent : Event
    {
        public override EventType Type => EventType.DrawFromPile;

        [JsonProperty("playerId")]
        public string PlayerId;

        public DrawFromPileEvent(string playerId)
        {
            PlayerId = playerId;
        }

        public DrawFromPileEvent()
        {
        }
    }
}
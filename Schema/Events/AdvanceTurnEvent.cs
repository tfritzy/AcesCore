using System.Text.Json.Serialization;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class AdvanceTurnEvent : Event
    {
        public override EventType Type => EventType.AdvanceTurn;

        [JsonProperty("turn")]
        public int Turn;

        public AdvanceTurnEvent(int turn)
        {
            Turn = turn;
        }

        public AdvanceTurnEvent()
        {
        }
    }
}
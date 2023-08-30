using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class AdvanceRoundEvent : Event
    {
        public override EventType Type => EventType.AdvanceRound;

        [JsonProperty("round")]
        public int Round;

        public AdvanceRoundEvent(int round)
        {
            Round = round;
        }

        public AdvanceRoundEvent()
        {
        }
    }
}

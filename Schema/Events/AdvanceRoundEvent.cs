using System.Text.Json.Serialization;
using AcesCore;

namespace AcesCore
{
    public class AdvanceRoundEvent : Event
    {
        public override EventType Type => EventType.AdvanceRound;

        public AdvanceRoundEvent()
        {
        }
    }
}

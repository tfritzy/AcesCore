using System.Text.Json.Serialization;
using AcesCore;

namespace AcesCore
{
    public class AdvanceTurnEvent : Event
    {
        public override EventType Type => EventType.AdvanceTurn;
    }
}
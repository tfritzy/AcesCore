using System.Text.Json.Serialization;
using AcesCore;

public class AdvanceRoundEvent : Event
{
    public override EventType Type => EventType.AdvanceRound;
}
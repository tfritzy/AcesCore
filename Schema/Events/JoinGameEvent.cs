

namespace AcesCore
{
    public class JoinGameEvent : Event
    {
        public override EventType Type => EventType.JoinGame;

        public string DisplayName { get; set; }

        public JoinGameEvent(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
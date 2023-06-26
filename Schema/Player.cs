using AcesCore;

namespace Schema
{
    public class Player
    {
        public string Id { get; private set; }
        public List<Card> Hand;

        public Player(string id)
        {
            Id = id;
            Hand = new();
        }
    }
}
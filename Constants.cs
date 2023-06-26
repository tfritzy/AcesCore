using AcesCore;

namespace AcesCore
{
    public static class Constants
    {
        public static readonly List<Card> FullDeck = Enum.GetValues(typeof(Card)).Cast<Card>().ToList();
    }
}

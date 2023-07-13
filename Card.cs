namespace AcesCore
{
    public class Card
    {
        public CardType Type;
        public int Deck;
        public Suit Suit
        {
            get
            {
                if ((int)Type > 0)
                {
                    return (Suit)(((int)Type - 1) / 13 + 1);
                }
                else
                {
                    return Suit.Invalid;
                }
            }
        }

        public CardValue Value
        {
            get
            {
                if ((int)Type > 0 && (int)Type < 53)
                {
                    return (CardValue)(((int)Type - 1) % 13 + 1);
                }
                else if ((int)Type >= 53)
                {
                    return CardValue.Joker;
                }
                else
                {
                    return CardValue.Invalid;
                }
            }
        }

        public Card(CardType type, int deck)
        {
            Type = type;
            Deck = deck;
        }
    }
}


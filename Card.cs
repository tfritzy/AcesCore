using System;
using System.Text.Json;
using AcesCore;
using Newtonsoft.Json;

namespace AcesCore
{
    public class Card
    {
        [JsonProperty("type")]
        public CardType Type;

        [JsonProperty("deck")]
        public int Deck;

        [JsonIgnore]
        public int Score => GetScore();

        [JsonIgnore]
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

        [JsonIgnore]
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

        public int GetScore()
        {
            return Value switch
            {
                CardValue.Two or
                CardValue.Three or
                CardValue.Four or
                CardValue.Five or
                CardValue.Six or
                CardValue.Seven or
                CardValue.Eight or
                CardValue.Nine or
                CardValue.Ten => (int)Value + 1,

                CardValue.Jack or
                CardValue.Queen or
                CardValue.King => 10,

                CardValue.Ace => 20,

                CardValue.Joker => 50,
                _ => 0,
            };
        }

        public Card(CardType type, int deck)
        {
            Type = type;
            Deck = deck;
        }

        public override string ToString()
        {
            return $"{Type} ({Deck})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Card card)
            {
                return card.Type == Type && card.Deck == Deck;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Deck);
        }

        public static bool operator ==(Card left, Card right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left == right);
        }
    }
}


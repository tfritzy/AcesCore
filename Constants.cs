using System;
using System.Collections.Generic;
using System.Linq;
using AcesCore;

namespace AcesCore
{
    public static class Constants
    {
        public static readonly List<CardType> FullDeck =
            Enum.GetValues(typeof(CardType)).Cast<CardType>().Where(c => c != CardType.INVALID).ToList();
    }
}

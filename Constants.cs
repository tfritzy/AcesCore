using System;
using System.Collections.Generic;
using System.Linq;
using AcesCore;

namespace AcesCore
{
    public static class Constants
    {
        public static readonly List<Card> FullDeck =
            Enum.GetValues(typeof(Card)).Cast<Card>().Where(c => c != Card.INVALID).ToList();
    }
}

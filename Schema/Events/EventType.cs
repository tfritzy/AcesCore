
namespace AcesCore
{
    public enum EventType
    {
        Invalid,
        JoinGame,
        StartGame,
        DrawFromDeck,
        DrawFromPile,
        Discard,
        AdvanceTurn,
        PlayerWentOut,
        AdvanceRound,
        PlayerDoneForRound,
        GameEndEvent
    }
}
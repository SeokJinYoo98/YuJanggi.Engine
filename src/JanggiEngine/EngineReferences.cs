#nullable enable

namespace YuJanggi.Engine.JanggiEngine
{
    using JanggiBoard;
    using JanggiRecord;
    using JanggiScore;
    using JanggiTurn;

    public sealed class EngineReferences : IReadOnlyEngine
    {
        public IReadOnlyGameEvents  GameEvents { get; }
        public IReadOnlyRecord      ReadOnlyRecord { get; }
 
        public IReadOnlyBoard       Board { get; }
        public IReadOnlyScore       Score { get; }
        public IReadOnlyTurn        Turn { get; }

        public EngineReferences(
            IReadOnlyGameEvents gameEvents,
            IReadOnlyRecord readOnlyRecord,
            IReadOnlyBoard board,
            IReadOnlyScore score,
            IReadOnlyTurn turn)
        {
            GameEvents      = gameEvents;
            ReadOnlyRecord  = readOnlyRecord;
            Board           = board;
            Score           = score;
            Turn            = turn;
        }
    }
}

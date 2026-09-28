using System;
namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IReadOnlyGameEvents
    {
        event Action<MoveContext>? OnPieceMoved;
        event Action<PlayerTeam>? OnCheckOccurred;
        event Action? OnCheckReleased;
        event Action<GameResultInfo>? OnGameEnded;
    }
    public interface IReadOnlyGameStateEvents
    {
        event Action<PlayerTeam>? OnTurnChanged;
        event Action<(PlayerTeam team, int time)>? OnTimeChanged;
        event Action<int, int>? OnRecordChanged;
        event Action<PlayerTeam, int>? OnScoreChanged;
    }

    internal class JanggiEngineEvents : IReadOnlyGameEvents, IReadOnlyGameStateEvents
    {
        public event Action<MoveContext>? OnPieceMoved;
        public event Action<PlayerTeam>? OnCheckOccurred;
        public event Action? OnCheckReleased;
        public event Action<GameResultInfo>? OnGameEnded;

        // 
        public event Action<PlayerTeam>? OnTurnChanged;
        public event Action<(PlayerTeam team, int time)>? OnTimeChanged;
        public event Action<int, int>? OnRecordChanged;
        public event Action<PlayerTeam, int>? OnScoreChanged;

        internal void PieceMoved(MoveContext ctx)
            => OnPieceMoved?.Invoke(ctx);
        internal void CheckOccurred(PlayerTeam team)
            => OnCheckOccurred?.Invoke(team);
        internal void CheckReleased()
            => OnCheckReleased?.Invoke();
        internal void GameEnded(GameResultInfo info)
            => OnGameEnded?.Invoke(info);
        internal void TurnChanged(PlayerTeam next)
            => OnTurnChanged?.Invoke(next);
        internal void TimeChanged((PlayerTeam team, int time) value)
            => OnTimeChanged?.Invoke(value);
        internal void RecordChanged(int current, int total)
            => OnRecordChanged?.Invoke(current, total);
        internal void ScoreChanged(PlayerTeam team, int score)
            => OnScoreChanged?.Invoke(team, score);
    }
}

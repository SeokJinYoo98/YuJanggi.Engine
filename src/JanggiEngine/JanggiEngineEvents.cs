using System;
namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IJanggiEngineEvents
    {
        event Action<MoveContext>?      OnPieceMoved;
        event Action<PlayerTeam>?       OnCheckOccurred;
        event Action?                   OnCheckReleased;
        event Action<GameResultInfo>?   OnGameEnded;
        event Action<PlayerTeam>?       OnTurnChanged;
    }
    internal class JanggiEngineEvents : IJanggiEngineEvents
    {
        public event Action<MoveContext>? OnPieceMoved;
        public event Action<PlayerTeam>? OnCheckOccurred;
        public event Action? OnCheckReleased;
        public event Action<GameResultInfo>? OnGameEnded;
        public event Action<PlayerTeam>? OnTurnChanged;
        public void PieceMoved(MoveContext ctx)
            => OnPieceMoved?.Invoke(ctx);
        public void CheckOccurred(PlayerTeam team)
            => OnCheckOccurred?.Invoke(team);
        public void CheckReleased()
            => OnCheckReleased?.Invoke();
        public void GameEnded(GameResultInfo info)
            => OnGameEnded?.Invoke(info);
        public void TurnChanged(PlayerTeam next)
            => OnTurnChanged?.Invoke(next);
    }
}

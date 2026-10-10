#nullable enable
using System;
namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IReadOnlyGameEvents
    {
        event Action<TurnData>? OnTurnCompleted;
    }
    public interface IReadOnlyGameStateEvents
    {

        event Action<(PlayerTeam team, int time)>? OnTimeChanged;
        event Action<int, int>?                    OnRecordChanged;
    }

    internal class JanggiEngineEvents : IReadOnlyGameEvents, IReadOnlyGameStateEvents
    {
        public event Action<TurnData>?                    OnTurnCompleted;
        public event Action<(PlayerTeam team, int time)>? OnTimeChanged;
        public event Action<int, int>?                    OnRecordChanged;

        internal void TurnCompleted(TurnData data)
            => OnTurnCompleted?.Invoke(data);
        internal void TimeChanged((PlayerTeam team, int time) value)
            => OnTimeChanged?.Invoke(value);
        internal void RecordChanged(int current, int total)
            => OnRecordChanged?.Invoke(current, total);
    }
}

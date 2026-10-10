#nullable enable
using System;
namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IReadOnlyGameEvents
    {
        event Action<TurnData>? OnTurnCompleted;
        event Action<UndoData>? OnUndoCompleted;
    }

    internal class JanggiEngineEvents : IReadOnlyGameEvents
    {
        public event Action<TurnData>? OnTurnCompleted;
        public event Action<UndoData>? OnUndoCompleted;

        internal void TurnCompleted(TurnData data)
            => OnTurnCompleted?.Invoke(data);

        internal void UndoCompleted(UndoData data)
            => OnUndoCompleted?.Invoke(data);
    }
}

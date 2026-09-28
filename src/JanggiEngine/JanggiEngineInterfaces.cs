using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using YuJanggi.Engine.JanggiRecord;
    public interface IJanggiEngine
        :   ISessionEngine,
            IControllerQuery
    {
        void UnBindEvents();
        void BindEvents();
        bool InitEngine();
        bool StartEngine();
        void Tick(float deltaTime);
    }
    public interface IReadOnlyEngine
    {
        IReadOnlyGameEvents GameEvents { get; }
        IReadOnlyGameStateEvents GameStateEvents { get; }
        IReadOnlyRecord Record { get; }
    }
    public interface ISessionEngine : IReadOnlyEngine
    {
        bool TryMove(Pos from, Pos to);
        void ToLiveRecord();
        void ToReplayRecord();
    }
    public interface IControllerQuery
    {
        PlayerTeam CurrentTurn { get; }

        bool IsValidPiece(PlayerTeam team, Pos pos, out int pieceNum);

        void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells);
    }
}

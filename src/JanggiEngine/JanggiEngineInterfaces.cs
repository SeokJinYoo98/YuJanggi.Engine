using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using YuJanggi.Engine.JanggiRecord;
    public interface IJanggiEngine
        :   ISessionQuery,
            IControllerQuery
    {
    }
    public interface IReadOnlyEngine
    {
        IReadOnlyGameEvents GameEvents { get; }
        IReadOnlyGameStateEvents GameStateEvents { get; }
        IReadOnlyRecord Record { get; }
    }
    public interface ISessionQuery : IReadOnlyEngine
    {
        bool InitEngine();
        bool StartEngine();
        bool TryMove(Pos from, Pos to);
        void Tick(float deltaTime);

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

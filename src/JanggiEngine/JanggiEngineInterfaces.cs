using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IReadonlyEngine : IControllerQuery
    {
        public IReadOnlyGameEvents      GameEvents { get; }
        public IReadOnlyGameStateEvents GameStateEvents { get; }
        public bool InitEngine();
        public bool StartEngine();
        public bool TryMove(Pos from, Pos to);
        public void Tick(float deltaTime);
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

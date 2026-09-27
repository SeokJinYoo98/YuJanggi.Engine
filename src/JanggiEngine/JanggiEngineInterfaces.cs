using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    public interface IJanggiEngine : IControllerQuery
    { 
        public bool InitEngine();
        public bool StartEngine();
        public bool TryMove(Pos from, Pos to);
        public void Tick(float deltaTime);
    }
    public interface IControllerQuery
    {
        PlayerTeam CurrentTurn { get; }

        bool IsValidPiece(Pos pos, PlayerTeam team);

        void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells);
    }
}

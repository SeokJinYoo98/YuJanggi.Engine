using System.Collections.Generic;

namespace YuJanggi.Core.JanggiEngine
{
    using Domain;
    public interface IJanggiEngine : IControllerQuery
    { 
        public bool InitEngine();
        public bool StartEngine();
    }
    public interface IControllerQuery
    {
        PlayerTeam CurrentTurn { get; }

        bool HasPiece(Pos pos);
        bool IsTeamPiece(Pos pos, PlayerTeam team);

        void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells);
    }
}

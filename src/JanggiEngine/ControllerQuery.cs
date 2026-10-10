using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using JanggiBoard;
    using JanggiRule;
    using JanggiTurn;

    internal sealed class ControllerQuery : IControllerQuery
    {
        private readonly JanggiBoard _board;
        private readonly JanggiRule _rule;
        private readonly JanggiTurn _turn;

        public PlayerTeam CurrentTurn => _turn.CurrentTeam;

        internal ControllerQuery(
            JanggiBoard board,
            JanggiRule rule,
            JanggiTurn turn)
        {
            _board = board;
            _rule = rule;
            _turn = turn;
        }

        public bool IsValidPiece(PlayerTeam team, Pos pos, out int pieceId)
        {
            pieceId = int.MinValue;

            if (!_board.IsInside(pos))
                return false;

            if (!_board.HasPiece(pos))
                return false;

            var piece = _board.GetPiece(pos);

            if (piece.Team != team)
                return false;

            pieceId = piece.Id;
            return true;
        }

        public void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells)
            => _rule.FindLegalMoves(from, legalCells, illegalCells);
    }
}

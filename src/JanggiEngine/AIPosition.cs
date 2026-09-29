#nullable enable
using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using JanggiBoard;
    using JanggiRule;

    internal sealed class AIPosition : IAIPosition
    {
        private readonly JanggiBoard _board;
        private readonly JanggiRule _rule;
        private readonly List<Pos> _legal = new();
        private readonly List<Pos> _illegal = new();

        public AIPosition(IReadOnlyBoard source)
        {
            _board = new JanggiBoard(source);
            _rule = new JanggiRule(_board);
        }

        public int Width => _board.WIDTH;
        public int Height => _board.HEIGHT;
        public PieceModel GetPiece(Pos pos) => _board.GetPiece(pos);
        public bool HasPiece(Pos pos) => _board.HasPiece(pos);
        public bool IsPalace(Pos pos) => _board.IsPalace(pos);
        public bool IsKingInCheck(PlayerTeam team) => _rule.IsKingInCheck(team);
        public MoveRecord DoMove(Pos from, Pos to) => _board.DoMove(from, to);
        public void UndoMove(in MoveRecord record) => _board.UndoMove(record);

        public void GetLegalMoves(PlayerTeam team, List<AIMove> moves)
        {
            moves.Clear();
            for (int x = 0; x < Width; ++x)
            {
                for (int z = 0; z < Height; ++z)
                {
                    var from = new Pos(x, z);
                    if (!HasPiece(from) || GetPiece(from).Team != team)
                        continue;

                    _rule.FindLegalMoves(from, _legal, _illegal);
                    foreach (var to in _legal)
                        moves.Add(new AIMove(from, to));
                }
            }
        }
    }
}

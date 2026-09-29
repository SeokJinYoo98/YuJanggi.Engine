#nullable enable
using System.Collections.Generic;
namespace YuJanggi.Engine.JanggiRule
{
    using JanggiBoard;
    using Domain;
    using Context;
    using Steps;
    /// <summary>재사용 작업 메모리로 이동 규칙을 계산합니다. 인스턴스는 동시 호출하지 않습니다.</summary>
    internal sealed class JanggiRule
    {
        private readonly MoveQueryContext   _moveContext;
        private readonly CandidatePipeline  _candidatePipeline;
        private readonly LegalMoveStep      _legalMoveStep;
        private readonly KingCheckDetector  _checkDetector;

        private readonly IJanggiBoard _board;
        public JanggiRule(IJanggiBoard board)
        {
            _board         = board;
            _moveContext   = new();
            _candidatePipeline = new CandidatePipeline();
            _checkDetector = new KingCheckDetector(_board, _candidatePipeline);
            _legalMoveStep = new LegalMoveStep(_checkDetector);

        }

        private void Evaluate(Pos from)
        {
            _moveContext.Reset(from);
            _candidatePipeline.Execute(_board, _moveContext);
            _legalMoveStep.Execute(_board, _moveContext);
        }

        public void FindLegalMoves(
            Pos from,
            List<Pos> legal,
            List<Pos> illegal)
        {
            Evaluate(from);

            legal.Clear();
            illegal.Clear();

            legal.AddRange(_moveContext.Legal);
            illegal.AddRange(_moveContext.Illegal);
        }

        public bool IsLegalMove(Pos from, Pos to)
        {
            Evaluate(from);
            return _moveContext.Legal.Contains(to);
        }

        public bool IsKingInCheck(PlayerTeam team)
            => _checkDetector.IsKingInCheck(team);

        public bool HasAnyLegalMove(PlayerTeam team)
        {
            for (int x = 0; x < _board.WIDTH; ++x)
            {
                for (int z = 0; z < _board.HEIGHT; ++z)
                {
                    var from = new Pos(x, z);
                    if (!_board.HasPiece(from) || _board.GetPiece(from).Team != team)
                        continue;

                    Evaluate(from);
                    if (_moveContext.Legal.Count > 0)
                        return true;
                }
            }
            return false;
        }
    }
}

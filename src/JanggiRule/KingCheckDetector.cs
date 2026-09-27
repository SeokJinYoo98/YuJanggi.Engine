
namespace YuJanggi.Engine.JanggiRule
{
    using JanggiBoard;
    using Domain;
    using Context;
    /// <summary>상대의 이동 후보로 장군을 검사합니다. 합법 수 판정으로 재귀하지 않습니다.</summary>
    internal sealed class KingCheckDetector
    {
        private readonly IJanggiBoard _board;
        private readonly CandidatePipeline _candidatePipeline;
        private readonly MoveQueryContext _attackContext = new();

        public KingCheckDetector(IJanggiBoard board, CandidatePipeline candidatePipeline)
        {
            _board = board;
            _candidatePipeline = candidatePipeline;
        }

        public bool IsKingInCheck(PlayerTeam team)
        {
            var kingPos = _board.GetKingPos(team);
            for (int x = 0; x < _board.WIDTH; ++x)
            {
                for (int z = 0; z < _board.HEIGHT; ++z)
                {
                    var from = new Pos(x, z);
                    if (!_board.HasPiece(from) || _board.GetPiece(from).Team == team)
                        continue;

                    _attackContext.Reset(from);
                    _candidatePipeline.Execute(_board, _attackContext);
                    if (_attackContext.Candidates.Contains(kingPos))
                        return true;
                }
            }
            return false;
        }
    }
}


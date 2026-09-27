namespace YuJanggi.Core.JanggiRule.Steps
{
    using Board;
    using Domain;
    using Context;
    using Movement;
    /// <summary>궁성 이동을 추가한 뒤 왕·사와 졸의 기존 후보 제한을 적용합니다.</summary>
    internal sealed class PalaceStep : IRuleStep
    {
        private readonly PalaceMovement _palaceMovement = new();

        public void Execute(IJanggiBoard board, MoveQueryContext context)
        {
            var from = context.From;
            var piece = board.GetPiece(from);
            var candidates = context.Candidates;

            switch (piece.Type)
            {
                case PieceType.Cannon:
                case PieceType.Chariot:
                case PieceType.Soldier:
                case PieceType.King:
                case PieceType.Guard:
                    if (board.IsPalace(from))
                        _palaceMovement.FindWays(board, from, candidates);
                    break;
            }

            // 역순 제거로 기존 후보 순서를 유지하고 조건식 클로저 할당을 피합니다.
            for (int i = candidates.Count - 1; i >= 0; --i)
            {
                var to = candidates[i];
                switch (piece.Type)
                {
                    case PieceType.Soldier:
                        if (piece.Team == PlayerTeam.Cho ? to.Z < from.Z : to.Z > from.Z)
                            candidates.RemoveAt(i);
                        break;
                    case PieceType.King:
                    case PieceType.Guard:
                        if (!board.IsPalace(to))
                            candidates.RemoveAt(i);
                        break;
                }
            }
        }
    }
}


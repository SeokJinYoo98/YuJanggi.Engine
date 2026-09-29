#nullable enable
namespace YuJanggi.Engine.JanggiRule.Steps
{
    using JanggiBoard;

    using Context;
    internal sealed class LegalMoveStep : IRuleStep
    {
        private readonly KingCheckDetector _checkDetector;

        public LegalMoveStep(KingCheckDetector checkDetector)
        {
            _checkDetector = checkDetector;
        }

        public void Execute(IJanggiBoard board, MoveQueryContext context)
        {
            var team = board.GetPiece(context.From).Team;
            foreach (var to in context.Candidates)
            {
                var record = board.DoMove(context.From, to);
                bool inCheck;
                try
                {
                    inCheck = _checkDetector.IsKingInCheck(team);
                }
                finally
                {
                    board.UndoMove(record);
                }

                if (inCheck)
                    context.Illegal.Add(to);
                else
                    context.Legal.Add(to);
            }
        }
    }
}


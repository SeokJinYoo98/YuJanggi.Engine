#nullable enable
using System.Collections.Generic;
namespace YuJanggi.Engine.JanggiRule.Movement
{
    using JanggiBoard;
    using Domain;

    internal class PatternMovement : Movement
    {

        protected readonly Step[][] _steps;
        protected PatternMovement(Step[][] steps)
        {
            _steps = steps;
        }
        internal override void FindWays(
            IJanggiBoard board,
            Pos from,
            List<Pos> buffer)
        {
            var piece = board.GetPiece(from);
            foreach (var steps in _steps)
                ProcessDirection(buffer, board, piece.Team, from, steps);
        }
        
        private void ProcessDirection(
            List<Pos> buffer,
            IJanggiBoard board,
            PlayerTeam team,
            Pos pos,
            Step[] steps)
        {
            int len = steps.Length;
            var dPos = pos;
            for (int j = 0; j < len; ++j)
            {
                dPos = ApplyStep(steps[j], team, dPos);

                if (j < len - 1)
                {
                    if (IsBlocked(board, team, dPos))
                        return;
                }
                else
                {
                    if (CanLand(board, team, dPos))
                        buffer.Add((dPos));
                }
            }
        }
        private bool IsBlocked(IJanggiBoard board, PlayerTeam team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return result != StepResult.Empty;
        }
        private bool CanLand(IJanggiBoard board, PlayerTeam team, Pos pos)
        {
            var result = CheckCell(board, team, pos);
            return (result == StepResult.Empty) || (result == StepResult.Enemy);
        }
    }
}

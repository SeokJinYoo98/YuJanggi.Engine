using System.Collections.Generic;

namespace YuJanggi.Core.JanggiRule.Movement
{
    using Board;
    using Domain;

    internal class ChariotMovement : Movement
    {
        //
        internal override void FindWays(IJanggiBoard board, Pos from, List<Pos> buffer)
        {
            var piece = board.GetPiece(from);
            var team = piece.Team;

            foreach (var step in _steps)
            {
                var dPos = from;
                while (true)
                {
                    dPos = ApplyStep(step, team, dPos);
                    var result = CheckCell(board, team, dPos);

                    if (result == StepResult.Block || result == StepResult.Team)
                        break;

                    buffer.Add(dPos);
                    if (result == StepResult.Enemy)
                        break;
                }
            }
        }
        //
        Step[] _steps = new Step[] { Step.Up, Step.Down, Step.Left, Step.Right };

    }
}
#nullable enable
using System.Collections.Generic;
namespace YuJanggi.Engine.JanggiRule.Movement
{
    using JanggiBoard;
    using Domain;

    internal enum Step
    { Right, Left, Up, Down, RightUp, RightDown, LeftUp, LeftDown }
    internal enum StepResult
    { Block, Empty, Enemy, Team }
    internal abstract class Movement
    {

        //
        internal abstract void FindWays(
            IJanggiBoard board,
            Pos from,
            List<Pos> buffer);
        //
        private int Forward(PlayerTeam team)
            => team == PlayerTeam.Cho? 1 : -1;
        protected static readonly Pos[] Dirs =
        {
            Pos.Right,
            Pos.Left,
            Pos.Up,
            Pos.Down,
            Pos.RightUp,
            Pos.RightDown,
            Pos.LeftUp,
            Pos.LeftDown
        };
        protected Pos ApplyStep(
            Step step,
            Pos pos)
            => pos + GetDir(step);
        protected Pos GetDir(Step step)
            => Dirs[(int)step];
        protected Pos ApplyStep(
            Step step,
            PlayerTeam team,
            Pos pos)
            => pos += GetDir(step, team);
        protected Pos GetDir(Step step, PlayerTeam team)
        {
            var dir = Dirs[(int)step];
            return new Pos(dir.X, dir.Z * Forward(team));
        }

        protected static StepResult CheckCell(
            IJanggiBoard board,
            PlayerTeam team,
            Pos pos)
        {
            if (!board.IsInside(pos))
                return StepResult.Block;

            if (!board.HasPiece(pos))
                return StepResult.Empty;

            if (board.GetPiece(pos).Team == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }
    }
}
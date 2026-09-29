#nullable enable
namespace YuJanggi.Engine.JanggiRule.Movement
{
    internal class HorseMovement : PatternMovement
    {
        internal HorseMovement()
            : base(new Step[][]
            {
                // Up
                new[] { Step.Up, Step.LeftUp },
                new[] { Step.Up, Step.RightUp },

                // Down
                new[] { Step.Down, Step.LeftDown },
                new[] { Step.Down, Step.RightDown },

                // Left
                new[] { Step.Left, Step.LeftUp },
                new[] { Step.Left, Step.LeftDown },

                // Right
                new[] { Step.Right, Step.RightUp },
                new[] { Step.Right, Step.RightDown }
            })
        {
        }
    }
}
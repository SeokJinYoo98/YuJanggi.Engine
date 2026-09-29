#nullable enable
namespace YuJanggi.Engine.JanggiRule.Movement
{
    internal class SoldierMovement : PatternMovement
    {
        internal SoldierMovement()
            : base(new Step[][]
            {
                new[] { Step.Up },
                new[] { Step.Left },
                new[] { Step.Right }
            })
        {
        }
    }
}
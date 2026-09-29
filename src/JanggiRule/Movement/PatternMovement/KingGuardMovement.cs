#nullable enable
namespace YuJanggi.Engine.JanggiRule.Movement
{
    internal class KingGuardMovement : PatternMovement
    {
        internal KingGuardMovement()
            : base(new Step[][]
            {
                new[] { Step.Up },
                new[] { Step.Down },
                new[] { Step.Left },
                new[] { Step.Right },
            })
        {
        }
    }
}

namespace YuJanggi.Engine.JanggiRule.Movement
{
    internal class KingGuardMovement : PatternMovement
    {
        internal KingGuardMovement()
        {
            _steps = new Step[][]
            {
                new [] { Step.Up },
                new [] { Step.Down },
                new [] { Step.Left },
                new [] { Step.Right },
            };
        }
    }
}

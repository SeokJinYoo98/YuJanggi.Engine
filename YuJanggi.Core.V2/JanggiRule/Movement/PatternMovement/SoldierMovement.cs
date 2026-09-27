namespace YuJanggi.Core.JanggiRule.Movement
{
    internal class SoldierMovement : PatternMovement
    {
        internal SoldierMovement()
        {
            _steps = new Step[][]
            {
                new Step[] { Step.Up },
                new Step[] { Step.Left },
                new Step[] { Step.Right }
            };
        }
    }
}
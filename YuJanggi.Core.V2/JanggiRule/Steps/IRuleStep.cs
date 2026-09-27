namespace YuJanggi.Core.JanggiRule.Steps
{
    using Board;
    using Context;
    internal interface IRuleStep
    {
        void Execute(IJanggiBoard board, MoveQueryContext context);
    }
}


#nullable enable
namespace YuJanggi.Engine.JanggiRule.Steps
{
    using JanggiBoard;
    using Context;
    internal interface IRuleStep
    {
        void Execute(IJanggiBoard board, MoveQueryContext context);
    }
}


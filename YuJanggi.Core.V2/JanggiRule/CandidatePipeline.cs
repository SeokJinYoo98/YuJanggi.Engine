namespace YuJanggi.Core.JanggiRule
{
    using Board;
    using Context;
    using Steps;

    /// <summary>기본 이동과 궁성 규칙으로 후보만 생성합니다. 합법성은 판정하지 않습니다.</summary>
    internal sealed class CandidatePipeline
    {
        private readonly MovementStep _movementStep = new();
        private readonly PalaceStep _palaceStep = new();

        public void Execute(IJanggiBoard board, MoveQueryContext context)
        {
            _movementStep.Execute(board, context);
            _palaceStep.Execute(board, context);
        }
    }
}

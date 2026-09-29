#nullable enable
namespace YuJanggi.Engine.JanggiOption
{
    using Domain;
    public sealed record JanggiOptions
    {
        public GameModeType GameMode { get; init; }

        public PlayerType PlayerCho { get; init; }
        public Formation ChoFormation { get; init; } = Formation.EHHE;

        public PlayerType PlayerHan { get; init; }
        public Formation HanFormation { get; init; } = Formation.EHHE;

        public float TurnTime { get; init; } = 0f;
        public int Width { get; init; } = 9;
        public int Height { get; init; } = 10;
    }

}

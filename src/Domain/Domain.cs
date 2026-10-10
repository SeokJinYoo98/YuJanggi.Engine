#nullable enable
using System;
using YuJanggi.Engine.JanggiBoard;

namespace YuJanggi.Engine.Domain
{


    public enum Formation { HEHE, EHEH, EHHE, HEEH }
    public enum PlayerType { Local, AI, Network, Remote}
    public enum GameModeType { Local, AI, Network }
    public enum PlayerTeam{ Cho, Han, None }
    public enum PieceType
    {
        King,       // 궁
        Chariot,    // 차
        Cannon,     // 포
        Horse,      // 마
        Elephant,   // 상
        Guard,      // 사
        Soldier,    // 졸/병
        None
    }
    public readonly struct Pos : IEquatable<Pos>
    {
        public Pos(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }
        public readonly int X;
        public readonly int Z;

        public static Pos operator +(Pos a, Pos b)
            => new Pos(a.X + b.X, a.Z + b.Z);
        public static Pos operator -(Pos a, Pos b)
            => new Pos(a.X - b.X, a.Z - b.Z);
        public static bool operator ==(Pos a, Pos b)
            => a.X == b.X && a.Z == b.Z;
        public static bool operator !=(Pos a, Pos b)
            => !(a == b);
        public bool Equals(Pos other)
            => X == other.X && Z == other.Z;
        public override bool Equals(object? obj)
            => obj is Pos other && Equals(other);
        public override int GetHashCode()
            => HashCode.Combine(X, Z);
        public override string ToString()
        {
            return $"({X}, {Z})";
        }
        public static readonly Pos Up = new Pos(+0, +1);
        public static readonly Pos Down = new Pos(+0, -1);
        public static readonly Pos Left = new Pos(-1, +0);
        public static readonly Pos Right = new Pos(+1, +0);
        public static readonly Pos LeftUp = new Pos(-1, +1);
        public static readonly Pos RightUp = new Pos(+1, +1);
        public static readonly Pos LeftDown = new Pos(-1, -1);
        public static readonly Pos RightDown = new Pos(+1, -1);
        public static readonly Pos Invalid = new Pos(-100, -100);
    }

    public sealed record MoveRecord
    {
        public PieceModel   MovedPiece      { get; init; }
        public PieceModel   CapturedPiece   { get; init; }
        public Pos          From            { get; init; }
        public Pos          To              { get; init; }
        public bool IsCaptured 
            => !CapturedPiece.IsNone;
    }
    public enum GameResult { Draw, CheckMate, GiveUp,  Score }
    public struct GameResultInfo
    {
        public GameResult Type;
        public PlayerTeam Loser;
        public PlayerTeam Winner;
    }
    public sealed record CheckRecord
    {
        public PlayerTeam? CheckedTeam { get; init; }
        public PlayerTeam? CheckReleasedTeam { get; init; }
    }
    public sealed record GameInfo
    {
        public GameModeType Mode { get; init; }
        public Formation Formation { get; init; }
        public PlayerType ChoPlayerType { get; init; }
        public PlayerType HanPlayerType { get; init; }
    }
    public sealed record TurnData
    {
        public PlayerTeam           ActingTeam          { get; init; }
        public int                  MoveCount           { get; init; }
        public (int Cho, int Han)   Score               { get; init; }
        public int                  TotalTurn           { get; init; }
        public MoveRecord?          MovedRecord         { get; init; }
        public PlayerTeam?          CheckedTeam         { get; init; }
        public PlayerTeam?          CheckReleasedTeam   { get; init; }
        public GameResultInfo?      GameResult          { get; init; }
    }
    public sealed record UndoData
    {
        public MoveRecord?          UndoneMove  { get; init; }
        public PlayerTeam           CurrentTurn { get; init; }
        public (int Cho, int Han)   Score       { get; init; }
        public int                  RecordCount { get; init; }
        public PlayerTeam?          CheckedTeam { get; init; }
        public PlayerTeam?          CheckReleasedTeam { get; init; }
    }
}

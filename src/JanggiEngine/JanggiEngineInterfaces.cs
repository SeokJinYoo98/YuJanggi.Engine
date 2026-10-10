#nullable enable
using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using JanggiBoard;  
    using JanggiRecord;
    using JanggiScore;
    using JanggiTurn;

    public interface IJanggiEngine
        :   IGameEngine,
            IAIPositionSource
    {
        IReadOnlyEngine References { get; }
        IReplayRecord ReplayRecord { get; }
        void UnBindEvents();
        void BindEvents();
    }
    public interface IReadOnlyEngine
    {
        public IReadOnlyGameEvents  GameEvents      { get; }
        public IReadOnlyRecord      ReadOnlyRecord  { get; }
        public IReadOnlyBoard       Board           { get; }
        public IReadOnlyScore       Score           { get; }
        public IReadOnlyTurn        Turn            { get; }
    }
    public interface IGameEngine
    {
        IControllerQuery ControllerQuery { get; }
        bool InitEngine();
        bool StartEngine();
        void Tick(float deltaTime);
        bool TryProcessTurn(Pos from, Pos to);
        void Undo();
        void GiveUp();
        void HandleHandicap();
    }
    public interface IControllerQuery
    {
        PlayerTeam CurrentTurn { get; }

        bool IsValidPiece(PlayerTeam team, Pos pos, out int pieceNum);

        void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells);
    }

    public readonly struct AIMove
    {
        public AIMove(Pos from, Pos to) { From = from; To = to; }
        public Pos From { get; }
        public Pos To { get; }
    }

    // A search position is a private copy of the live board. Only Engine applies rules to it.
    public interface IAIPositionSource
    {
        IAIPosition CreateAIPosition();
    }

    public interface IAIPosition
    {
        int Width { get; }
        int Height { get; }
        PieceModel GetPiece(Pos pos);
        bool HasPiece(Pos pos);
        bool IsPalace(Pos pos);
        bool IsKingInCheck(PlayerTeam team);
        void GetLegalMoves(PlayerTeam team, List<AIMove> moves);
        MoveRecord DoMove(Pos from, Pos to);
        void UndoMove(in MoveRecord record);
    }
}

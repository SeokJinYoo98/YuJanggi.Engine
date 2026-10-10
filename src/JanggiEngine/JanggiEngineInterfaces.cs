#nullable enable
using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using Domain;
    using JanggiRecord;
    using JanggiBoard;  

    public interface IJanggiEngine
        :   ISessionEngine,
            IAIPositionSource
    {
        IControllerQuery ControllerQuery { get; }
        void UnBindEvents();
        void BindEvents();
        bool InitEngine();
        bool StartEngine();
        void Tick(float deltaTime);
    }
    public interface IReadOnlyEngine
    {
        IReadOnlyGameEvents GameEvents { get; }
        IReadOnlyGameStateEvents GameStateEvents { get; }
        IReadOnlyRecord Record { get; }
        IReadOnlyBoard Board { get; }
    }
    public interface ISessionEngine : IReadOnlyEngine
    {
        bool TryProcessTurn(Pos from, Pos to);
        void ToLiveRecord();
        void ToReplayRecord();
        bool TryUnDo(out UndoData undoData);
        void GiveUp();
        void Handicap();
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

using System;
using System.Collections.Generic;

namespace YuJanggi.Core.Domain
{
    using Board;
    using Match;
    public interface IMatchEvents
    {
        event Action<MoveContext>? OnPieceMoved;
        event Action<PlayerTeam>? OnCheckOccurred;
        event Action? OnCheckReleased;
        event Action<GameResultInfo>? OnGameEnded;
        event Action<PlayerTeam>? OnTurnChanged;
    }
    public interface IMatchControllerQuery
    {
        PlayerTeam PlayerTurn { get; }

        bool HasPiece(Pos pos);
        bool IsTeamPiece(Pos pos, PlayerTeam team);

        void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells);
    }
    public interface IMatchSessionQuery
    {
        IMatchEvents MatchEvent { get; }

        PlayerTeam PlayerTurn { get; }
        int RecordCount { get; }

        bool IsGameEnded { get; }
    }
    public interface IMatchSessionCommand
    {
        void InitGame(Formation cho, Formation han);
        void StartGame();

        bool TryMove(Pos from, Pos to);
        bool TryUndo(out MoveContext context);

        void GiveUp();
        void Handicap();

        void Tick(float deltaTime);
    }
    public interface IMatchViewQuery
    {
        IMatchEvents MatchEvent { get; }

        PlayerTeam PlayerTurn { get; }
        int RecordCount { get; }

        int ChoScore { get; }
        int HanScore { get; }
    }
}

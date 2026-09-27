using YuJanggi.Core.V2.Board;
using YuJanggi.Core.V2.Domain;
using YuJanggi.Core.V2.Match;
using YuJanggi.Core.V2.Rule;

namespace YuJanggiCore.Tests;

internal static class JanggiTestBoard
{
    public static JanggiBoard CreateBoardWithKings()
    {
        var board = new JanggiBoard();
        board.ResetBoard();
        board.SetPiece(new Pos(4, 1), Piece(PieceType.King, PlayerTeam.Cho));
        board.SetPiece(new Pos(4, 8), Piece(PieceType.King, PlayerTeam.Han));
        return board;
    }

    public static MatchModel CreateEmptyMatch()
    {
        var match = new MatchModel(
            new JanggiTurn(0),
            new JanggiRecord(),
            new JanggiScore(),
            new JanggiBoard(),
            new JanggiRule());

        match.Board.ResetBoard();
        match.Board.SetPiece(new Pos(4, 1), Piece(PieceType.King, PlayerTeam.Cho));
        match.Board.SetPiece(new Pos(4, 8), Piece(PieceType.King, PlayerTeam.Han));
        match.StartGame();
        return match;
    }

    public static PieceModel Piece(PieceType type, PlayerTeam team, int id = 0)
        => new(type, team, id);

    public static void MoveChoKing(JanggiBoard board, Pos to)
        => board.DoMove(new Pos(4, 1), to);
}

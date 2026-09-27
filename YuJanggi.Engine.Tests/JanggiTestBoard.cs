using YuJanggi.Engine.JanggiBoard;
using YuJanggi.Engine.Domain;
using Board = YuJanggi.Engine.JanggiBoard.JanggiBoard;

namespace YuJanggi.Engine.Tests;

internal static class JanggiTestBoard
{
    public static Board CreateBoardWithKings()
    {
        var board = new Board();
        board.ResetBoard();
        board.SetPiece(new Pos(4, 1), Piece(PieceType.King, PlayerTeam.Cho));
        board.SetPiece(new Pos(4, 8), Piece(PieceType.King, PlayerTeam.Han));
        return board;
    }

    public static PieceModel Piece(PieceType type, PlayerTeam team, int id = 0)
        => new(type, team, id);

    public static void MoveChoKing(Board board, Pos to)
        => board.DoMove(new Pos(4, 1), to);
}
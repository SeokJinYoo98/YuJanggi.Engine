using System.Reflection;
using YuJanggi.Engine.Domain;
using EngineModel = YuJanggi.Engine.JanggiEngine.JanggiEngine;
using Board = YuJanggi.Engine.JanggiBoard.JanggiBoard;
using Record = YuJanggi.Engine.JanggiRecord.JanggiRecord;
using Score = YuJanggi.Engine.JanggiScore.JanggiScore;
using Turn = YuJanggi.Engine.JanggiTurn.JanggiTurn;

namespace YuJanggi.Engine.Tests;

// Engine이 상태 조회 API를 노출하지 않아 테스트의 배치와 관찰에만 reflection을 사용합니다.
// 이동, 무르기, 한 수 쉼은 실제 Engine 메서드로 실행합니다.
internal sealed class JanggiEngineFixture
{
    public EngineModel Engine { get; }
    public Board Board { get; }
    public Record Record { get; }
    public Score Score { get; }
    public Turn Turn { get; }

    private JanggiEngineFixture()
    {
        Engine = new EngineModel(new JanggiOptions(turnTime: 30f));
        Board = ReadField<Board>("_janggiBoard");
        Record = ReadField<Record>("_janggiRecord");
        Score = ReadField<Score>("_janggiScore");
        Turn = ReadField<Turn>("_janggiTurn");
        Board.ResetBoard();
        Board.SetPiece(new Pos(4, 1), JanggiTestBoard.Piece(PieceType.King, PlayerTeam.Cho));
        Board.SetPiece(new Pos(4, 8), JanggiTestBoard.Piece(PieceType.King, PlayerTeam.Han));
        Engine.StartEngine();
    }

    public static JanggiEngineFixture Create() => new();

    private T ReadField<T>(string name) where T : class
        => typeof(EngineModel).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(Engine) as T
            ?? throw new InvalidOperationException($"Engine 테스트 상태를 찾을 수 없습니다: {name}");
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YuJanggi.Engine.Domain;

namespace YuJanggi.Engine.Tests;

[TestClass]
public class JanggiEngineTests
{
    [TestMethod]
    public void LegalMove_CapturesEnemyUpdatesScoreRecordAndChangesTurn()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 6);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        match.Board.SetPiece(to, JanggiTestBoard.Piece(PieceType.Soldier, PlayerTeam.Han));
        var hanScore = -1;
        match.Score.OnScoreChanged += (team, score) =>
        {
            if (team == PlayerTeam.Han)
                hanScore = score;
        };
        TurnData? completed = null;
        match.Engine.GameEvents.OnTurnCompleted += data => completed = data;

        // Act
        var moved = match.Engine.TryProcessTurn(from, to);

        // Assert
        Assert.IsTrue(moved);
        Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(to).Type);
        Assert.AreEqual(PlayerTeam.Cho, match.Board.GetPiece(to).Team);
        Assert.IsTrue(match.Board.GetPiece(from).IsNone);
        Assert.AreEqual(PlayerTeam.Han, match.Turn.CurrentTeam);
        Assert.AreEqual(1, match.Record.Count);
        Assert.AreEqual(70, hanScore);
        Assert.IsNotNull(completed);
        Assert.AreEqual(PlayerTeam.Cho, completed.ActingTeam);
        Assert.AreEqual((72, 70), completed.Score);
        Assert.AreEqual(1, completed.MoveCount);
        Assert.IsNotNull(completed.MovedRecord);
        Assert.AreEqual(from, completed.MovedRecord.From);
        Assert.AreEqual(to, completed.MovedRecord.To);
        Assert.IsTrue(completed.MovedRecord.IsCaptured);
    }

    [TestMethod]
    public void IllegalMove_LeavesBoardTurnScoreAndRecordUnchanged()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(1, 4);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        var hanScoreChanged = false;
        match.Score.OnScoreChanged += (team, _) => hanScoreChanged |= team == PlayerTeam.Han;

        // Act
        var moved = match.Engine.TryProcessTurn(from, to);

        // Assert
        Assert.IsFalse(moved);
        Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(from).Type);
        Assert.IsTrue(match.Board.GetPiece(to).IsNone);
        Assert.AreEqual(PlayerTeam.Cho, match.Turn.CurrentTeam);
        Assert.AreEqual(0, match.Record.Count);
        Assert.IsFalse(hanScoreChanged);
    }

    [TestMethod]
    public void MoveByOpponentDuringChoTurn_IsRejectedAndStateIsPreserved()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 6);
        var to = new Pos(0, 5);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Han));
        match.Board.SetPiece(to, JanggiTestBoard.Piece(PieceType.Soldier, PlayerTeam.Cho));
        var choScoreChanged = false;
        match.Score.OnScoreChanged += (team, _) => choScoreChanged |= team == PlayerTeam.Cho;

        // Act
        var moved = match.Engine.TryProcessTurn(from, to);

        // Assert
        Assert.IsFalse(moved);
        Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(from).Type);
        Assert.AreEqual(PieceType.Soldier, match.Board.GetPiece(to).Type);
        Assert.AreEqual(PlayerTeam.Cho, match.Board.GetPiece(to).Team);
        Assert.AreEqual(PlayerTeam.Cho, match.Turn.CurrentTeam);
        Assert.AreEqual(0, match.Record.Count);
        Assert.IsFalse(choScoreChanged);
    }

    [TestMethod]
    public void MoveThatDoesNotResolveCheck_IsRejectedAndStateIsPreserved()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 4);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        match.Board.SetPiece(new Pos(4, 5), JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Han));

        // Act
        var moved = match.Engine.TryProcessTurn(from, to);

        // Assert
        Assert.IsFalse(moved);
        Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(from).Type);
        Assert.IsTrue(match.Board.GetPiece(to).IsNone);
        Assert.AreEqual(PlayerTeam.Cho, match.Turn.CurrentTeam);
        Assert.AreEqual(0, match.Record.Count);
    }

    [TestMethod]
    public void CheckingMove_RecordsJanggun()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(4, 3);
        var to = new Pos(4, 7);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));

        // Act
        var moved = match.Engine.TryProcessTurn(from, to);
        var hasRecord = match.Record.TryPeek(out var context);

        // Assert
        Assert.IsTrue(moved);
        Assert.IsTrue(hasRecord);
        Assert.IsNotNull(context);
        Assert.AreEqual<PlayerTeam?>(PlayerTeam.Han, context.CheckedTeam);
        Assert.AreEqual(PlayerTeam.Han, match.Turn.CurrentTeam);
    }

    [TestMethod]
    public void UndoAfterCapture_RestoresBoardTurnScoreAndRecord()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 6);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        match.Board.SetPiece(to, JanggiTestBoard.Piece(PieceType.Soldier, PlayerTeam.Han, 7));
        var hanScore = -1;
        match.Score.OnScoreChanged += (team, score) =>
        {
            if (team == PlayerTeam.Han)
                hanScore = score;
        };
        Assert.IsTrue(match.Engine.TryProcessTurn(from, to));

        // Act
        var undone = match.Engine.TryUnDo(out var context);

        // Assert
        Assert.IsTrue(undone);
        Assert.IsNotNull(context.UndoneMove);
        Assert.IsTrue(context.UndoneMove.IsCaptured);
        Assert.AreEqual((72, 72), context.Score);
        Assert.AreEqual(PlayerTeam.Cho, context.CurrentTurn);
        Assert.AreEqual(0, context.RecordCount);
        Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(from).Type);
        Assert.AreEqual(PieceType.Soldier, match.Board.GetPiece(to).Type);
        Assert.AreEqual(PlayerTeam.Han, match.Board.GetPiece(to).Team);
        Assert.AreEqual(PlayerTeam.Cho, match.Turn.CurrentTeam);
        Assert.AreEqual(0, match.Record.Count);
        Assert.AreEqual(72, hanScore);
    }

    [TestMethod]
    public void Handicap_RecordsPassAndChangesTurnWithoutChangingBoard()
    {
        // Arrange
        var match = JanggiEngineFixture.Create();
        var choKingPosition = match.Board.GetKingPos(PlayerTeam.Cho);
        var hanKingPosition = match.Board.GetKingPos(PlayerTeam.Han);

        // Act
        match.Engine.HandleHandicap();
        var hasRecord = match.Record.TryPeek(out var context);

        // Assert
        Assert.IsTrue(hasRecord);
        Assert.IsNotNull(context);
        Assert.IsNull(context.MovedRecord);
        Assert.AreEqual(PlayerTeam.Cho, context.ActingTeam);
        Assert.AreEqual(PlayerTeam.Han, match.Turn.CurrentTeam);
        Assert.AreEqual(1, match.Record.Count);
        Assert.AreEqual(choKingPosition, match.Board.GetKingPos(PlayerTeam.Cho));
        Assert.AreEqual(hanKingPosition, match.Board.GetKingPos(PlayerTeam.Han));
    }

    [TestMethod]
    public void BindEventsTwice_TimeoutCompletesOnlyOneTurn()
    {
        var match = JanggiEngineFixture.Create();
        int completed = 0;
        match.Engine.GameEvents.OnTurnCompleted += _ => completed++;
        match.Engine.BindEvents();
        match.Engine.BindEvents();

        match.Engine.Tick(30f);

        Assert.AreEqual(1, completed);
        Assert.AreEqual(1, match.Record.Count);
        Assert.AreEqual(PlayerTeam.Han, match.Turn.CurrentTeam);
        match.Engine.UnBindEvents();
    }

    [TestMethod]
    public void ControllerQuery_ReadsLiveTurnAndLegalMovesWithoutChangingBoard()
    {
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 4);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho, 7));
        var query = match.Engine.ControllerQuery;
        var legal = new List<Pos>();
        var illegal = new List<Pos>();

        Assert.IsTrue(query.IsValidPiece(PlayerTeam.Cho, from, out var id));
        Assert.AreEqual(7, id);
        Assert.IsFalse(query.IsValidPiece(PlayerTeam.Han, from, out _));
        query.GetMovableCells(from, legal, illegal);
        CollectionAssert.Contains(legal, to);
        Assert.AreEqual(7, match.Board.GetPiece(from).Id);
        Assert.IsTrue(match.Board.GetPiece(to).IsNone);
        Assert.AreEqual(PlayerTeam.Cho, query.CurrentTurn);
        match.Engine.Handicap();
        Assert.AreEqual(PlayerTeam.Han, query.CurrentTurn);
    }

    [TestMethod]
    public void UndoPass_ReturnsRestoredStateWithoutMovingPieces()
    {
        var match = JanggiEngineFixture.Create();
        match.Engine.Handicap();

        Assert.IsTrue(match.Engine.TryUnDo(out var data));

        Assert.IsNull(data.UndoneMove);
        Assert.AreEqual(PlayerTeam.Cho, data.CurrentTurn);
        Assert.AreEqual((72, 72), data.Score);
        Assert.AreEqual(0, data.RecordCount);
        Assert.AreEqual(PieceType.King, match.Board.GetPiece(new Pos(4, 1)).Type);
        Assert.AreEqual(PieceType.King, match.Board.GetPiece(new Pos(4, 8)).Type);
    }

    [TestMethod]
    public void MoveNotifications_ObserveCompletedBoardScoreTurnAndRecord()
    {
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 6);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        match.Board.SetPiece(to, JanggiTestBoard.Piece(PieceType.Soldier, PlayerTeam.Han));
        match.Engine.BindEvents();
        int notifications = 0;

        void AssertCompletedState()
        {
            notifications++;
            Assert.IsTrue(match.Board.GetPiece(from).IsNone);
            Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(to).Type);
            Assert.AreEqual((72, 70), match.Score.Score);
            Assert.AreEqual(PlayerTeam.Han, match.Turn.CurrentTeam);
            Assert.AreEqual(1, match.Record.Count);
        }

        match.Engine.GameStateEvents.OnTimeChanged += _ => AssertCompletedState();
        match.Engine.GameStateEvents.OnRecordChanged += (_, _) => AssertCompletedState();
        match.Engine.GameEvents.OnTurnCompleted += _ => AssertCompletedState();

        Assert.IsTrue(match.Engine.TryProcessTurn(from, to));
        Assert.AreEqual(4, notifications);
    }

    [TestMethod]
    public void UndoNotifications_ObserveRestoredBoardScoreTurnAndRecord()
    {
        var match = JanggiEngineFixture.Create();
        var from = new Pos(0, 3);
        var to = new Pos(0, 6);
        match.Board.SetPiece(from, JanggiTestBoard.Piece(PieceType.Chariot, PlayerTeam.Cho));
        match.Board.SetPiece(to, JanggiTestBoard.Piece(PieceType.Soldier, PlayerTeam.Han));
        Assert.IsTrue(match.Engine.TryProcessTurn(from, to));
        match.Engine.BindEvents();
        int notifications = 0;
        int completed = 0;

        void AssertRestoredState()
        {
            notifications++;
            Assert.AreEqual(PieceType.Chariot, match.Board.GetPiece(from).Type);
            Assert.AreEqual(PieceType.Soldier, match.Board.GetPiece(to).Type);
            Assert.AreEqual((72, 72), match.Score.Score);
            Assert.AreEqual(PlayerTeam.Cho, match.Turn.CurrentTeam);
            Assert.AreEqual(0, match.Record.Count);
        }

        match.Engine.GameStateEvents.OnTimeChanged += _ => AssertRestoredState();
        match.Engine.GameStateEvents.OnRecordChanged += (_, _) => AssertRestoredState();
        match.Engine.GameEvents.OnTurnCompleted += _ => completed++;

        Assert.IsTrue(match.Engine.TryUnDo(out var data));
        Assert.AreEqual((72, 72), data.Score);
        Assert.AreEqual(3, notifications);
        Assert.AreEqual(0, completed);
    }

    [TestMethod]
    public void GiveUp_ReportsWinnerAndRejectsFurtherTurns()
    {
        var match = JanggiEngineFixture.Create();
        TurnData? completed = null;
        match.Engine.GameEvents.OnTurnCompleted += data => completed = data;

        match.Engine.GiveUp();

        Assert.IsNotNull(completed);
        Assert.IsTrue(completed.GameResult.HasValue);
        Assert.AreEqual(GameResult.GiveUp, completed.GameResult.Value.Type);
        Assert.AreEqual(PlayerTeam.Cho, completed.GameResult.Value.Loser);
        Assert.AreEqual(PlayerTeam.Han, completed.GameResult.Value.Winner);
        Assert.IsNull(completed.MovedRecord);
        Assert.IsTrue(match.Turn.IsEnd);
        match.Engine.Handicap();
        Assert.AreEqual(1, match.Record.Count);
        Assert.IsFalse(match.Engine.TryUnDo(out _));
    }
}

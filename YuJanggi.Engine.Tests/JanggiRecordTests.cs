using Microsoft.VisualStudio.TestTools.UnitTesting;
using YuJanggi.Engine.Domain;
using Record = YuJanggi.Engine.JanggiRecord.JanggiRecord;

namespace YuJanggi.Engine.Tests;

[TestClass]
public class JanggiRecordTests
{
    [TestMethod]
    public void ExitReplay_SelectsLatestRecordAndNotifiesCursor()
    {
        var record = new Record();
        record.Push(new TurnData());
        record.Push(new TurnData());
        record.EnterReplay();
        Assert.IsTrue(record.TryGetTurnData(0, out _));
        record.Push(new TurnData());
        (int Current, int Next) cursor = (-1, -1);
        record.OnRecordChanged += (current, next) => cursor = (current, next);

        record.ExitReplay();

        Assert.IsTrue(record.IsAtLatestRecord);
        Assert.AreEqual(3, record.CurrMoveNumber);
        Assert.AreEqual((3, 4), cursor);
    }

    [TestMethod]
    public void PopDuringReplay_ClampsCursorIncludingEmptyHistory()
    {
        var record = new Record();
        record.Push(new TurnData());
        record.Push(new TurnData());
        record.EnterReplay();
        Assert.IsTrue(record.TryGetTurnData(1, out _));

        Assert.IsTrue(record.TryPop(out _));
        Assert.AreEqual(1, record.CurrMoveNumber);
        Assert.IsTrue(record.IsAtLatestRecord);
        Assert.IsTrue(record.TryPop(out _));
        Assert.AreEqual(0, record.CurrMoveNumber);
        Assert.AreEqual(0, record.Count);
        Assert.IsFalse(record.TryGetTurnData(0, out _));
    }

    [TestMethod]
    public void StartGame_ResetsReplaySoNewRecordsFollowLatest()
    {
        var record = new Record();
        record.Push(new TurnData());
        record.EnterReplay();

        record.StartGame();
        record.Push(new TurnData());

        Assert.IsTrue(record.IsAtLatestRecord);
        Assert.AreEqual(1, record.CurrMoveNumber);
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using YuJanggi.Engine.Domain;
using Turn = YuJanggi.Engine.JanggiTurn.JanggiTurn;

namespace YuJanggi.Engine.Tests;

[TestClass]
public class JanggiTurnTests
{
    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TurnReset_DoesNotCarryElapsedTime(bool restart)
    {
        var turn = new Turn(30f);
        turn.StartGame(PlayerTeam.Cho);
        turn.Update(0.75f);
        if (restart)
            turn.StartGame(PlayerTeam.Cho);
        else
            turn.NextTurn();

        int notifications = 0;
        int remaining = -1;
        turn.OnTimeChanged += value =>
        {
            notifications++;
            remaining = value.time;
        };

        turn.Update(0.5f);
        Assert.AreEqual(0, notifications);
        turn.Update(0.5f);
        Assert.AreEqual(1, notifications);
        Assert.AreEqual(29, remaining);
    }
}

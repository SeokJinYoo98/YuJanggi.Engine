using YuJanggi.Core.Board;
using YuJanggi.Core.Domain;
using YuJanggi.Core.JanggiRule;

namespace YuJanggi.Core.Match
{
    public readonly struct JanggiOptions
    {
        public float     TurnTime { get; }
        public Formation ChoFormation { get; }
        public Formation HanFormation { get; }
        public int Width { get; }
        public int Height { get; }
        public JanggiOptions(
            float turnTime         = 30f,
            Formation choFormation = Formation.EHHE,
            Formation hanFormation = Formation.EHHE,
            int width=9, int height=10)
        {
            if (turnTime < 10f)
                TurnTime = 30f;
            else
                TurnTime = turnTime;
            ChoFormation = choFormation;
            HanFormation = hanFormation;
            Width = 9; Height = 10;
        }
    }
    public static class MatchFactory
    {

        //public static ILiveMatch LocalCreate(MatchOptions options)
        //{
        //    var match = new MatchModel(
        //        new Turn(options.TurnTime),
        //        new Record(),
        //        new Score(),
        //        new BoardModel(),
        //        new JanggiRule());

        //    match.InitGame(
        //        options.ChoFormation,
        //        options.HanFormation);

        //    return match;
        //}
    }
}

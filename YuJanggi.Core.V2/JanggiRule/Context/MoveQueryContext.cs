using System.Collections.Generic;

namespace YuJanggi.Core.JanggiRule.Context
{
    using Domain;
    internal sealed class MoveQueryContext
    {
        public Pos From { get; private set; } = Pos.Invalid;

        public List<Pos> Candidates { get; }    = new(25);
        public List<Pos> Legal { get; }         = new(25);
        public List<Pos> Illegal { get; }       = new(25);
        public void Reset(Pos from)
        {
            From = from;

            Candidates.Clear();
            Legal.Clear();
            Illegal.Clear();
        }
    }
}

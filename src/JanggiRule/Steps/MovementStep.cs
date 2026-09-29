#nullable enable
using System;
using System.Collections.Generic;


namespace YuJanggi.Engine.JanggiRule.Steps
{
    using JanggiBoard;
    using Domain;
    using Movement;
    using Context;

    internal sealed class MovementStep : IRuleStep
    {
        private readonly Dictionary<PieceType, Movement> _rules;

        public MovementStep()
        {
            var kingGuard = new KingGuardMovement();

            _rules = new()
            {
                { PieceType.Soldier,  new SoldierMovement() },
                { PieceType.Chariot,  new ChariotMovement() },
                { PieceType.Cannon,   new CannonMovement() },
                { PieceType.Horse,    new HorseMovement() },
                { PieceType.Elephant, new ElephantMovement() },
                { PieceType.King,     kingGuard },
                { PieceType.Guard,    kingGuard }
            };
        }
        public void Execute(IJanggiBoard board, MoveQueryContext context)
        {
            var piece = board.GetPiece(context.From);

            if (!_rules.TryGetValue(piece.Type, out var movement))
                throw new InvalidOperationException(
                    $"Movement가 없습니다: {piece.Type}");

            movement.FindWays(
                board,
                context.From,
                context.Candidates);
        }
    }
}

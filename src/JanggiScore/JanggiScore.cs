#nullable enable
using System;

namespace YuJanggi.Engine.JanggiScore
{
    using Domain;
    public interface IReadOnlyScore
    {
        event Action<(int Cho, int Han)>? OnScoreChanged;
        (int Cho, int Han) Score { get; }
    }

    public class JanggiScore : IReadOnlyScore
    {
        public event Action<(int Cho, int Han)>? OnScoreChanged;
        private int _choScore = 72;
        private int _hanScore = 72;
        public (int Cho, int Han) Score
            => (_choScore, _hanScore);
        private int GetPieceScore(PieceType type)
        {
            return type switch
            {
                PieceType.Chariot => 13,    // 차
                PieceType.Cannon => 7,      // 포
                PieceType.Horse => 5,       // 마
                PieceType.Elephant => 3,    // 상
                PieceType.Guard => 3,       // 사
                PieceType.Soldier => 2,     // 졸
                PieceType.King => 10000,
                _ => 0
            };
        }
        public void ApplyScore(PlayerTeam team, PieceType type, bool isUndo = false)
        {
            var value = GetPieceScore(type);
            value = isUndo ? value : value * -1;
            if (team == PlayerTeam.Cho)
                _choScore += value;  
            else
                _hanScore += value;

            OnScoreChanged?.Invoke(Score);
        }
        public PlayerTeam Winner()
        {
            if (_choScore == _hanScore)
                return PlayerTeam.None;

            return _choScore < _hanScore ? PlayerTeam.Han : PlayerTeam.Cho;
        }

        public void StartGame()
        {
            _choScore = 72;
            _hanScore = 72;

            OnScoreChanged?.Invoke(Score);
        }
    }
}

using System;
using System.Collections.Generic;

namespace YuJanggi.Core.JanggiEngine
{
    using Board;
    using Match;
    using JanggiRule;
    using Domain;

    internal sealed class JanggiEngine : IJanggiEngine
    {
        #region Fields
        // 내부 상태와 참조를 저장하는 변수
        private readonly JanggiBoard    _board;
        private readonly JanggiRule     _rule;
        private readonly JanggiRecord   _record;
        private readonly JanggiTurn     _turn;
        private readonly JanggiScore    _score;
        private readonly JanggiOptions  _options;
        #endregion

        #region Properties
        public PlayerTeam CurrentTurn { get; private set; }

        #endregion

        #region Events
        // 상태 변화나 특정 동작을 외부에 알리는 이벤트
        #endregion

        #region Constructors
        // 순수 C#
        public JanggiEngine(JanggiOptions options)
        {
            _turn   = new JanggiTurn(options.TurnTime);
            _board  = new JanggiBoard(options.Width, options.Height);
            _rule   = new JanggiRule(_board);
            _record = new JanggiRecord();
            _score  = new JanggiScore();

            _options = options;
        }


        public void GetMovableCells(Pos from, List<Pos> legalCells, List<Pos> illegalCells)
        {
            throw new NotImplementedException();
        }

        public bool HasPiece(Pos pos)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        // 외부에서 호출하는 기능
        public bool InitEngine()
        {
            _board.ResetBoard();
            BoardInitializer.SetUpPieces(
                _board, 
                _options.ChoFormation, 
                _options.HanFormation);
            return true;
        }
        public bool StartEngine()
        {
            _record.StartGame();
            _score.StartGame();
            _turn.StartGame(PlayerTeam.Cho);

            return true;
        }
        public bool IsTeamPiece(Pos pos, PlayerTeam team)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Event Handlers
        // 구독한 이벤트가 발생했을 때 실행하는 처리 메서드
        #endregion

        #region Private Methods
        // 클래스 내부에서 사용하는 보조 로직
        #endregion




    }

}

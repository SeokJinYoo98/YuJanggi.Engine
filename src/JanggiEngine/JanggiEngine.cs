using System;
using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiEngine
{
    using JanggiBoard;
    using Domain;
    using JanggiRule;
    using JanggiRecord;
    using JanggiTurn;
    using JanggiScore;
    using JanggiOption;

    internal sealed class JanggiEngine : IJanggiEngine
    {
        #region Fields
        // 내부 상태와 참조를 저장하는 변수
        private readonly JanggiBoard        _janggiBoard;
        private readonly JanggiRule         _janggiRule;
        private readonly JanggiRecord       _janggiRecord;
        private readonly JanggiTurn         _janggiTurn;
        private readonly JanggiScore        _janggiScore;
        private readonly JanggiOptions      _janggiOptions;
        private readonly JanggiEngineEvents _janggiEvents;

        #endregion

        #region Properties
        public PlayerTeam CurrentTurn => _janggiTurn.CurrentTeam;
        public IReadOnlyGameEvents GameEvents 
            => _janggiEvents;
        public IReadOnlyGameStateEvents GameStateEvents
            => _janggiEvents;
        public IReadOnlyRecord Record 
            => _janggiRecord;

        #endregion

        #region Events
        // 상태 변화나 특정 동작을 외부에 알리는 이벤트
        #endregion

        #region Constructors
        // 순수 C#
        public JanggiEngine(JanggiOptions options)
        {
            _janggiTurn   = new JanggiTurn(options.TurnTime);
            _janggiBoard  = new JanggiBoard(options.Width, options.Height);
            _janggiRule   = new JanggiRule(_janggiBoard);
            _janggiRecord = new JanggiRecord();
            _janggiScore  = new JanggiScore();
            _janggiEvents = new JanggiEngineEvents();

            _janggiOptions = options;
        }
        #endregion

        #region Public Methods
        // 외부에서 호출하는 기능
        public void GetMovableCells(
            Pos from,
            List<Pos> legalCells,
            List<Pos> illegalCells)
        {
            _janggiRule.FindLegalMoves(
                from,
                legalCells, 
                illegalCells);
        }
        public bool InitEngine()
        {
            _janggiBoard.ResetBoard();
            BoardInitializer.SetUpPieces(
                _janggiBoard, 
                _janggiOptions.ChoFormation, 
                _janggiOptions.HanFormation);
            return true;
        }
        public bool StartEngine()
        {
            _janggiRecord.StartGame();
            _janggiScore.StartGame();
            _janggiTurn.StartGame(PlayerTeam.Cho);

            return true;
        }
        public bool IsValidPiece(PlayerTeam team, Pos pos, out int pieceId)
        {
            pieceId = int.MinValue;

            if (!_janggiBoard.IsInside(pos))
                return false;

            if (!_janggiBoard.HasPiece(pos))
                return false;

            var piece = _janggiBoard.GetPiece(pos);

            if (piece.Team != team)
                return false;

            pieceId = piece.Id;
            return true;
        }
        public bool TryMove(Pos from, Pos to)
        {
            if (_janggiTurn.IsEnd)
                return false;

            if (!_janggiBoard.IsInside(from) || !_janggiBoard.IsInside(to))
                return false;

            if (!_janggiBoard.HasPiece(from))
                return false;

            if (_janggiBoard.GetPiece(from).Team != _janggiTurn.CurrentTeam)
                return false;

            if (!_janggiRule.IsLegalMove(from, to))
                return false;

            ExecuteMove(from, to);
            return true;
        }
        public void Tick(float deltaTime)
        {
            _janggiTurn.Update(deltaTime);
        }
        public void UnBindEvents()
        {
            _janggiTurn.OnTimeChanged     -= _janggiEvents.TimeChanged;
            _janggiTurn.OnTurnChanged     -= _janggiEvents.TurnChanged;
            _janggiRecord.OnRecordChanged -= _janggiEvents.RecordChanged;
            _janggiScore.OnScoreChanged   -= _janggiEvents.ScoreChanged;
            _janggiTurn.OnTurnEnd      -= HandleHandicap;

        }
        public void BindEvents()
        {
            _janggiTurn.OnTimeChanged     += _janggiEvents.TimeChanged;
            _janggiTurn.OnTurnChanged     += _janggiEvents.TurnChanged;
            _janggiRecord.OnRecordChanged += _janggiEvents.RecordChanged;
            _janggiScore.OnScoreChanged   += _janggiEvents.ScoreChanged;
            _janggiTurn.OnTurnEnd      += HandleHandicap;
        }
        public bool TryUnDo(out MoveContext ctx)
        {
            ctx = default;

            if (_janggiTurn.IsEnd)
                return false;

            if (!_janggiRecord.TryPop(out ctx))
                return false;

            if (!ctx.IsHandicap)
            {
                var record = ctx.Record;
                _janggiBoard.UndoMove(record);

                if (record.IsCapture)
                {
                    var captured = record.CapturedPiece;
                    _janggiScore.ApplyScore(captured.Team, captured.Type, true);
                }
            }

            _janggiTurn.NextTurn();

            return true;
        }
        #endregion

        #region Event Handlers
        // 구독한 이벤트가 발생했을 때 실행하는 처리 메서드
        public void HandleGiveUp()
            => OnGameEnded(GameResult.GiveUp, _janggiTurn.CurrentTeam);
        public void HandleHandicap()
        {
            if (_janggiTurn.IsEnd) return;
            _janggiRecord.Push(MoveContext.Handicap);
            _janggiTurn.NextTurn();
        }
        public void ToLiveRecord()
            => _janggiRecord.ExitReplay();

        public void ToReplayRecord()
            => _janggiRecord.EnterReplay();

        #endregion

        #region Private Methods
        // 클래스 내부에서 사용하는 보조 로직
        private void ExecuteMove(Pos from, Pos to)
        {
            // ExecuteMove인데 너무 처리하는 역할이 많음 리팩토링 필요
            var record = _janggiBoard.DoMove(from, to);

            var otherTeam = _janggiTurn.CurrentTeam == PlayerTeam.Cho
                ? PlayerTeam.Han
                : PlayerTeam.Cho;

            if (record.IsCapture)
                _janggiScore.ApplyScore(otherTeam, record.CapturedPiece.Type);


            var isJanggun = IsCheck(otherTeam);
            var isEnd = !_janggiRule.HasAnyLegalMove(otherTeam);
            var ctx = new MoveContext(record, isJanggun, isEnd);

            _janggiRecord.Push(ctx);
            _janggiEvents.PieceMoved(ctx);

            if (isEnd)
                OnGameEnded(GameResult.CheckMate, otherTeam);
            else
                _janggiTurn.NextTurn();
        }
        private bool IsCheck(PlayerTeam otherTeam)
        {
            var result = _janggiRule.IsKingInCheck(otherTeam);

            if (result) 
                _janggiEvents.CheckOccurred(otherTeam);

            if (_janggiRecord.TryPeek(out var ctx) && ctx.IsJanggun)
                _janggiEvents.CheckReleased();

            return result;
        }
        private void OnGameEnded(GameResult result, PlayerTeam loser)
        {
            _janggiTurn.EndGame();
            GameResultInfo info = new()
            {
                Type    = result,
                Loser   = loser,
                MoveCnt = _janggiRecord.TotalTurn
            };
            _janggiEvents.GameEnded(info);
        }


        #endregion
    }

}

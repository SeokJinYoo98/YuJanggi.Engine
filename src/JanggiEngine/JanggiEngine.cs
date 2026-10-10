#nullable enable

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
        private readonly ControllerQuery    _controllerQuery;

        #endregion

        #region Properties
        public PlayerTeam CurrentTurn           => _janggiTurn.CurrentTeam;
        public IControllerQuery ControllerQuery => _controllerQuery;
        public IReadOnlyGameEvents GameEvents   => _janggiEvents;
        public IReadOnlyGameStateEvents GameStateEvents => _janggiEvents;
        public IReadOnlyRecord Record           => _janggiRecord;
        public IReadOnlyBoard Board             => _janggiBoard;
        public IAIPosition CreateAIPosition()   => new AIPosition(_janggiBoard);

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
            _controllerQuery = new ControllerQuery(
                _janggiBoard,
                _janggiRule,
                _janggiTurn);

            _janggiOptions = options;
        }
        #endregion

        #region Public Methods
        // 외부에서 호출하는 기능
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
        public bool TryProcessTurn(Pos from, Pos to)
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

            var actingTeam = CurrentTurn;
            var record     = UpdateMovement(from, to);

            ProcessTurn(actingTeam, record);
            return true;
        }
        public void Tick(float deltaTime)
        {
            _janggiTurn.Update(deltaTime);
        }
        public void UnBindEvents()
        {
            _janggiTurn.OnTimeChanged     -= _janggiEvents.TimeChanged;
            _janggiRecord.OnRecordChanged -= _janggiEvents.RecordChanged;
            _janggiTurn.OnTurnEnd         -= HandleHandicap;

        }
        public void BindEvents()
        {
            UnBindEvents();

            _janggiTurn.OnTimeChanged     += _janggiEvents.TimeChanged;
            _janggiRecord.OnRecordChanged += _janggiEvents.RecordChanged;
            _janggiTurn.OnTurnEnd         += HandleHandicap;
        }
        public bool TryUnDo(out UndoData ctx)
        {
            ctx = null!;

            if (_janggiTurn.IsEnd)
                return false;

            if (!_janggiRecord.TryPop(out var data, notify: false) || data == null)
                return false;

            if (data.MovedRecord is MoveRecord record)
            {
                _janggiBoard.UndoMove(record);

                if (record.IsCaptured)
                {
                    var captured = record.CapturedPiece;
                    _janggiScore.ApplyScore(captured.Team, captured.Type, true);
                }
            }

            _janggiTurn.NextTurn(notify: false);

            _janggiRecord.TryPeek(out var previous);
            var checkedTeam = previous?.CheckedTeam;

            ctx = new UndoData
            {
                UndoneMove        = data.MovedRecord,
                CurrentTurn       = _janggiTurn.CurrentTeam,
                Score             = _janggiScore.Score,
                RecordCount       = _janggiRecord.Count,
                CheckedTeam       = checkedTeam,
                CheckReleasedTeam = data.CheckedTeam != checkedTeam
                    ? data.CheckedTeam
                    : null
            };
            _janggiTurn.NotifyTurnChanged();
            _janggiRecord.NotifyRecordChanged();
            return true;
        }
        public void GiveUp()
        {
            if (_janggiTurn.IsEnd)
                return;

            ProcessTurn(
                CurrentTurn,
                record: null,
                gameResult: new GameResultInfo
                {
                    Type   = GameResult.GiveUp,
                    Loser  = CurrentTurn,
                    Winner = _janggiTurn.NextTeam
                });
        }
        public void Handicap()
        {
            if (_janggiTurn.IsEnd)
                return;

            ProcessTurn(CurrentTurn, record: null);
        }
        public void ToLiveRecord()
            => _janggiRecord.ExitReplay();

        public void ToReplayRecord()
            => _janggiRecord.EnterReplay();
        #endregion

        #region Event Handlers
        // 구독한 이벤트가 발생했을 때 실행하는 처리 메서드
        public void HandleHandicap()
            => Handicap();


        #endregion

        #region Private Methods
        // 클래스 내부에서 사용하는 보조 로직
        private void ProcessTurn(
            PlayerTeam      actingTeam,
            MoveRecord?     record,
            GameResultInfo? gameResult = null)
        {
            var otherTeam   = _janggiTurn.NextTeam;

            _janggiRecord.TryPeek(out var previous);
            var previousCheckedTeam = previous?.CheckedTeam;

            var checkedTeam = record != null
                ? GetCheckedTeam(otherTeam)
                : previousCheckedTeam;

            PlayerTeam? releasedTeam =
                record != null && previousCheckedTeam == actingTeam
                    ? actingTeam
                    : null;

            if (record != null && !gameResult.HasValue)
                gameResult = ProcessTurnEnd(actingTeam, otherTeam);

            int moveCount = _janggiRecord.NextMoveNumber;

            var data = new TurnData
            {
                ActingTeam        = actingTeam,
                MoveCount         = moveCount,
                TotalTurn         = moveCount + 1,
                Score             = _janggiScore.Score,
                MovedRecord       = record,
                CheckedTeam       = checkedTeam,
                CheckReleasedTeam = releasedTeam,
                GameResult        = gameResult
            };

            CompleteTurn(data);
        }
        private MoveRecord          UpdateMovement(Pos from, Pos to)
        {
            var record = _janggiBoard.DoMove(from, to);
            if (record.IsCaptured)
            {
                _janggiScore.ApplyScore(
                    record.CapturedPiece.Team,
                    record.CapturedPiece.Type);
            }
            return record;
        }
        private PlayerTeam?         GetCheckedTeam(PlayerTeam team)
            => _janggiRule.IsKingInCheck(team)
                ? team
                : null;
        private GameResultInfo?     ProcessTurnEnd(
            PlayerTeam actingTeam,
            PlayerTeam otherTeam)
        {
            if (_janggiRule.HasAnyLegalMove(otherTeam))
                return null;

            return new GameResultInfo
            {
                Type   = GameResult.CheckMate,
                Loser  = otherTeam,
                Winner = actingTeam
            };
        }

        private void CompleteTurn(TurnData data)
        {
            if (data.GameResult.HasValue)
                _janggiTurn.EndGame();
            else
                _janggiTurn.NextTurn(notify: false);

            _janggiRecord.Push(data, notify: false);

            if (!data.GameResult.HasValue)
                _janggiTurn.NotifyTurnChanged();

            _janggiRecord.NotifyRecordChanged();
            _janggiEvents.TurnCompleted(data);
        }

        #endregion
    }

}

#nullable enable
using System;
using System.Collections.Generic;

namespace YuJanggi.Engine.JanggiRecord
{
    using Domain;
    public interface IReadOnlyRecord
    {
        int Count { get; }
        bool TryGetTurnData(int idx, out TurnData? data);
    }
    public interface IReplayRecord : IReadOnlyRecord
    {
        void EnterReplay();
        void ExitReplay();
    }
    internal class JanggiRecord : IReplayRecord
    {
        public event Action<int, int>  ?OnRecordChanged;
        public bool IsAtLatestRecord
            => Count - 1 == _currIdx;
        public int CurrMoveNumber         
            => _currIdx + 1;
        public int NextMoveNumber
            => Count + 1;
        public int Count            
            => _records.Count;

        // 현재 화면에 적용된 마지막 기록 Index
        private int  _currIdx = -1;
        private bool _replay = false;
        private readonly List<TurnData> _records = new(100);
        public void StartGame()
        {
            _records.Clear();
            _currIdx = -1;
            OnRecordChanged?.Invoke(CurrMoveNumber, NextMoveNumber);
        }
        public bool TryGetTurnData(int idx, out TurnData? data)
        {
            if (idx < 0 || _records.Count <= idx)
            {
                data = null;
                return false;
            }
            _currIdx = idx;
            data = _records[_currIdx];
            OnRecordChanged?.Invoke(CurrMoveNumber, NextMoveNumber);
            return true;
        }

  
        public void Push(TurnData data)
        {
            _records.Add(data);

            if (!_replay) _currIdx = _records.Count - 1;
            
            OnRecordChanged?.Invoke(CurrMoveNumber, NextMoveNumber);
        }
        public bool TryPop(out TurnData? data)
        {
            if (_records.Count == 0)
            {
                data = null;
                return false;
            }

            int lastIdx = _records.Count - 1;
            data = _records[lastIdx];
            _records.RemoveAt(lastIdx);

            if (!_replay) _currIdx = _records.Count - 1;

            OnRecordChanged?.Invoke(CurrMoveNumber, NextMoveNumber);
            return true;
        }
        public bool TryPeek(out TurnData? data)
        {
            if (_records.Count == 0)
            {
                data = null;
                return false;
            }

            data = _records[^1];
            return true;
        }
        public void EnterReplay() => _replay = true;
        public void ExitReplay() => _replay = false;
    }
}

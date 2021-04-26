using System;
using UnityEngine;

namespace Game.Core
{
    public abstract class TimedEvent : MonoBehaviour
    {
        private float _timeLeft;
        private Action _onTimerComplete;
        
        protected void StartTimer(float time, Action onTimerComplete)
        {
            _timeLeft = time;
            _onTimerComplete = onTimerComplete;
        }

        protected void CancelTimer()
        {
            _onTimerComplete = null;
            _timeLeft = 0f;
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                _onTimerComplete?.Invoke();
            }
        }
    }
}
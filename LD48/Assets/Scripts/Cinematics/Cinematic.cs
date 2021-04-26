using System.Collections;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Cinematics
{
    public class Cinematic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _camera = null;
        [SerializeField] private Path _cameraPath = null;
        
        [Header("Stats")]
        [SerializeField] private float _waypointTolerance = 0.6f;
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float[] _waitTimes = null;

        [Header("Events")]
        [SerializeField] private UnityEvent _onPlay = null;
        [SerializeField] private UnityEvent _onEnd = null;

        private float _timeSinceTouchedWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        private Vector3 _movement = Vector3.zero;
        private bool _isPlaying;
        private bool _reachedEnd;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main.transform;
            if (_waitTimes.Length == 0) _waitTimes = new[] {1f};
        }

        private void Update()
        {
            if (!_isPlaying) return;
            print(AtWaypoint());
            if (AtWaypoint())
            {
                _timeSinceTouchedWaypoint = 0;
                if (_currentWaypointIndex != _cameraPath.transform.childCount - 1)
                {
                    CycleWaypoint();
                }
                else
                {
                    if (_reachedEnd) return;
                    _reachedEnd = true;
                    print("END");
                    StartCoroutine(EndAfterWait(GetWaitTime()));
                    _movement = Vector3.zero;
                }
            }

            var nextPosition = GetCurrentWaypoint();
            
            if (_timeSinceTouchedWaypoint > GetWaitTime())
            {
                _movement = (nextPosition - _camera.position).normalized;
            }
        }

        private void LateUpdate()
        {
            var currentPos = _camera.position;
            var newPos = _movement * _speed + currentPos;
            _camera.position = Vector3.Lerp(currentPos, newPos, Time.deltaTime);
        }

        private float GetWaitTime()
        {
            return _waitTimes[_currentWaypointIndex];
        }

        private IEnumerator EndAfterWait(float wait)
        {
            yield return new WaitForSeconds(wait);
            _onEnd?.Invoke();
            _isPlaying = false;
        }
        
        public void Play()
        {
            if (_isPlaying) return;
            
            _onPlay?.Invoke();
            _isPlaying = true;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(_camera.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }
        
        private void CycleWaypoint()
        {
            _currentWaypointIndex = _cameraPath.GetNextIndex(_currentWaypointIndex);
        }
        
        private Vector3 GetCurrentWaypoint()
        {
            return _cameraPath.GetWaypoint(_currentWaypointIndex);
        }
    }
}
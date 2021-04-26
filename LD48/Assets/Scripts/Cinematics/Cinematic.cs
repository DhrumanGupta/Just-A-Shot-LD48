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
        [SerializeField] private float _waypointWaitTime = 1f;
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
        
        private void Awake()
        {
            if (_camera == null) _camera = Camera.main.transform;
            if (_waitTimes.Length == 0) _waitTimes = new[] {1f};
        }

        private void Update()
        {
            if (!_isPlaying) return;
            
            if (AtWaypoint())
            {
                if (_waitTimes.Length - 1 > _currentWaypointIndex)
                    _timeSinceTouchedWaypoint = _waitTimes[_waitTimes.Length - 1];
                else 
                    _timeSinceTouchedWaypoint = -_waitTimes[_currentWaypointIndex];
                
                if (_currentWaypointIndex != _cameraPath.transform.childCount)
                {
                    CycleWaypoint();
                }
                else
                {
                    _isPlaying = false;
                    _onEnd?.Invoke();
                }
            }

            var nextPosition = GetCurrentWaypoint();
            
            if (_timeSinceTouchedWaypoint > _waypointWaitTime)
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
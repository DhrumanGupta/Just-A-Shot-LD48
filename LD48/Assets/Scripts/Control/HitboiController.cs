using System;
using Game.Combat;
using Game.Core;
using UnityEngine;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitboiController : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private float _waypointDwellTime = 1f;
        [SerializeField] private float _waypointTolerance = 0.6f;
        [SerializeField] private float _speed = 7f;
        [Range(0, 1)]
        [SerializeField] private float _frictionForce = 2f;

        [Header("References")] [SerializeField]
        private Path _patrolPath = null;

        [SerializeField] private GameObject _questionMark = null;

        private Fighter _fighter;
        private Health _health;
        private Health _player;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceTouchedWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        
        private Vector2 _movement = Vector2.zero;
        private int _animatorRunId;

        private float _localScale;
        private bool _isPatrolPathNotNull;

        private void Start()
        {
            _isPatrolPathNotNull = _patrolPath != null;
            _localScale = transform.localScale.x;
            _health = GetComponent<Health>();
            _fighter = GetComponent<Fighter>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _guardPosition = transform.position;

            _player = GameObject.FindWithTag("Player").GetComponent<Health>();
            _animatorRunId = Animator.StringToHash("isWalking");
        }

        private void Update()
        {
            if (_health.IsDead) return;

            if (InAttackRangeOfPlayer())
            {
                if (_questionMark.activeSelf) _questionMark.SetActive(false);
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                if (!_questionMark.activeSelf) _questionMark.SetActive(true);
                _movement = Vector2.zero;
            }
            else
            {
                if (_questionMark.activeSelf) _questionMark.SetActive(false);
                PatrolBehaviour();
            }

            UpdateTimers();
            UpdateAnimator();
            FlipBasedOnMovement();
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(_movement, ForceMode2D.Impulse);
            
            // Apply some friction
            Vector2 friction = _rigidbody.velocity * _frictionForce;
            friction.x *= -1f;
            friction.y = 0;
            _rigidbody.AddForce(friction, ForceMode2D.Force);
        }

        private void FlipBasedOnMovement()
        {
            var velocity = _rigidbody.velocity.x;
            if (Mathf.Abs(velocity) < 0.1f) return;
            
            _spriteRenderer.flipX = velocity < 0;
        }
        
        private void UpdateAnimator()
        {
            _animator.SetBool(_animatorRunId, Mathf.Abs(_movement.x) > 0.1f);
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;

            if (!_fighter.CanAttack(_player))
            {
                var distance = Mathf.Clamp(_player.transform.position.x - _rigidbody.position.x, -1, 1) * _speed;
                _movement = new Vector2(distance, 0);
                return;
            }
            
            _fighter.Attack(_player);
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition;

            if (_isPatrolPathNotNull)
            {
                if (AtWaypoint())
                {
                    _timeSinceTouchedWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceTouchedWaypoint > _waypointDwellTime)
            {
                var distance = Mathf.Clamp(nextPosition.x - _rigidbody.position.x, -1, 1) * (_speed * .8f);
                _movement = new Vector2(distance, 0);
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceTouchedWaypoint += Time.deltaTime;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < _chaseDistance;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.collider.CompareTag("Player")) return;
            if (!other.collider.TryGetComponent(out Health target)) return;
            if (_fighter.CanAttack(target))
            {
                _fighter.Attack(target);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 138, 230);
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}
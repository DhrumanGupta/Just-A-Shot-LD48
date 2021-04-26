using System;
using System.Collections;
using System.Collections.Generic;
using Game.Combat;
using Game.Core;
using Game.Environment;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BossController : MonoBehaviour
    {
        enum Phase
        {
            Chill,
            Angry
        }
        
        [Space]
        
        [Header("Attack Settings")]
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _attackRange = 100f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        
        [Space]
        
        [SerializeField] private AnimationCurve _runeScaleCurve = null;
        [SerializeField] private int _maxHealth = 20;
        [Range(0, 1)] [SerializeField] private float _phaseChange = 0.6f;
        
        private float _timeSinceLastAttack = 0f;
        private Phase _currentPhase = Phase.Chill;

        [Space]
        [Header("References")]
        [SerializeField] private Path _patrolPath = null;
        [SerializeField] private GameObject[] _runes = null;
        [SerializeField] private GameObject _laser = null;

        private Health _health;
        private Health _player;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        private Vector3 _guardPosition;
        private float _timeSinceTouchedWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        
        private Vector2 _movement = Vector2.zero;
        private int _animatorAttackId;
        private bool _isPatrolPathNotNull;

        private void Start()
        {
            _isPatrolPathNotNull = _patrolPath != null;
            _health = GetComponent<Health>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _guardPosition = transform.position;

            _player = GameObject.FindWithTag("Player").GetComponent<Health>();
            _animatorAttackId = Animator.StringToHash("isAttacking");
        }

        private void Update()
        {
            if (_health.IsDead) return;

            if (CanAttack(_player))
            {
                AttackBehaviour();
            }
            else
            {
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
            // _animator.SetBool(_animatorAttackId, _isAttacking);
        }

        private void AttackBehaviour()
        {
            _timeSinceLastAttack = 0f;
            if (_currentPhase == Phase.Chill)
            { 
                // StartCoroutine(SpawnLaserOrSkull());
            }
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
            _timeSinceTouchedWaypoint += Time.deltaTime;
            _timeSinceLastAttack += Time.deltaTime;
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
        
        private bool CanAttack(Health target)
        {
            if (target == null) { return false; }
            return !target.IsDead && IsInRange(target.transform) && _timeSinceLastAttack >= _timeBetweenAttacks;
        }

        private bool IsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < _attackRange;
        }

        private void OnDestroy()
        {
            foreach (var obj in _toDestroy)
            {
                Destroy(obj);
            }
        }

#if UNITY_EDITOR

        [Header("EDITOR ONLY")]
        [SerializeField] private bool _showGizmosGlobally = false;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 138, 230);
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmosGlobally) return;
            Gizmos.color = new Color(0, 138, 230);
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
        
        #endif
    }
}
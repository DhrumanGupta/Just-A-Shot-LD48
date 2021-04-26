﻿using System;
using Game.Combat;
using UnityEngine;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AIController : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private float _waypointDwellTime = 1f;
        [SerializeField] private float _waypointTolerance = 0.6f;
        [SerializeField] private float _speed = 7f;
        [SerializeField] private float _frictionForce = 2f;

        [Header("References")] [SerializeField]
        private PatrolPath _patrolPath = null;

        [SerializeField] private GameObject _questionMark = null;

        private Fighter _fighter;
        private Health _health;
        private Health _player;
        private Rigidbody2D _rigidbody;

        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceTouchedWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        
        private Vector2 _movement = Vector2.zero;

        private void Start()
        {
            _health = GetComponent<Health>();
            _fighter = GetComponent<Fighter>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _guardPosition = transform.position;

            _player = GameObject.FindWithTag("Player").GetComponent<Health>();
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
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(_movement);
            
            // Apply some friction
            Vector2 friction = _rigidbody.velocity.normalized * _frictionForce;
            friction.x *= -1;
            friction.y = 0;
            _rigidbody.AddForce(friction, ForceMode2D.Force);
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;

            if (!_fighter.CanAttack(_player))
            {
                var distance = Mathf.Clamp(_player.transform.position.x - _rigidbody.position.x, -1, 1) * _speed;
                _movement = new Vector2(distance, 0);
                print("going");
                return;
            }
            
            print("attack");
            _fighter.Attack(_player);
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition;

            if (_patrolPath != null)
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 138, 230);
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}
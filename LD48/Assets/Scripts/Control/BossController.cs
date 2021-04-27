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
        [SerializeField] private float _attackRange = 100f;
        [SerializeField] private float _timeBetweenAttacks = 2f;
        
        [Space]
        
        [SerializeField] private int _maxHealth = 20;
        [Range(0, 1)] [SerializeField] private float _phaseChange = 0.6f;
        
        private float _timeSinceLastAttack = 0f;
        private Phase _currentPhase = Phase.Chill;

        [Space]
        [Header("References")]
        [SerializeField] private GameObject _skullAttack = null;
        [SerializeField] private float _skullSpeed = 2f;
        [SerializeField] private GameObject _laserAttack = null;
        [SerializeField] private float _laserSpeed = 5f;
        [SerializeField] private GameObject[] _enemiesToSpawn = null;

        private Health _health;
        private Health _player;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        private int _animatorAttackId;

        private List<GameObject> _toDestroy;

        private void Start()
        {
            _health = GetComponent<Health>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _player = GameObject.FindWithTag("Player").GetComponent<Health>();
            _animatorAttackId = Animator.StringToHash("isAttacking");
            _toDestroy = new List<GameObject>();
        }

        private void Update()
        {
            if (_health.IsDead) return;
        
            if (CanAttack(_player))
            {
                AttackBehaviour();
            }

            UpdateTimers();
            UpdateAnimator();
            FlipBasedOnMovement();
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
                StartCoroutine(SpawnLaserOrSkull());
            }
        }

        private IEnumerator SpawnLaserOrSkull()
        {
            GameObject chosenAttack;
            float speed;
            if (Random.value > 0.5f)
            {
                chosenAttack = _skullAttack;
                speed = _skullSpeed;
            }
            else
            {
                chosenAttack = _laserAttack;
                speed = _laserSpeed;
            }
            
            var dir = (_player.transform.position - transform.position).normalized;
            var angle = Mathf.Atan(dir.x / dir.y);
            var eulerAngle = new Vector3(0, 0, angle);

            var spawnedItem = Instantiate(chosenAttack, transform.position, Quaternion.LookRotation(dir))
                .transform;
            spawnedItem.GetComponent<DamagingObject>().SetData(_player);
            spawnedItem.GetComponent<Rigidbody2D>().velocity = dir * speed;
            yield break;
        }

        private void UpdateTimers()
        {
            _timeSinceLastAttack += Time.deltaTime;
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
    }
}
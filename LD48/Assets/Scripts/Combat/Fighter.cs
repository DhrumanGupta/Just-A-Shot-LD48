using System;
using UnityEngine;

namespace Game.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private float _timeBetweenAttacks = 1f;

        private Animator _animator;
        private float _timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
        }

        public void Attack(Health target)
        {
            if (_timeSinceLastAttack < _timeBetweenAttacks) return;
            if (target == null) { return; }
            
            _timeSinceLastAttack = 0f;
            target.TakeDamage(_damage);
        }

        public bool CanAttack(Health target)
        {
            if (target == null) { return false; }
            return !target.IsDead && IsInRange(target.transform);
        }

        private bool IsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < _attackRange;
        }
    }
}
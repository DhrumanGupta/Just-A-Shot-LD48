using System;
using UnityEngine;

namespace Game.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] private Weapon _defaultWeapon = null;
        [SerializeField] private float _timeBetweenAttacks = 1f;

        private Animator _animator;
        private float _timeSinceLastAttack = Mathf.Infinity;

        private Weapon _currentWeapon = null;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            EquipWeapon(_defaultWeapon);
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
            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(_currentWeapon.ProjectileSpawnTransform, target);
                return;
            }
            
            target.TakeDamage(_currentWeapon.GetDamage());
        }

        public bool CanAttack(Health target)
        {
            if (target == null) { return false; }
            return !target.IsDead && IsInRange(target.transform);
        }
        
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            _currentWeapon = weapon;
            weapon.Spawn(_animator);
        }

        private bool IsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < _currentWeapon.GetRange();
        }
    }
}
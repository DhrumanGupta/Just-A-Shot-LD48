using System;
using Game.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Combat
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private int _maxHealth = 10;
        private int _health;

        [SerializeField] private GameObject _dropOnDeath = null;
        [SerializeField] private GameObject _deathEffect = null;

        public event Action<int> OnHealthChanged;
        
        public bool IsDead { get; private set; } = false;

        private void Start()
        {
            if (_health == 0) _health = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            OnHealthChanged?.Invoke(_health);
            if (_health == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (IsDead) return;

            IsDead = true;
            var pos = transform.position;
            if (_dropOnDeath != null) Instantiate(_dropOnDeath, pos, Quaternion.identity);
            if (_deathEffect != null) Instantiate(_deathEffect, pos, Quaternion.identity);

            Destroy(gameObject);
        }

        public object CaptureState()
        {
            return _health;
        }

        public void RestoreState(object state)
        {
            _health = (int) state;
            OnHealthChanged?.Invoke(_health);
            if (_health == 0)
            {
                Die();
            }
        }
    }
}
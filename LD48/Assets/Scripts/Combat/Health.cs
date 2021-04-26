using System;
using Game.Saving;
using UnityEngine;

namespace Game.Combat
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _maxHealth = 1f;
        private float _health;

        [SerializeField] private GameObject _dropOnDeath = null;
        [SerializeField] private GameObject _deathEffect = null;

        public bool IsDead { get; private set; } = false;

        private void Start()
        {
            if (_health == 0) _health = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
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
            _health = (float) state;
            if (_health == 0)
            {
                Die();
            }
        }
    }
}
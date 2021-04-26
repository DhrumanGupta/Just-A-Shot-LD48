using System;
using Game.Saving;
using UnityEngine;

namespace Game.Combat
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _maxHealth = 100f;
        private float _health;

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

            GetComponent<Animator>().SetTrigger("die");
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


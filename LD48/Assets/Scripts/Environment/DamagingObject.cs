using System;
using Game.Combat;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class DamagingObject : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        private Health _target = null;

        public void SetData(int damage, Health target)
        {
            _damage = damage;
            _target = target;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Health collision)) return;
            if (_target == null || collision == _target) collision.TakeDamage(_damage);
        }
    }
}
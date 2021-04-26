using System;
using Game.Combat;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public class DamagingObject : MonoBehaviour
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private Health _target = null;

        public void SetData(float damage, Health target)
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
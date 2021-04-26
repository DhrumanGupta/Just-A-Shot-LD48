using UnityEngine;

namespace Game.Combat
{
    public class Projectile : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private float _speed = 1;

        [SerializeField] private bool _isHoming = true;

        [Header("Effects")]
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private float _maxLifetime = 15f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private float _lifeAfterImpact = 3f;

        private Health _target = null;
        private CapsuleCollider _targetCapsule;
        private float _damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            if (_isHoming && !_target.IsDead)
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        }

        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
            _targetCapsule = target.GetComponent<CapsuleCollider>();
            Destroy(gameObject, _maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            return _target.transform.position + (Vector3.up * _targetCapsule.height / 2);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_target.IsDead && other.GetComponent<Health>() == _target)
            {
                _target.TakeDamage(_damage);
            }

            _speed = 0f;

            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (var toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }
}
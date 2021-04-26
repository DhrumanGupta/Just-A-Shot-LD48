using System;
using System.Linq;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class DestructibleObject : MonoBehaviour
    {
        [SerializeField] private int _hitsToDestroy = 1;
        [SerializeField] private Sprite[] _sprites;

        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private int _hitsTaken;

        [SerializeField] private GameObject _drop = null;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (_sprites.Length > _hitsToDestroy + 1) _sprites = _sprites.Take(_hitsToDestroy + 1).ToArray();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.CompareTag("Player")) return;
            if (Mathf.Abs(other.relativeVelocity.y) < 0.5f) return;
            
            // If collision was from not the top, dont damage the object
            var contact = other.GetContact(0);
            if (Vector3.Dot(contact.normal, Vector3.down) <= 0.7f) return;
                
            DamageObject();
        }

        private void DamageObject()
        {
            _hitsTaken++;
            if (_hitsTaken <= _sprites.Length - 1) _spriteRenderer.sprite = _sprites[_hitsTaken];

            if (_hitsTaken < _hitsToDestroy) return;
            _collider.enabled = false;
            if (_drop != null) Instantiate(_drop, transform.position, Quaternion.identity);
        }
    }
}
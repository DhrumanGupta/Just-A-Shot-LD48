using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Obstacles
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PressurePlate : TimedEvent
    {
        [SerializeField] private UnityEvent _onPressurePlateDown = null;
        [SerializeField] private UnityEvent _onPressurePlateUp = null;

        private Vector3 _startPos;
        private Vector3 _endPos;

        [SerializeField] private float _pressDistance = 1f;
        [SerializeField] private float _timeBeforeUp = 5f;

        private bool _isPressed;

        private void Awake()
        {
            _startPos = transform.position;
            _endPos = _startPos - new Vector3(0, _pressDistance, 0);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            if (collision.contacts.Length <= 0) return;

            var contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.down) <= 0.7) return;

            _isPressed = true;

            CancelTimer();
            transform.position = _endPos;
            _onPressurePlateDown?.Invoke();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!_isPressed) return;
            base.StartTimer(_timeBeforeUp, HandleTimerFinished);
            _isPressed = false;
        }

        private void HandleTimerFinished()
        {
            transform.position = _startPos;
            _onPressurePlateUp?.Invoke();
        }
    }
}

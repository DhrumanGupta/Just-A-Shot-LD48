using System.Linq;
using UnityEngine;

namespace Game.Control
{
    public class CameraController : MonoBehaviour
    {
        [Header("Customization")]
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private float _smooth = .4f;
        
        private Vector3 _velocity = Vector3.zero;

        [SerializeField] private Transform _player = null;
        private Camera _camera = null;
        private Transform _transform;

        private void Start()
        {
            _transform = GetComponent<Transform>();
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            var bounds = GetBounds();

            Move(bounds);
        }

        private void Move(Bounds bounds)
        {
            Vector3 newPos = _player.position - _offset;
            _transform.position = Vector3.SmoothDamp(_transform.position, newPos, ref _velocity, _smooth);
        }

        private Bounds GetBounds()
        {
            var position = _player.position;
            
            var bounds = new Bounds(position, Vector3.zero);
            bounds.Encapsulate(position);

            return bounds;
        }
    }
}
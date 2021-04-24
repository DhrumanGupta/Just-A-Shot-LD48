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
        private new Transform _transform;
        
        // private Transform[] _players;
        //
        // [SerializeField] private float _minZoom = 10f;
        // [SerializeField] private float _maxZoom = 40f;
        // [SerializeField] private float _zoomLimiter = 10f;

        private void Start()
        {
            _transform = GetComponent<Transform>();
            _camera = GetComponent<Camera>();
            // _players = PlayerController.Controllers?.Select(x => x.Transform).ToArray() ?? new Transform[0];
        }

        private void LateUpdate()
        {
            var bounds = GetBounds();

            Move(bounds);
            // Zoom(bounds);
        }

        // private void Zoom(Bounds bounds)
        // {
        //     float greatestDistance = bounds.size.x;
        //
        //     float newZoom = Mathf.Lerp(_minZoom, _maxZoom, greatestDistance / _zoomLimiter);
        //     _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newZoom, Time.deltaTime * 1.5f);
        // }

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
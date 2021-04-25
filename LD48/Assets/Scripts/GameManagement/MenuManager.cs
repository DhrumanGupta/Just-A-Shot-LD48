using System;
using UnityEngine;

namespace Game.GameManagement
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private Camera _camera;
        
        [Space]
        [SerializeField] private Placeable[] _left = null;
        [SerializeField] private Placeable[] _right = null;
        
        [Space]
        [SerializeField] private GameObject _optionsPanel;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void Start()
        {
            var positionOffset = _camera.transform.position.x;

            var leftPos = -_camera.orthographicSize * _camera.aspect + positionOffset;
            var rightPos = _camera.orthographicSize * _camera.aspect + positionOffset;

            foreach (var item in _left)
            {
                SetPositiom(item.Transform, leftPos + item.Offset);
            }

            foreach (var item in _right)
            {
                SetPositiom(item.Transform, rightPos + item.Offset);
            }
        }
        

        private void SetPositiom(Transform transform, float pos)
        {
            var position = transform.position;
            position.x = pos;
            
            transform.position = position;
        }

        public void OpenSettings()
        {
            
        }

        [Serializable]
        public class Placeable
        {
            public Transform Transform;
            public float Offset;
        }
    }
}
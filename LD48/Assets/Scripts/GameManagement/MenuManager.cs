using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.GameManagement
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        
        [SerializeField] private Placeable[] _left, _right = null;

        [Space]
        [SerializeField] private Volume _postProcessingVolume = null;
        [SerializeField] private GameObject _helpButton, _helpPanel  = null;

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

        public void OpenHelp()
        {
            _helpButton.SetActive(false);
            _helpPanel.SetActive(true);
            _postProcessingVolume.enabled = true;
        }
        

        [Serializable]
        public class Placeable
        {
            public Transform Transform;
            public float Offset;
        }
    }
}
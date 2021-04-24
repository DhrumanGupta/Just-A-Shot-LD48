using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private Transform[] _backgrounds = null;
        private float[] _parallaxScales;

        [Range(0.01f, 1)]
        [SerializeField] private float _smooth = 1f;

        private Transform _camera;
        private Vector3 _previousCameraPosition;

        private void Awake()
        {
            _camera = Camera.main.transform;

            _previousCameraPosition = _camera.position;
            _parallaxScales = new float[_backgrounds.Length];

            for (int i = 0; i < _backgrounds.Length; i++)
            {
                _parallaxScales[i] = _backgrounds[i].position.z * -1;
            }
        }

        private void Update()
        {
            for (int i = 0; i < _backgrounds.Length; i++)
            {
                float parallax = (_previousCameraPosition.x - _camera.position.x) * _parallaxScales[i];

                var background = _backgrounds[i];
                var position = background.position;
                Vector3 backgroundTargetPos = new Vector3(position.x + parallax, position.y, position.z);

                background.position = Vector3.Lerp(background.position, backgroundTargetPos, _smooth * Time.deltaTime);
            }

            _previousCameraPosition = _camera.position;
        }
    }

}
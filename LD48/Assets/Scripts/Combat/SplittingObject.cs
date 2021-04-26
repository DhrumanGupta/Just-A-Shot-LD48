using System;
using UnityEngine;

namespace Game.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SplittingObject : MonoBehaviour
    {
        [SerializeField] private int _splitInto = 3;
        [SerializeField] private GameObject _splitObject = null;
        [SerializeField] private float _splitAfter = 3f;

        private float _spawnTime;
        private float _timeToSplitAt;
        
        private void Update()
        {
            _spawnTime = Time.time;
            _timeToSplitAt = _spawnTime + _splitAfter;

            if (Time.time >= _timeToSplitAt)
                Split();
        }

        private void Split()
        {
            var rigidbody = GetComponent<Rigidbody2D>();
            var dir = rigidbody.velocity;

            var angleEachTurn = 45f;
            var halfAngle = -((_splitInto - 1) * angleEachTurn);

            for (int i = 0; i < _splitInto; i++)
            {
                var angle = halfAngle + i * angleEachTurn;
                var splitDir = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
            }
        }
    }
}
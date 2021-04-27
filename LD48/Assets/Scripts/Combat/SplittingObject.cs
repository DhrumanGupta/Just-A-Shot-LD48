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

        private void Start()
        {
            _spawnTime = Time.time;
            _timeToSplitAt = _spawnTime + _splitAfter;
        }

        private void Update()
        {
            if (Time.time >= _timeToSplitAt)
                Split();
        }

        private void Split()
        {
            var rigidbody = GetComponent<Rigidbody2D>();
            var dir = (Vector3) rigidbody.velocity.normalized;

            var angleEachTurn = 45f;
            var halfAngle = -((_splitInto - 1) * angleEachTurn);

            for (int i = 1; i <= _splitInto; i++)
            {
                var angle = halfAngle + i * angleEachTurn;
                var splitDir = new Vector3(0, 0, Mathf.Cos(Mathf.Deg2Rad * angle));
                var spawned = Instantiate(_splitObject, transform.position, Quaternion.Euler((splitDir + dir) * angle)).transform;
                spawned.position += spawned.right * 3f;
            }
            
            
            Destroy(gameObject);
        }
    }
}
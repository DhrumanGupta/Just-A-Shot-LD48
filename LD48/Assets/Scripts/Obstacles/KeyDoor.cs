using Game.GameManagement;
using UnityEngine;

namespace Game.Obstacles
{
    [RequireComponent(typeof(Door))]
    public class KeyDoor : MonoBehaviour
    {
        [SerializeField] private int _keysToUnlock = 1;
        private Door _door;

        private void Awake()
        {
            _door = GetComponent<Door>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            if (!KeyManager.Instance.CanUseKeys(_keysToUnlock)) return;
            
            if (_keysToUnlock > 0)
            {
                KeyManager.Instance.UseKeys(_keysToUnlock);
            }
            _door.SetState(true);
        }
    }
}

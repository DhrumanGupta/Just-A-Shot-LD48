using Game.GameManagement;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Key : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                KeyManager.Instance.AddKeys(1);
                Destroy(gameObject);
            }
        }
    }
}


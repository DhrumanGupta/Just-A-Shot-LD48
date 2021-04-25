using Game.GameManagement;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Key : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                KeyManager.Instance.AddKeys(1);
                Destroy(gameObject);
            }
        }
    }
}


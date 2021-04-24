using UnityEngine;

namespace Game.Management
{
    public class KeyManager : MonoBehaviour
    {
        public static KeyManager Instance { get; private set; }

        private int _keys;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }

        public bool CanUseKeys(int keys)
        {
            return _keys >= keys;
        }

        public void UseKeys(int keys)
        {
            _keys -= keys;
        }

        public void AddKeys(int keys)
        {
            _keys += keys;
        }
    }
}
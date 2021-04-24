using UnityEngine;

namespace Game.Management
{
    public class KeyManager : MonoBehaviour
    {
        public static KeyManager Instance { get; private set; }

        private int _nKeys;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void UseKey()
        {
            _nKeys--;
        }

        public void AddKey()
        {
            _nKeys++;
        }
    }
}
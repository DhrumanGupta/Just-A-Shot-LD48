using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GameManagement
{
    public class AudioManager : MonoBehaviour
    {
        public static KeyManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }

        public void PlayMusic(AudioClip){

        }
    }
}

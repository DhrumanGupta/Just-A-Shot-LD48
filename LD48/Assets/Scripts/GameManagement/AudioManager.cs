using UnityEngine;

namespace Game.GameManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private AudioSource _audioSource = null;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(transform.root);

            _audioSource = GetComponent<AudioSource>();
        }
        
        public AudioClip CurrentClip => _audioSource.clip;

        public void PlayMusic(AudioClip clip)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}

using System;
using UnityEngine;

namespace Game.GameManagement
{
    public class PlayAudioOnAwake : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioClip = null;
        
        private void Awake()
        {
            if (_audioClip == null || AudioManager.Instance.CurrentClip == _audioClip) return;
            AudioManager.Instance.PlayMusic(_audioClip);
        }
    }
}
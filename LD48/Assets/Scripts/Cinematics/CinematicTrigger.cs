using UnityEngine;

namespace Game.Cinematics
{
    [RequireComponent(typeof(Cinematic))]
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _hasPlayed = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_hasPlayed) return;
            _hasPlayed = true;
            GetComponent<Cinematic>().Play();
        }
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneManagement
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination = DestinationIdentifier.A;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1.5f;
        [SerializeField] float fadeWaitTime = .5f;

        private bool _isTransitioning = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (_isTransitioning) return;
                StartCoroutine(TransitionToScene());
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator TransitionToScene()
        {
            _isTransitioning = true;
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to lead not set (portal variable).");
                yield break;
            }

            Fader fader = FindObjectOfType<Fader>();

            DontDestroyOnLoad(gameObject);

            if (fader == null)
            {
                print("fader not found");
                yield break;
            }

            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            var player = GameObject.FindWithTag("Player");
            player.transform.position = otherPortal.spawnPoint.position;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this || portal.destination != destination) continue;

                return portal;
            }
            return null;
        }
        
        #region Gizmos

        private void OnDrawGizmos()
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            Gizmos.color = new Color32(255, 0, 0, 100);
            Gizmos.DrawCube(bounds.center, bounds.size);
        }

        #endregion
    }
}

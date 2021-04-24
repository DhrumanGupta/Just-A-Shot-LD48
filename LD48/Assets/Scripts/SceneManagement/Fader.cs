using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SceneManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private float _alpha;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImediate()
        {
            _canvasGroup.alpha = 1;
        }

        public IEnumerator FadeIn(float time)
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeOut(float time)
        {
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }      
        }
    }
}

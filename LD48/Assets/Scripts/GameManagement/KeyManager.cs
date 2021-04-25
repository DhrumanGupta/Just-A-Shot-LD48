using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.GameManagement
{
    public class KeyManager : MonoBehaviour
    {
        public static KeyManager Instance { get; private set; }
        private int _keys;

        [Header("Key UI")]
        [SerializeField] private RectTransform _keySprite = null;
        [SerializeField] private TextMeshProUGUI _keyCountText = null;

        [Header("Key Animation")]
        [SerializeField] private AnimationCurve _scaleUpCurve = null;
        [SerializeField] private AnimationCurve _scaleDownCurve = null;
        [SerializeField] private float _scaleUpDuration = 0.3f;
        [SerializeField] private float _scaleDownDuration;
        [SerializeField] private float _scaleFactor = 1.5f;

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
            UpdateUI();
        }

        public void AddKeys(int keys)
        {
            _keys += keys;
            UpdateUI();
        }

        private void UpdateUI()
        {
            StartCoroutine(PopupKey());
            StartCoroutine(ChangeKeyCount());
        }

        private IEnumerator PopupKey()
        {
            Vector3 startScale = _keySprite.localScale;
            Vector3 endScale = startScale * _scaleFactor;
            float start = Time.time;

            while (Time.time < start + _scaleUpDuration)
            {
                float completion = (Time.time - start) / _scaleUpDuration;
                _keySprite.localScale = Vector3.Lerp(startScale, endScale, _scaleUpCurve.Evaluate(completion));
                yield return null;
            }
            
            start = Time.time;
            
            while (Time.time < start + _scaleDownDuration)
            {
                float completion = (Time.time - start) / _scaleDownDuration;
                _keySprite.localScale = Vector3.Lerp(endScale, startScale, _scaleDownCurve.Evaluate(completion));
                yield return null;
            }

            _keySprite.localScale = startScale;
        }

        private IEnumerator ChangeKeyCount()
        {
            var wait = new WaitForSeconds(0.05f);
            
            var currentCount = int.Parse(_keyCountText.text);
            var operation = currentCount > _keys ? -1 : 1;
            while (currentCount != _keys)
            {
                currentCount += operation;

                _keyCountText.SetText(currentCount.ToString());
                yield return wait;
            }
        }
    }
}
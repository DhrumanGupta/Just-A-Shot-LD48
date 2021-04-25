using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Environment
{
    public class Npc : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _chatBox = null;
        [SerializeField] private TextMeshPro _textMesh = null;
        
        [Space]
        [SerializeField] private SpeakableLines[] _linesToSay = null;
        [SerializeField] private SpeakableLines[] _linesToSayAfter = null;

        protected bool HasInteracted = false;
        private bool _isInteracting = false;
        protected event Action OnInteractionComplete;

        private void Awake()
        {
            if (_chatBox == null)
                _chatBox = transform.GetChild(0).gameObject;

            if (_chatBox == null)
            {
                Debug.LogError($"ChatBox for GameObject '{gameObject.name}' not assigned.");
            }

                if (_textMesh == null)
                _textMesh = _chatBox.transform.GetChild(0).GetComponent<TextMeshPro>();
        }

        public IEnumerator Interact()
        {
            if (_isInteracting) yield break;
            yield return SpeakLines();
        }

        private IEnumerator SpeakLines()
        {
            _isInteracting = true;
            
            var characterWait = new WaitForSeconds(0.02f);
            
            _textMesh.SetText("");
            _chatBox.SetActive(true);

            var lines = _linesToSay;
            if (HasInteracted && _linesToSayAfter != null && _linesToSayAfter.Length > 0)
            {
                lines = _linesToSayAfter;
            }
            
            foreach (var line in lines)
            {
                _textMesh.SetText("");
                foreach (var character in line.Line)
                {
                    _textMesh.text += character;
                    yield return characterWait;
                }

                yield return new WaitForSeconds(line.TimeToWait);
            }

            yield return new WaitForSeconds(1f);
            
            _chatBox.SetActive(false);
            _isInteracting = false;
            
            OnInteractionComplete?.Invoke();
            
            HasInteracted = true;
        }
    }

    [Serializable]
    public class SpeakableLines
    {
        [field: SerializeField] public string Line { get; set; }
        [field: SerializeField] public float TimeToWait { get; set; }
    }
}

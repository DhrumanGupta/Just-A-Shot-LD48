using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Interaction
{
    public class Npc : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpeakableLines[] _linesToSay = null;
        [SerializeField] private TextMeshPro _textMesh = null;

        private bool _isInteracting = false;
        
        public void Interact()
        {
            if (_isInteracting) return;
            StartCoroutine(SpeakLines());
        }

        private IEnumerator SpeakLines()
        {
            _isInteracting = true;
            var characterWait = new WaitForSeconds(0.03f);
            
            foreach (var line in _linesToSay)
            {
                _textMesh.SetText("");
                foreach (var character in line.Line)
                {
                    _textMesh.text += character;
                    yield return characterWait;
                }

                yield return new WaitForSeconds(line.TimeToWait);
            }

            _isInteracting = false;
        }
        
    }

    [Serializable]
    public class SpeakableLines
    {
        [field: SerializeField] public string Line { get; set; }
        [field: SerializeField] public float TimeToWait { get; set; }
    }
}

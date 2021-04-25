using System;
using System.Collections;
using Game.Environment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.GameManagement
{
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] private GameObject _player = null;
        [SerializeField] private GameObject _npcPlayer = null;

        private IEnumerator Start()
        {
            _player.SetActive(false);
            _npcPlayer.SetActive(true);
            
            yield return new WaitForSeconds(1.5f);
            yield return _npcPlayer.GetComponent<IInteractable>().Interact();
            
            _player.transform.position = _npcPlayer.transform.position;
            _player.SetActive(true);
            _npcPlayer.SetActive(false);
        }
    }
}
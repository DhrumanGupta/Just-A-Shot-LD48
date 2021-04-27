using System.Collections;
using Game.Environment;
using UnityEngine;

namespace Game.GameManagement
{
    public class EndingManager : MonoBehaviour
    {
        [SerializeField] private GameObject _endingNpc = null;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            yield return _endingNpc.GetComponent<IInteractable>().Interact();
        }
    }   
}

using System.Collections;
using Game.Environment;
using Game.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.GameManagement
{
    public class EndingManager : MonoBehaviour
    {
        [SerializeField] private GameObject _endingNpc = null;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            yield return _endingNpc.GetComponent<IInteractable>().Interact();
            
            yield return new WaitForSeconds(1f);
            GameObject.FindObjectOfType<SavingWrapper>().Delete();

            SceneManager.LoadScene(0);
        }
    }   
}

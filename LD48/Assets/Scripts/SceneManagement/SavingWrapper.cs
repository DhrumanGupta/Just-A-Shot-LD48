using System.Collections;
using Game.Control;
using Game.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string _defaultSaveFile = "save_file";
        [SerializeField] private float fadeInTime = 1f;
        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
        }

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImediate();
            yield return fader.FadeIn(fadeInTime);
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

#endif

        public void Save()
        {
            // Call saving system to save
            _savingSystem.Save(_defaultSaveFile);
        }

        public void Load()
        {
            // Call saving system to load
            _savingSystem.Load(_defaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(_defaultSaveFile);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
            _savingSystem.Load(_defaultSaveFile);
        }
    }
}
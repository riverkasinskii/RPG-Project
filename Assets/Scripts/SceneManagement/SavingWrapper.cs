using UnityEngine;
using System.Collections;
using RPG.Saving;

namespace RPG.SceneManagement
{    
    public class SavingWrapper : MonoBehaviour
    {
        private const string DEFAULT_SAVE_FILE = "save";

        [SerializeField] private float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {            
            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyUp(KeyCode.S))
            { 
                Save(); 
            }
            if(Input.GetKeyUp(KeyCode.Delete))
            {
                Delete();
            }

        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(DEFAULT_SAVE_FILE);
        }
    }
}
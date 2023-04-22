using UnityEngine;

namespace RPG.Core
{    
    public class PersistantObjectsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistantObjectPrefab;

        private static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned)
            {
                return;
            }
            SpawnPersistantObjects();
            hasSpawned = true;            
        }

        private void SpawnPersistantObjects()
        {
            GameObject persistantObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistantObject);
        }
    }
}


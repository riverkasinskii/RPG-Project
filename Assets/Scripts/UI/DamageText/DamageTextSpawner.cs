using UnityEngine;

namespace RPG.UI.DamageText
{    
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab = null;

        private void Start()
        {
            Spawn(11);
        }

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}

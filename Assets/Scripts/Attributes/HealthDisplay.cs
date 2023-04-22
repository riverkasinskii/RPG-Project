using TMPro;
using UnityEngine;

namespace RPG.Attributes
{    
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = string.Format(
                "{0:0}/{1:0}", 
                health.GetHealthPoints(), 
                health.GetMaxHealthPoints());
        }
    }
}

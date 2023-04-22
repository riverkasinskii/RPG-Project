using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{    
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTargetHealth() == null)
            {
                GetComponent<TextMeshProUGUI>().text = "N/A";
                return;
            }
            Health health = fighter.GetTargetHealth();
            GetComponent<TextMeshProUGUI>().text = string.Format(
                "{0:0}/{1:0}",
                health.GetHealthPoints(),
                health.GetMaxHealthPoints());
        }
    }
}

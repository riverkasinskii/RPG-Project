using TMPro;
using UnityEngine;

namespace RPG.Stats
{    
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience experience;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = string.Format("{0:0}", experience.GetPoints());
        }
    }
}

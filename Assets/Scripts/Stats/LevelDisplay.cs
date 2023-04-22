using TMPro;
using UnityEngine;

namespace RPG.Stats
{    
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats baseStats;
        private TextMeshProUGUI getLevelUI;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            getLevelUI = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            getLevelUI.text = string.Format("{0:0}", baseStats.GetLevel());
        }
    }
}

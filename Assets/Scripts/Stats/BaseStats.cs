using RPG.Saving;
using RPG.Utils;
using System;
using UnityEngine;

namespace RPG.Stats
{    
    public class BaseStats : MonoBehaviour, ISaveable
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;        
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;

        public event Action OnLevelUp;

        private LazyValue<int> currentLevel;        

        private Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();            
            currentLevel = new LazyValue<int>(CalculateLevel);           
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }                

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }

        public int GetLevel()
        {               
            return currentLevel.Value;
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }                
        
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) { return startingLevel; }

            float currentXP = experience.GetPoints();

            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

                if(XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }                

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.Value)
            {
                currentLevel.Value = newLevel;
                LevelUpEffect();
                OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public object CaptureState()
        {
            return GetLevel();
        }

        public void RestoreState(object state)
        {
            currentLevel.Value = (int)state;
        }
    }
}

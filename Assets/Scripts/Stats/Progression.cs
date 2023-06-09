using UnityEngine;
using System;
using System.Collections.Generic;

namespace RPG.Stats
{    
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]    
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level) { return 0f; }

            return levels[level - 1];            
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.Stat] = progressionStat.Levels;
                }

                lookupTable[progressionClass.CharacterClass] = statLookupTable;
            }
            
        }

        [Serializable]
        private class ProgressionCharacterClass
        {
            public ProgressionStat[] stats;

            [SerializeField] private CharacterClass characterClass;

            public CharacterClass CharacterClass
            {
                get
                {
                    return characterClass;
                }
            }                        
        }

        [Serializable]
        private class ProgressionStat
        {
            [SerializeField] private Stat stat;
            public Stat Stat
            {
                get
                {
                    return stat;
                }
            }

            [SerializeField] private float[] levels;
            public float[] Levels
            {
                get
                {
                    return levels;
                }
            }
        }
    }
}
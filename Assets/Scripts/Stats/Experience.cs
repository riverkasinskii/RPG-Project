using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{    
    public class Experience : MonoBehaviour, ISaveable
    {        
        public event Action OnExperienceGained;
        [SerializeField] private float experiencePoints = 0f;
                                        
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            OnExperienceGained();
        }

        public object CaptureState()
        {            
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        public float GetPoints()
        {
            return experiencePoints;
        }
    }
}

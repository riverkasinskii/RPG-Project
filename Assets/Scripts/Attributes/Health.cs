using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using UnityEngine.Events;
using System;
using RPG.Utils;

namespace RPG.Attributes
{    
    public class Health : MonoBehaviour, ISaveable
    {
        [Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        [SerializeField] private float regeneratePercentage = 70;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private UnityEvent onDie;

        private Animator animator;
        private ActionScheduler actionScheduler;  
        
        private LazyValue<float> healthPoints;

        private bool isDead = false;

        private void Start()
        {
            healthPoints.ForceInit();            
        }

        private void Awake()
        {
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();            
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }                

        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }               

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {      
            healthPoints.Value = Mathf.Max(healthPoints.Value - damage, 0);
            
            if (healthPoints.Value <= 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        } 
        
        public float GetHealthPoints()
        {
            return healthPoints.Value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentageHealth()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthPoints.Value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {            
            return healthPoints.Value;
        }

        public void RestoreState(object state)
        {
            healthPoints.Value = (float)state;

            if (healthPoints.Value <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead)
            {
                return;
            } 
            isDead = true;
            animator.SetTrigger("Die");
            actionScheduler.CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * 
                                                                (regeneratePercentage / 100);
            healthPoints.Value = Mathf.Max(healthPoints.Value, regenHealthPoints);
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.Value = Mathf.Min(healthPoints.Value + healthToRestore, GetMaxHealthPoints());
        }
    }
}

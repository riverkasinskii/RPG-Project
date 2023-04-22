using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;
using RPG.Utils;

namespace RPG.Combat
{           
    public class Fighter : MonoBehaviour, IAction, IModifierProvider, ISaveable
    {
        
        [SerializeField] private float timeBetweenAttacks = 1f;               
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;        

        private Health target;
        private Animator animator;
        private Mover mover;
        
        private float timeSinceLastAttack = Mathf.Infinity;

        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;

        private void Start()
        {
            currentWeapon.ForceInit();                                 
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            currentWeaponConfig = defaultWeapon;
        }                

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null)
            {                
                return;
            }
            if (target.IsDead())
            {
                return;
            }

            if (!GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();                
                AttackBehaviour();                
            }
        }
                
        public void Attack(GameObject combatTarget)
        {            
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; } 
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) 
            { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.Value = AttachWeapon(weapon);
        }               

        public Health GetTargetHealth()
        {
            return target;
        }

        public object CaptureState()
        {            
            return currentWeaponConfig.name;            
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);            
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                // This will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("Attack");
        }

        // Animation Attack Event
        private void Hit()
        {
            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.Value != null)
            {
                currentWeapon.Value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, 
                                               leftHandTransform, 
                                               target,
                                               gameObject,
                                               damage);
            }
            else
            {                
                target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position)
                < currentWeaponConfig.GetRange();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("stopAttack");
        }                 
    }
}

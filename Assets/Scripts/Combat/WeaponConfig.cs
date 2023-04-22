using RPG.Attributes;
using System;
using UnityEngine;

namespace RPG.Combat
{    
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    [Serializable]        
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 0f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string WEAPON_NAME = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = WEAPON_NAME;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(overrideController != null)
            {                   
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;                
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME) ?? leftHand.Find(WEAPON_NAME);
            if (oldWeapon == null) { return; }
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(
            Transform rightHand, 
            Transform leftHand, 
            Health target, 
            GameObject instigator,
            float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile,
                GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }
    }
}

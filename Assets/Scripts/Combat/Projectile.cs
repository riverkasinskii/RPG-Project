using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{    
    public class Projectile : MonoBehaviour
    {        
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 1f;
        [SerializeField] private UnityEvent onHit;

        private Health target = null;
        private GameObject instigator = null;
        private float damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target == null) { return; }
            if (isHoming && target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }            
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) 
            { 
                return target.transform.position; 
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) { return; }
            if (target.IsDead()) { return; }
            target.TakeDamage(instigator, damage);

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);                        
        }
    }
}

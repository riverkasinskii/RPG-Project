using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine;
using RPG.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 2f;
        [SerializeField] private float agroCooldownTime = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float wayPointTolerance = 1f;
        [SerializeField] private float wayPointDwellTime = 3f;

        [Range(0f, 1f)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float shoutDistance = 5f;

        private Fighter fighter;
        private Health health;
        private GameObject player;
        private Mover mover;

        private LazyValue<Vector3> guardPosition;

        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWayPoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private int currentWayPointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }                

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (suspicionTime >= timeSinceLastSawPlayer)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWayPoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.Value;

            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArrivedAtWayPoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }
            if (timeSinceArrivedAtWayPoint > wayPointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position,
                                                      GetCurrentWayPoint());

            return distanceToWayPoint < wayPointTolerance;
        }

        private void CycleWayPoint()
        {
            currentWayPointIndex = patrolPath.GetNextIndex(currentWayPointIndex);
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(currentWayPointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearByEnemies();
        }

        private void AggrevateNearByEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) { continue; }

                ai.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position,
                                                      transform.position);

            return distanceToPlayer < chaseDistance || timeSinceAggrevated < agroCooldownTime;            
        }        
    }
}

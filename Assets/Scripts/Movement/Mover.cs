using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{    
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLength = 40f;

        private NavMeshAgent agent;
        private Health health;
        private Animator animator;
                
        private void Awake()
        {
            health = GetComponent<Health>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if(health.IsDead())
            {
                agent.enabled = false;
            }
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new();
            bool hasPath = NavMesh.CalculatePath(transform.position,
                destination,
                NavMesh.AllAreas,
                path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {            
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;            
        }

        public void Cancel()
        {
            agent.isStopped = true;            
        }                

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0f;
            if (path.corners.Length < 2) { return total; }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);            
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            agent.enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            agent.enabled = true;
        }
    }
}

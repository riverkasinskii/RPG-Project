using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{    
    public class PlayerController : MonoBehaviour
    {        
        [Serializable]
        private struct CursorMapping
        {
            [SerializeField] private CursorType cursorType;
            public CursorType CursorType 
            { 
                get 
                { 
                    return cursorType; 
                } 
            }

            [SerializeField] private Texture2D texture;
            public Texture2D Texture
            {
                get
                {
                    return texture;
                }
            }

            [SerializeField] private Vector2 hotspot;
            public Vector2 Hotspot
            {
                get
                {
                    return hotspot;
                }
            }
        }

        private Health health;
        private Mover mover;

        [SerializeField] private CursorMapping[] cursorMappings = null;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float rayCastRadius = 1f;

        private void Awake()
        {
            health = GetComponent<Health>();             
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (InteractWithUI()) { return; }

            if (health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return; 
            }            

            if (InteractWithComponent()) { return; }                        
              
            if (InteractWithMovement()) { return; }              
                        
            SetCursor(CursorType.None);
        }
                
        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }            
            return false;
        }                

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), rayCastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }
                
        private bool InteractWithMovement()
        {            
            bool hasHit = RayCastNavMesh(out Vector3 target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) { return false; }
                
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RayCastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);

            if (!hasHit) { return false; }

            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, 
                out NavMeshHit navMeshHit, 
                maxNavMeshProjectionDistance, 
                NavMesh.AllAreas);

            if (!hasCastToNavMesh) { return false; }

            target = navMeshHit.position;                                

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.Texture, mapping.Hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.CursorType == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

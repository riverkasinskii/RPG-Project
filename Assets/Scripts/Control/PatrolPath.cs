using UnityEngine;

namespace RPG.Control
{    
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            const float wayPointGizmosRadius = 0.2f;
            Gizmos.color = Color.white;

            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWayPoint(i), wayPointGizmosRadius);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWayPoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}


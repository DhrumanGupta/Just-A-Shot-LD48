using UnityEngine;

namespace Game.Core
{
    public class Path : MonoBehaviour
    {
        #if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeGameObject == this.gameObject)
            {
                Gizmos.color = new Color(192,192,192,0.75f);
            }
            else
            {
                Gizmos.color = new Color(192,192,192,0.25f);
            }

            const float sphereRadius = 0.2f;
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), sphereRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }
        
        #endif

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
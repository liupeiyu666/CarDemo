using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWS
{
    [RequireComponent(typeof(PathManager))]
    public class ShowWayForward : MonoBehaviour
    {
        public bool Show;
        public Color color= Color.red;
        private PathManager _manager;
        private PathManager manager
        {
            get { return _manager == null ? GetComponent<PathManager>() : _manager; }
        }

        void OnDrawGizmos()
        {
            if (Show)
            {
                for (int i = 0; i < manager.waypoints.Length; i++)
                {
                    Transform sg = manager.waypoints[i];
                    Gizmos.color = color;
                    Gizmos.DrawLine(sg.transform.position, sg.transform.position + sg.transform.forward*2);
                }
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityDungeonGenerator
{
    // Holds static data about the dungeon part
    public class PartPrefab : MonoBehaviour
    {
        // Stop part from being rotated in an unfortunate manner (We don't want corridors that point up, do we?)
        [SerializeField]
        public bool m_xLock, m_yLock, m_zLock;

        // Size in voxels of the part
        [SerializeField]
        public Vector3 m_Size;

        // GameObjects that hold the location of connected doors (Fill function to check door if wall or not)
        [SerializeField]
        public List<GameObject> m_ConnectionGOs;

        public void Awake()
        {

        }

        public void Start()
        {
            if (m_ConnectionGOs == null)
            {
                m_ConnectionGOs = new List<GameObject>();

                // Will remember how to populate later
            }
        }

        public List<Vector3> GetSpace(Vector3 _center)
        {
            List<Vector3> l_toReturn = new List<Vector3>();
            float l_voxelCount = m_Size.x * m_Size.y * m_Size.z;

            while (l_toReturn.Count < l_voxelCount)
            {
                
            }

                // for (int x = 0; x <= m_Size.x; x++)
                // {
                //     for (int y = 0; y <= m_Size.y; y++)
                //     {
                //         for (int z = 0; z <= m_Size.z; z++)
                //         {
                //             l_toReturn.Add(new Vector3(x, y, z) + _center);
                //         }
                //     }
                // }

                return l_toReturn;
        }
    }
}
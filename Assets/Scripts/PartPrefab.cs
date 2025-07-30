using System;
using System.Collections.Generic;
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

        private List<Vector3> m_Coordinates;

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

        public List<Vector3> GetCoordinates(Vector3 _center)
        {
            if (m_Coordinates == null || m_Coordinates.Count == 0)
            {
                m_Coordinates = new List<Vector3>();
                float l_voxelCount = m_Size.x * m_Size.y * m_Size.z;
                Vector3 negBound = _center - (new Vector3(
                                                        (int)(m_Size.x * 0.5f),
                                                        (int)(m_Size.y * 0.5f),
                                                        (int)(m_Size.z * 0.5f)
                                            ));
            
                Debug.Log("Dungeon Generator: Lower bound for GetSpace: \nBound: " + negBound);

                Vector3 posBound = _center + (new Vector3(
                                                        (int)(m_Size.x * 0.5f),
                                                        (int)(m_Size.y * 0.5f),
                                                        (int)(m_Size.z * 0.5f)
                                            ));
            
                Debug.Log("Dungeon Generator: Upper bound for GetSpace: \nBound: " + posBound);

                Vector3 l_currentCoord = negBound;
                for (int x = (int)negBound.x; x <= posBound.x; x++)
                {
                    for (int y = (int)negBound.y; y <= posBound.y; y++)
                    {
                        for (int z = (int)negBound.z; z <= posBound.z; z++)
                        {
                            m_Coordinates.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }

            string message = "Dungeon Generator: Coordinates from GetSpace: " + m_Coordinates.Count + "\n";

            foreach (Vector3 coord in m_Coordinates)
            {
                message += coord;
            }

            Debug.Log(message);

            return m_Coordinates;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    // Holds static data about the dungeon part
    public class PartPrefab : MonoBehaviour
    {
        // Size in voxels of the part
        [SerializeField]
        public Vector3 m_Size;

        // GameObjects that hold the location of connected doors (Fill function to check door if wall or not)
        [SerializeField]
        public List<GameObject> m_ConnectionGOs;

        private List<Vector3> m_Coordinates;

        public List<Vector3> GetCoordinates(Vector3 _center)
        {
            Debug.Log("Dungeon Generator: GetCoordinates of object center: " + _center);
            m_Coordinates = new List<Vector3>();
            float l_voxelCount = m_Size.x * m_Size.y * m_Size.z;
            Debug.Log("Dungeon Generator: Suspected voxel count: " + l_voxelCount);

            Vector3 negBound = _center - (new Vector3(
                                                    (int)(m_Size.x * 0.5f),
                                                    (int)(m_Size.y * 0.5f),
                                                    (int)(m_Size.z * 0.5f)
                                        ));

            if (negBound.x - (int)negBound.x == 0.5f)
            {
                negBound.x += 0.5f;
            }

            if (negBound.y - (int)negBound.y == 0.5f)
            {
                negBound.y += 0.5f;
            }

            if (negBound.z - (int)negBound.z == 0.5f)
            {
                negBound.z += 0.5f;
            }

            Debug.Log("Dungeon Generator: Lower bound for GetSpace: \nBound: " + negBound);

            Vector3 posBound = _center + (new Vector3(
                                                    (int)(m_Size.x * 0.5f),
                                                    (int)(m_Size.y * 0.5f),
                                                    (int)(m_Size.z * 0.5f)
                                        ));

            if (posBound.x - (int)posBound.x == 0.5f)
            {
                posBound.x -= 0.5f;
            }

            if (posBound.y - (int)posBound.y == 0.5f)
            {
                posBound.y -= 0.5f;
            }

            if (posBound.z - (int)posBound.z == 0.5f)
            {
                posBound.z -= 0.5f;
            }

            Debug.Log("Dungeon Generator: Upper bound for GetSpace: \nBound: " + posBound);

            Vector3 l_currentCoord = negBound;
                // while (l_currentCoord != posBound)
                // {
                //     m_Coordinates.Add(new Vector3(
                //         (int)l_currentCoord.x,
                //         (int)l_currentCoord.y,
                //         (int)l_currentCoord.z
                //     ));

                //     if (l_currentCoord.x != posBound.x)
                //     {
                //         l_currentCoord.x += 1;
                //     }
                //     else if (l_currentCoord.y != posBound.y)
                //     {
                //         l_currentCoord.x = negBound.x;
                //         l_currentCoord.y += 1;
                //     }
                //     else if (l_currentCoord.z != posBound.z)
                //     {
                //         l_currentCoord.x = negBound.x;
                //         l_currentCoord.y = negBound.y;
                //         l_currentCoord.z += 1;
                //     }
                // }

                for (int x = (int)negBound.x; x <= (int)posBound.x; x++)
                {
                    for (int y = (int)negBound.y; y <= (int)posBound.y; y++)
                    {
                        for (int z = (int)negBound.z; z <= (int)posBound.z; z++)
                        {
                            m_Coordinates.Add(new Vector3(x, y, z));
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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    // Holds static data about the dungeon part
    public class DungeonPart : MonoBehaviour
    {
        // Size in voxels of the part
        [SerializeField]
        public Vector3 m_Size;

        private List<Vector3> m_Coordinates;

        private bool m_Dirty;

        public void Start()
        {
            m_Dirty = true;   
        }

        public void ReorientSize()
        {
            int l_halfTurns = Math.Abs((int)gameObject.transform.rotation.eulerAngles.y / 90);

            // On odd half turns, we'll need to swap the X and Z values of m_Size
            if (l_halfTurns % 2 == 1)
            {
                float swap = m_Size.x;
                m_Size.x = m_Size.z;
                m_Size.z = swap;
            }

            m_Dirty = true;
        }

        public List<Vector3> GetCoordinates(Vector3 _center)
        {
            //Debug.Log("Dungeon Generator: GetCoordinates of object center: " + _center);

            if (m_Dirty)
            {
                m_Coordinates = new List<Vector3>();
                float l_voxelCount = m_Size.x * m_Size.y * m_Size.z;
                //Debug.Log("Dungeon Generator: Suspected voxel count: " + l_voxelCount);

                Vector3 negBound = _center - (new Vector3(
                                                        m_Size.x * 0.5f,
                                                        m_Size.y * 0.5f,
                                                        m_Size.z * 0.5f
                                            ));

                //Debug.Log("Dungeon Generator: Lower bound for GetSpace: \nBound: " + negBound);

                Vector3 posBound = _center + (new Vector3(
                                                        m_Size.x * 0.5f,
                                                        m_Size.y * 0.5f,
                                                        m_Size.z * 0.5f
                                            ));

                //Debug.Log("Dungeon Generator: Upper bound for GetSpace: \nBound: " + posBound);

                for (int x = (int)negBound.x; x < (int)posBound.x; x++)
                {
                    for (int y = (int)negBound.y; y < (int)posBound.y; y++)
                    {
                        for (int z = (int)negBound.z; z < (int)posBound.z; z++)
                        {
                            m_Coordinates.Add(new Vector3(x, y, z));
                        }
                    }
                }

                m_Dirty = false;
            }

            string message = "Dungeon Generator: Coordinates from GetSpace: " + m_Coordinates.Count + "\n";

            foreach (Vector3 coord in m_Coordinates)
            {
                message += coord;
            }

            //Debug.Log(message);

            return m_Coordinates;
        }
    }
}
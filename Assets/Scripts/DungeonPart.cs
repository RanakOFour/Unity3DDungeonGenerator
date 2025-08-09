using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
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

        public void Awake()
        {
            m_Dirty = true;   
        }

        public void Start()
        {
            m_Dirty = true;
        }

        public void ReorientSize()
        {
            string l_debugMessage = "Dungeon Generator: Reorienting Piece of Size\nOld Sizing: " + m_Size;
            int l_halfTurns = Math.Abs((int)gameObject.transform.rotation.eulerAngles.y / 90);

            // On odd half turns, we'll need to swap the X and Z values of m_Size
            if (l_halfTurns % 2 == 1)
            {
                (m_Size.z, m_Size.x) = (m_Size.x, m_Size.z);
            }

            l_debugMessage += "\nNew Sizing: " + m_Size;

            Debug.Log(l_debugMessage);

            m_Dirty = true;
        }

        public List<Vector3> GetCoordinates(Vector3 _center)
        {
            string l_debugMessage = "Dungeon Generator: GetCoordinates of object center: " + _center;

            if (m_Dirty)
            {
                m_Coordinates = new List<Vector3>();
                float l_voxelCount = m_Size.x * m_Size.y * m_Size.z;

                l_debugMessage += "\nSuspected voxel count: " + l_voxelCount;

                Vector3 negBound = _center - (0.5f * m_Size);

                l_debugMessage += "\nLower bound for GetSpace: \nBound: " + negBound;

                Vector3 posBound = _center + (0.5f * m_Size);

                l_debugMessage += "\nUpper bound for GetSpace: \nBound: " + posBound + "\n\n";

                // Special case where objects near (0, 0, 0) will skip coordinates due to integer rounding -0.5 and 0.5 to 0, so another condition is needed
                // Tried clauses:
                // (int)negBound == (int)posBound && xyz == 0
                // (int)negBound == (int)posBound && xyz == negBound
                // Mathf.Round(negBound) instead of (int)negBound  THIS CAUSES MOST OF THEM TO FAIL
                // 

                for (int x = (int)negBound.x;
                         x < (int)posBound.x;
                         x++)
                {
                    for (int y = (int)negBound.y;
                             y < (int)posBound.y;
                             y++)
                    {
                        for (int z = (int)negBound.z;
                                 z < (int)posBound.z;
                                 z++)
                        {
                            m_Coordinates.Add(new Vector3(x, y, z));
                            l_debugMessage += "\n" + m_Coordinates[m_Coordinates.Count - 1];
                        }
                    }
                }

                if (m_Coordinates.Count != l_voxelCount)
                {
                    int l_diff = (int)l_voxelCount - m_Coordinates.Count;
                    l_debugMessage = "Dungeon Generator: " + gameObject.name + " Failed Coordinate Count by " + l_diff + "\n" + l_debugMessage;
                    Debug.Log(l_debugMessage);
                }
                else
                {
                    l_debugMessage = "Dungeon Generator: Successful Coordinate Count\n" + l_debugMessage;
                }

                m_Dirty = false;
            }

            //Debug.Log(l_debugMessage);

            return m_Coordinates;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    public enum Orientation
    {
        XPos = 1,
        XNeg = 2,
        YPos = 4,
        YNeg = 8,
        ZPos = 16,
        ZNeg = 32
    }

    public struct Door
    {
        // If true, the door is ignored
        [SerializeField]
        public bool m_IsWall;

        // GameObject that contains the location of the door
        [SerializeField]
        public GameObject m_DoorPrefab;

        [SerializeField]
        public Orientation m_Orientation;
    }


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
        public List<Door> m_DoorGOs;
    }
}
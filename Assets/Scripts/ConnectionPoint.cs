using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    // Assigned to different bits so I can do XOR shenanigans
    public enum Orientation
    {
        XPos = 1,
        XNeg = 2,
        YPos = 4,
        YNeg = 8,
        ZPos = 16,
        ZNeg = 32
    }

    public class ConnectionPoint : MonoBehaviour
    {
        // If true, the connection has been used
        private bool m_Connected;

        // The direction facing out from the prefab at the connection's location
        [SerializeField]
        public Orientation m_Orientation;

        private void Start()
        {
            m_Connected = false;
        }

        public void Connect()
        {
            m_Connected = true;
        }

        public bool Connected()
        {
            return m_Connected;
        }
    }
}
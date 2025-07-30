using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    public class ConnectionPoint : MonoBehaviour
    {
        // If true, the connection has been used
        [SerializeField]
        private bool m_Connected;

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
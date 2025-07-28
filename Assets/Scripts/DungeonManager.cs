using System.Collections.Generic;
using UnityEngine;

namespace UnityDungeonGenerator
{
    [System.Serializable]
    struct DungeonPart
    {
        // The prefab to be used for this dungeon part
        [SerializeField]
        public GameObject m_Prefab;

        // Max count of each corresponding part. -1 for unlimited
        [SerializeField]
        public int m_MaxIterations;
    }

    // Holds static data about the dungeon
    public class DungeonManager : MonoBehaviour
    {
        // Size of individual voxels that compose the dungeon parts
        [SerializeField]
        public Vector3 m_VoxelSize;

        // Size of the whole dungeon space in voxels
        [SerializeField]
        private Vector3 m_DungeonSize;

        [SerializeField]
        public GameObject m_StartingRoom;

        // Unique prefabs that will make up the dungeon
        [SerializeField]
        private List<DungeonPart> m_DungeonParts;

        public void Generate()
        {

        }
    }
}
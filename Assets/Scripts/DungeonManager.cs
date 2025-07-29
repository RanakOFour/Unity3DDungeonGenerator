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

        // Used for detecting overlaps, 0 for free space, 1 for taken space
        private int[,,] m_VoxelMap;

        [SerializeField]
        public GameObject m_StartingRoom;

        // Unique prefabs that will make up the dungeon
        [SerializeField]
        private List<DungeonPart> m_DungeonParts;

        public void Generate()
        {
            m_VoxelMap = new int[(int)m_DungeonSize.x, (int)m_DungeonSize.y, (int)m_DungeonSize.z];

            Queue<GameObject> l_partQueue = new Queue<GameObject>();

            if (m_StartingRoom == null)
            {
                Debug.Log("DungeonGenerator: no starting room set!");
                return;
            }

            GameObject l_instantiatedPiece = GameObject.Instantiate(m_StartingRoom);
            l_partQueue.Enqueue(l_instantiatedPiece);
            FillMap(m_StartingRoom);

            while (l_partQueue.Count > 0)
            {
                GameObject l_currentPiece = l_partQueue.Dequeue();
                ConnectionPoint[] l_connectionPoints = l_currentPiece.GetComponentsInChildren<ConnectionPoint>();

                Debug.Log("Dungeon Generator: Points on piece detected: " + l_connectionPoints.Length);
                return;
            }
        }

        private void FillMap(GameObject _dungeonPart)
        {
            PartPrefab l_prefab = _dungeonPart.GetComponent<PartPrefab>();

            // Voxel position of prefabs center
            Vector3 l_center = new Vector3(
                                           _dungeonPart.transform.position.x / m_DungeonSize.x,
                                           _dungeonPart.transform.position.y / m_DungeonSize.y,
                                           _dungeonPart.transform.position.z / m_DungeonSize.z
                                           );

            List<Vector3> l_prefabSpace = l_prefab.GetSpace(l_center);
            foreach (Vector3 coord in l_prefabSpace)
            {
                m_VoxelMap[(int)coord.x, (int)coord.y, (int)coord.z] = 1;
                Debug.Log("Dungeon Generator: Position filled: " + coord.x + ", " + coord.y + ", " + coord.z);
            }
        }
    }
}
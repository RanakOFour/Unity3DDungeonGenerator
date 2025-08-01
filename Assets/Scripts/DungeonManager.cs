using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace UnityDungeonGenerator
{
    [System.Serializable]
    struct DungeonPartContainer
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
        private List<DungeonPartContainer> m_DungeonParts;

        private bool m_Generated;

        public void Start()
        {
            m_Generated = false;
        }

        public void Generate()
        {
            if (m_Generated) {
                Debug.Log("DungeonGenerator: dungeon already generated!");
                return; }

            if(m_StartingRoom == null) {
                Debug.Log("DungeonGenerator: no starting room set!");
                return; }
            
            m_Generated = true;

            Debug.Log("Dungeon Generator: Starting generation");

            // I tried adding an 'm_Iterations' property to DungeonParts but it wouldn't work for some reason when changing the value
            List<int> l_IterationCounts = new List<int>();
            foreach (DungeonPartContainer p in m_DungeonParts)
            {
                l_IterationCounts.Add(0);
            }

            bool[,,] l_VoxelMap = new bool[(int)m_DungeonSize.x, (int)m_DungeonSize.y, (int)m_DungeonSize.z];
            Queue<GameObject> l_partQueue = new Queue<GameObject>();

            Debug.Log("Dungeon Generator: Creating starter room");

            GameObject l_instantiatedPiece = GameObject.Instantiate(m_StartingRoom, gameObject.transform);
            l_partQueue.Enqueue(l_instantiatedPiece);
            FillMap(ref l_instantiatedPiece, ref l_VoxelMap);
            PrintVoxelMap(ref l_VoxelMap);

            while (l_partQueue.Count > 0 && m_DungeonParts.Count > 0)
            {
                //Debug.Log("Dungeon Generator: Getting piece to connect");
                GameObject l_currentPiece = l_partQueue.Dequeue();
                ConnectionPoint[] l_connectionPoints = l_currentPiece.GetComponentsInChildren<ConnectionPoint>();

                //Debug.Log("Dungeon Generator: Points on piece detected: " + l_connectionPoints.Length);

                foreach (ConnectionPoint l_currentPoint in l_connectionPoints)
                {
                    if (l_currentPoint.Connected()) { continue; }
                    if (m_DungeonParts.Count == 0) { break; }

                    //Debug.Log("Dungeon Generator: Connecting point at " + l_currentPoint.gameObject.transform.position);

                    bool l_partFits = false;
                    int l_newPieceIndex = 0;

                    // Prevent deadlocking
                    int l_unsuccessfulFits = 0;

                    while (!l_partFits && l_unsuccessfulFits < 5)
                    {
                        // Cycle through parts randomly until one that can be placed is found
                        //Debug.Log("Dungeon Generator: Finding new piece to connect");

                        l_newPieceIndex = UnityEngine.Random.Range(0, m_DungeonParts.Count);

                        Debug.Log("Dungeon Generator: Piece index to connect: " + l_newPieceIndex);
                        GameObject l_newPiece = GameObject.Instantiate(m_DungeonParts[l_newPieceIndex].m_Prefab, gameObject.transform);
                        ConnectionPoint l_newPoint = l_newPiece.GetComponentInChildren<ConnectionPoint>();

                        // Using Quaternion.AngleAxis messes with l_newPiece transform, and that isn't good
                        l_newPiece.transform.Rotate(Vector3.up, (l_currentPoint.transform.eulerAngles.y - l_newPoint.transform.eulerAngles.y) + 180f);
                        l_newPiece.GetComponent<DungeonPart>().ReorientSize();

                        // Accounts for difference in overlapping pieces
                        Vector3 l_translate = l_currentPoint.transform.position - l_newPoint.transform.position;
                        l_newPiece.transform.position += l_translate;

                        // l_translate = l_currentPiece.transform.position - l_newPiece.transform.position;
                        // l_newPiece.transform.position += l_translate;

                        if (DoesObjectFit(ref l_newPiece, ref l_VoxelMap))
                        {
                            l_partFits = true;
                            FillMap(ref l_newPiece, ref l_VoxelMap);
                            l_newPoint.Connect();
                            l_currentPoint.Connect();
                            l_partQueue.Enqueue(l_newPiece);
                        }
                        else
                        {
                            GameObject.Destroy(l_newPiece);
                            l_unsuccessfulFits += 1;
                        }
                    }

                    PrintVoxelMap(ref l_VoxelMap);

                    //Increment number of instances and remove from list if the maximum number if instances is reached
                    l_IterationCounts[l_newPieceIndex]++;
                    if (l_IterationCounts[l_newPieceIndex] == m_DungeonParts[l_newPieceIndex].m_MaxIterations)
                    {
                        m_DungeonParts.Remove(m_DungeonParts[l_newPieceIndex]);
                        l_IterationCounts.Remove(l_IterationCounts[l_newPieceIndex]);
                    }
                }
            }

            PrintVoxelMap(ref l_VoxelMap);
        }

        private void FillMap(ref GameObject _dungeonPart, ref bool[,,] _VoxelMap)
        {
            DungeonPart l_prefab = _dungeonPart.GetComponent<DungeonPart>();

            // Voxel position of prefabs center
            Vector3 l_shapeCenter = new Vector3(
                                           (_dungeonPart.transform.position.x + (0.5f * m_VoxelSize.x)) / m_VoxelSize.x,
                                           (_dungeonPart.transform.position.y + (0.5f * m_VoxelSize.y)) / m_VoxelSize.y,
                                           (_dungeonPart.transform.position.z + (0.5f * m_VoxelSize.z)) / m_VoxelSize.z
                                           );
            List<Vector3> l_prefabCoords = l_prefab.GetCoordinates(l_shapeCenter);

            Vector3 l_currentCoord;
            for (int i = 0; i < l_prefabCoords.Count; i++)
            {
                l_currentCoord = l_prefabCoords[i] + (m_DungeonSize * 0.5f);
                _VoxelMap[(int)l_currentCoord.x,
                           (int)l_currentCoord.y,
                           (int)l_currentCoord.z] = true;
                Debug.Log("Dungeon Generator: Position filled: " + l_currentCoord.x + ", " + l_currentCoord.y + ", " + l_currentCoord.z);
            }
        }

        private bool DoesObjectFit(ref GameObject _dungeonPart, ref bool[,,] _VoxelMap)
        {
            DungeonPart l_prefab = _dungeonPart.GetComponent<DungeonPart>();
            Vector3 l_shapeCenter = new Vector3(
                                           (_dungeonPart.transform.position.x + (0.5f * m_VoxelSize.x)) / m_VoxelSize.x,
                                           (_dungeonPart.transform.position.y + (0.5f * m_VoxelSize.y)) / m_VoxelSize.y,
                                           (_dungeonPart.transform.position.z + (0.5f * m_VoxelSize.z)) / m_VoxelSize.z
                                           );

            List<Vector3> l_prefabCoords = l_prefab.GetCoordinates(l_shapeCenter);
            Vector3 l_currentCoord;
            foreach (Vector3 coordinate in l_prefabCoords)
            {
                l_currentCoord = new Vector3(
                    (int)(coordinate.x + (m_DungeonSize.x * 0.5f)),
                    (int)(coordinate.y + (m_DungeonSize.y * 0.5f)),
                    (int)(coordinate.z + (m_DungeonSize.z * 0.5f))

                );

                bool l_xCheck = (int)l_currentCoord.x < 0 || (int)l_currentCoord.x >= m_DungeonSize.x;
                bool l_yCheck = (int)l_currentCoord.y < 0 || (int)l_currentCoord.y >= m_DungeonSize.y;
                bool l_zCheck = (int)l_currentCoord.z < 0 || (int)l_currentCoord.z >= m_DungeonSize.z;

                if (l_xCheck ||
                    l_yCheck ||
                    l_zCheck)
                {
                    //Debug.Log("Dungeon Generator: Object does not fit. Error coord: " + l_currentCoord);
                    return false;
                }

                //Debug.Log("Voxel map check at: " + ((int)l_currentCoord.x) + ", " + ((int)l_currentCoord.y) + ", " + ((int)l_currentCoord.z));
                if (_VoxelMap[(int)l_currentCoord.x,
                           (int)l_currentCoord.y,
                           (int)l_currentCoord.z])
                {
                    //Debug.Log("Dungeon Generator: Object overlaps another. Error coord: " + l_currentCoord);
                    return false;
                }
            }

            //Debug.Log("Dungeon Generator: Object fits");
            return true;
        }

        private void PrintVoxelMap(ref bool[,,] _VoxelMap)
        {
            string message = "Dungeon Generator: Voxel Map Output:\n";

            for (int y = 0; y < m_DungeonSize.y; y++)
            {
                message += "For y = " + y + ":\n";
                for (int x = 0; x < m_DungeonSize.x; x++)
                {
                    for (int z = 0; z < m_DungeonSize.z; z++)
                    {
                        if (_VoxelMap[x, y, z])
                        {
                            message += "1  ";
                        }
                        else
                        {
                            message += "0 ";
                        }
                    }
                    message += "\n";
                }
            }

            Debug.Log(message);
        }
    }
}
using System;
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

        private int m_Iterations;

        public void Increment()
        {
            m_Iterations += 1;
        }

        public int Iterations()
        {
            return m_Iterations;
        }
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
            Debug.Log("Dungeon Generator: Starting generation");

            m_VoxelMap = new int[(int)m_DungeonSize.x, (int)m_DungeonSize.y, (int)m_DungeonSize.z];
            Queue<GameObject> l_partQueue = new Queue<GameObject>();

            if (m_StartingRoom == null)
            {
                Debug.Log("DungeonGenerator: no starting room set!");
                return;
            }


            Debug.Log("Dungeon Generator: Creating starter room");

            GameObject l_instantiatedPiece = GameObject.Instantiate(m_StartingRoom, gameObject.transform);
            l_partQueue.Enqueue(l_instantiatedPiece);
            FillMap(ref m_StartingRoom);

            Debug.Log("Dungeon Generator: Getting piece to connect");
            GameObject l_currentPiece = l_partQueue.Dequeue();
            ConnectionPoint[] l_connectionPoints = l_currentPiece.GetComponentsInChildren<ConnectionPoint>();

            Debug.Log("Dungeon Generator: Points on piece detected: " + l_connectionPoints.Length);

            ///////
            foreach (ConnectionPoint l_currentPoint in l_connectionPoints)
            {
                if (l_currentPoint.Connected()) { continue; }

                bool l_partFits = false;
                int l_newPieceIndex = 0;

                // Cycle through parts randomly until one that can be placed is found
                Debug.Log("Dungeon Generator: Finding new piece to connect");

                    l_newPieceIndex = UnityEngine.Random.Range(0, m_DungeonParts.Count - 1);

                    Debug.Log("Dungeon Generator: Piece index to connect: " + l_newPieceIndex);
                    GameObject l_newPiece = GameObject.Instantiate(m_DungeonParts[l_newPieceIndex].m_Prefab, gameObject.transform);
                    ConnectionPoint l_newPoint = l_newPiece.GetComponentInChildren<ConnectionPoint>();

                    l_newPiece.transform.Rotate(Vector3.up, (l_currentPiece.transform.eulerAngles.y - l_currentPoint.transform.eulerAngles.y) + 180f);
                    Vector3 l_translate = l_currentPoint.transform.position - l_newPoint.transform.position;
                    l_newPiece.transform.position += l_translate;
                    // newRoom.transform.rotation = Quaternion.AngleAxis((lastRoomDoor.transform.eulerAngles.y - newRoomDoor.transform.eulerAngles.y) + 180f, Vector3.up);
                    //     Vector3 translate = lastRoomDoor.transform.position - newRoomDoor.transform.position;
                    //     newRoom.transform.position += translate;

                    if (DoesObjectFit(ref l_newPiece))
                    {
                        l_partFits = true;
                        FillMap(ref l_newPiece);
                        l_newPoint.Connect();
                        l_currentPoint.Connect();
                        l_partQueue.Enqueue(l_newPiece);
                    }
                    else
                    {
                        GameObject.Destroy(l_newPiece);
                    }
            }

                ///////

                // while (l_partQueue.Count > 0 && m_DungeonParts.Count > 0)
                // {
                //     Debug.Log("Dungeon Generator: Getting piece to connect");
                //     GameObject l_currentPiece = l_partQueue.Dequeue();
                //     ConnectionPoint[] l_connectionPoints = l_currentPiece.GetComponentsInChildren<ConnectionPoint>();

                //     Debug.Log("Dungeon Generator: Points on piece detected: " + l_connectionPoints.Length);


                //     foreach (ConnectionPoint l_currentPoint in l_connectionPoints)
                //     {
                //         if (l_currentPoint.Connected()) { continue; }

                //         bool l_partFits = false;
                //         int l_newPieceIndex = 0;

                //         // Cycle through parts randomly until one that can be placed is found
                //         while (!l_partFits)
                //         {
                //             Debug.Log("Dungeon Generator: Finding new piece to connect");

                //             l_newPieceIndex = UnityEngine.Random.Range(0, m_DungeonParts.Count - 1);

                //             Debug.Log("Dungeon Generator: Piece index to connect: " + l_newPieceIndex);
                //             GameObject l_newPiece = GameObject.Instantiate(m_DungeonParts[l_newPieceIndex].m_Prefab, gameObject.transform);
                //             ConnectionPoint l_newPoint = l_newPiece.GetComponentInChildren<ConnectionPoint>();

                //             l_newPiece.transform.rotation = Quaternion.AngleAxis((l_currentPiece.transform.eulerAngles.y - l_currentPoint.transform.eulerAngles.y) + 180f, Vector3.up);
                //             Vector3 l_translate = l_currentPoint.transform.position - l_newPoint.transform.position;
                //             l_newPiece.transform.position += l_translate;
                //             // newRoom.transform.rotation = Quaternion.AngleAxis((lastRoomDoor.transform.eulerAngles.y - newRoomDoor.transform.eulerAngles.y) + 180f, Vector3.up);
                //             //     Vector3 translate = lastRoomDoor.transform.position - newRoomDoor.transform.position;
                //             //     newRoom.transform.position += translate;

                //             if (DoesObjectFit(ref l_newPiece))
                //             {
                //                 l_partFits = true;
                //                 FillMap(ref l_newPiece);
                //                 l_newPoint.Connect();
                //                 l_currentPoint.Connect();
                //                 l_partQueue.Enqueue(l_newPiece);
                //             }
                //             else
                //             {
                //                 GameObject.Destroy(l_newPiece);
                //             }
                //         }

                //         // Increment number of instances and remove from list if the maximum number if instances is reached
                //         m_DungeonParts[l_newPieceIndex].Increment();
                //         if (m_DungeonParts[l_newPieceIndex].Iterations() >= m_DungeonParts[l_newPieceIndex].m_MaxIterations)
                //         {
                //             m_DungeonParts.Remove(m_DungeonParts[l_newPieceIndex]);
                //         }
                //     }
                // }
            }

        private void FillMap(ref GameObject _dungeonPart)
        {
            PartPrefab l_prefab = _dungeonPart.GetComponent<PartPrefab>();

            // Voxel position of prefabs center
            Vector3 l_shapeCenter = new Vector3(
                                           _dungeonPart.transform.position.x / m_VoxelSize.x,
                                           _dungeonPart.transform.position.y / m_VoxelSize.y,
                                           _dungeonPart.transform.position.z / m_VoxelSize.z
                                           );

            List<Vector3> l_prefabCoords = l_prefab.GetCoordinates(l_shapeCenter);
            Vector3 l_currentCoord;
            for (int i = 0; i < l_prefabCoords.Count; i++)
            {
                l_currentCoord = l_prefabCoords[i] + (m_DungeonSize * 0.5f);
                m_VoxelMap[(int)l_currentCoord.x,
                           (int)l_currentCoord.y,
                           (int)l_currentCoord.z] = 1;
                Debug.Log("Dungeon Generator: Position filled: " + l_currentCoord.x + ", " + l_currentCoord.y + ", " + l_currentCoord.z);
            }
        }

        private bool DoesObjectFit(ref GameObject _dungeonPart)
        {
            PartPrefab l_prefab = _dungeonPart.GetComponent<PartPrefab>();
            Vector3 l_shapeCenter = new Vector3(
                                           _dungeonPart.transform.position.x / m_VoxelSize.x,
                                           _dungeonPart.transform.position.y / m_VoxelSize.y,
                                           _dungeonPart.transform.position.z / m_VoxelSize.z
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

                if (Math.Abs((int)l_currentCoord.x) > m_DungeonSize.x ||
                    Math.Abs((int)l_currentCoord.y) > m_DungeonSize.y ||
                    Math.Abs((int)l_currentCoord.z) > m_DungeonSize.z)
                {
                    Debug.Log("Dungeon Generator: Object does not fit. Error coord: " + l_currentCoord);
                    return false;
                }

                if (m_VoxelMap[(int)l_currentCoord.x,
                           (int)l_currentCoord.y,
                           (int)l_currentCoord.z] == 1)
                {
                    Debug.Log("Dungeon Generator: Object overlaps another. Error coord: " + l_currentCoord);
                    return false;
                }
            }

            Debug.Log("Dungeon Generator: Object fits");
            return true;
        }
    }
}
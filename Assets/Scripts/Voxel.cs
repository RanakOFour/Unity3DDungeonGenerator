using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


// Editor only voxel cube (for sizing)
[ExecuteAlways]
public class Voxel : MonoBehaviour
{
    private Vector3 m_Size;
    private UnityDungeonGenerator.DungeonManager m_dm;

    private void Awake()
    {
        Debug.Log("Voxel added");    
    }

    // Update is called once per frame
    private void Update()
    {
        GameObject obj = GameObject.Find("Dungeon");
        if (obj != null)
        {
            Debug.Log("DungeonManager found");
            if (obj.TryGetComponent<UnityDungeonGenerator.DungeonManager>(out m_dm))
            {
                if (m_dm.m_VoxelSize == Vector3.zero)
                {
                    Debug.Log("VoxelSize is zero");
                }
                else
                {
                    Debug.Log("Size set");
                    m_Size = m_dm.m_VoxelSize;
                }
            }
            else
            {
                Debug.Log("No DungeonManager found on dungeon");
                m_Size = Vector3.one;
            }
        }
        else
        {
            Debug.Log("No dungeon for reference found");
            m_Size = Vector3.one;
        }


        gameObject.transform.localScale = m_Size;
    }
}

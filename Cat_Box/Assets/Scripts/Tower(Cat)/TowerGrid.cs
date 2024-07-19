using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGrid : MonoBehaviour
{
    public TowerController myTower;
    public List<GameObject> boxObjects = new List<GameObject>();

    private void Start()
    {
        ChangeBoxObject(1);
    }

    public bool ChangeBoxObject(int level)
    {
        if (level < 1) level = 1;

        if (boxObjects[level - 1].activeInHierarchy)
        {
            return true;
        }

        for (int i = 0; i < boxObjects.Count; i ++)
        {
            if(boxObjects[i].activeInHierarchy) boxObjects[i].SetActive(false);
        }
        
        boxObjects[level - 1].SetActive(true);

        return boxObjects[level - 1].activeInHierarchy;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position, new Vector3(1, 0.5f, 1));
    }
}

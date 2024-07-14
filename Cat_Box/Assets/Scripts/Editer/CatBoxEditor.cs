using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CatBoxEditor
{
    public static EnemyManager manager;
    public static GameObject wayPoints;

    [MenuItem("CatBox/GameObject/EnemyManager", priority = 0)]
    static void CreateMonsterManager()
    {
        var enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();

        if (enemyManager == null) enemyManager = new GameObject("EnemyManager").AddComponent<EnemyManager>();

        manager = enemyManager;
    }

    [MenuItem("CatBox/GameObject/PoolManager", priority = 0)]
    static void CreatePoolManager()
    {
        var poolManager = GameObject.FindAnyObjectByType<PoolManager>();

        if (poolManager == null) poolManager = new GameObject("PoolManager").AddComponent<PoolManager>();
    }

    [MenuItem("CatBox/GameObject/WayPoint", priority = 1)]
    static void CreateWayPoint()
    {
        if (wayPoints == null) wayPoints = new GameObject("WayPoints");

        var temp = new GameObject("WayPoint").AddComponent<EnemyWayPoint>();
        temp.transform.parent = wayPoints.transform;
        temp.transform.position = Vector3.zero;
        temp.transform.rotation = Quaternion.identity;

        if (manager == null) CreateMonsterManager();

        if (manager.enemyWayPoints.Count == 0)
        {
            temp.isStartPoint = true;
            temp.name = "SpawnPoint";
        }
        else
        {
            temp.name = $"WayPoint{manager.enemyWayPoints.Count}";
        }

        manager.enemyWayPoints.Add(temp);
    }
}
[CustomEditor(typeof(EnemyManager))]
public class EnemyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemyManager enemyManager = (EnemyManager)target;

        if (GUILayout.Button("ResetEnemyWayPoints"))
        {
            enemyManager.ResetEnemyWayPoints();
        }
    }
}

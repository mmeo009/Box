using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public EnemyObject enemyObject;
    public float BeforeSpawnTime;
}
public class EnemyManager : MonoBehaviour
{
    [Header("�̹� ���������� ���� ���͵�")] public List<EnemyData> enemyDatas = new List<EnemyData>();                  // �̹� ���������� ���� ���͵�
    public HashSet<EnemyController> activeEnemies = new HashSet<EnemyController>();                                     // ���� �����Ǿ��ִ� ���͵�
    public int nowMonsterIndex = 0;                                                                                     // ���� ���� ����
    public Timer beforeSpawnTimeTimer = new Timer(0.0f);                                                                // ���� ���� ������ ��Ÿ��

    public List<EnemyWayPoint> enemyWayPoints = new List<EnemyWayPoint>();                                              // �� ��������Ʈ (������ �� ���)
    public Transform spawnPoint;                                                                                        // ���� ����Ʈ
    public List<Transform> wayPoints;                                                                                   // �� ��������Ʈ���� ������ ����Ʈ

    public static EnemyManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        SetWayPoints();

        if(enemyDatas.Count > 0)
        {
            beforeSpawnTimeTimer = new Timer(enemyDatas[nowMonsterIndex].BeforeSpawnTime);
            beforeSpawnTimeTimer.Start();
        }
    }
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        beforeSpawnTimeTimer.Update(deltaTime, GameManager.instance.gameSpeed);

        if(!beforeSpawnTimeTimer.IsRunning() && enemyDatas.Count >= nowMonsterIndex + 2)
        {
            SpawnEnemy(enemyDatas[nowMonsterIndex].enemyObject);
            nowMonsterIndex++;
            beforeSpawnTimeTimer = new Timer(enemyDatas[nowMonsterIndex].BeforeSpawnTime);
            beforeSpawnTimeTimer.Start();
        }
    }
    public bool SetWayPoints()
    {
        if(enemyWayPoints == null) return false;

        List<EnemyWayPoint> _wayPoints = enemyWayPoints;
        _wayPoints.OrderBy(way => way.index);

        EnemyWayPoint start = _wayPoints[0];
        EnemyWayPoint end = _wayPoints[0];
        wayPoints = new List<Transform>();

        foreach (var way in _wayPoints)
        {
            if(!start.isStartPoint && way.index < start.index)
            {
                start = way;
            }
            else if(!end.isEndPoint && way.index > end.index)
            {
                end = way;
            }
            wayPoints.Add(way.transform);
        }

        spawnPoint = start.transform;
        wayPoints.Remove(start.transform);
        return true;
    }

    public void SpawnEnemy(EnemyObject enemyObject)                         // �� ������Ʈ�� �����ϴ� �Լ�
    {
        var poolManager = PoolManager.Instance;                             // Ǯ �޴����� ������

        if(!poolManager.HasThisPool(enemyObject.name))                      // �����Ϸ��� ���� Ǯ�� �ִ��� Ȯ�� �� ���� ���
        {
            poolManager.AddNewPool(enemyObject.name, enemyObject.gameObject);       // Ǯ�� ���� �������
        }

        var enemyGameObject = poolManager.SpawnFromPool(enemyObject.name, spawnPoint.position, spawnPoint.rotation);        // �� ������Ʈ�� Ǯ���� ������

        EnemyController enemy;            // �� ��ũ��Ʈ

        if(!enemyGameObject.TryGetComponent<EnemyController>(out enemy))              // ���� ��ũ��Ʈ ������Ʈ�� ���� ������Ʈ���
        {
            enemy = enemyGameObject.AddComponent<EnemyController>();                  // �Ҵ�����
        }

        enemy.enemy = enemyObject;                  // ������ �ش��ϴ� ScriptableObject�� �־���
        enemy.ResetEnemy();                         // ���� �����͸� �ʱ�ȭ ��Ŵ
        enemy.wayPoints = wayPoints;                // ������ ��������Ʈ�� �־���
        activeEnemies.Add(enemy);                   // ���� Ȱ��ȭ�� ���� ����Ʈ�� �߰���
    }

    public void ResetEnemyWayPoints()               // ��� ��������Ʈ���� �����ϴ� �Լ�
    {
        DestroyImmediate(enemyWayPoints[0].gameObject.transform.parent);

        enemyWayPoints.Clear();
    }
    public void WayPointsRePosition()
    {
        foreach(EnemyWayPoint wayPoint in enemyWayPoints)
        {
            wayPoint.transform.position = new Vector3(Mathf.Round(wayPoint.transform.position.x) + 0.5f, 0.5f, Mathf.Round(wayPoint.transform.position.z) + 0.5f);
        }
    }

    public bool IsEnemyActive(EnemyController enemyController)          // �� ���� Ȱ��ȭ �Ǿ��ִ��� Ȯ���ϴ� �Լ�
    {
        return activeEnemies.Contains(enemyController);
    }
}

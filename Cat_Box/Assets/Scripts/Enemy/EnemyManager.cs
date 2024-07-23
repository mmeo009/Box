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
    [Header("이번 스테이지에 나올 몬스터들")] public List<EnemyData> enemyDatas = new List<EnemyData>();                  // 이번 스테이지에 나올 몬스터들
    public HashSet<EnemyController> activeEnemies = new HashSet<EnemyController>();                                     // 지금 생성되어있는 몬스터들
    public int nowMonsterIndex = 0;                                                                                     // 지금 몬스터 순서
    public Timer beforeSpawnTimeTimer = new Timer(0.0f);                                                                // 몬스터 스폰 사이의 쿨타임

    public List<EnemyWayPoint> enemyWayPoints = new List<EnemyWayPoint>();                                              // 적 웨이포인트 (수정할 때 사용)
    public Transform spawnPoint;                                                                                        // 스폰 포인트
    public List<Transform> wayPoints;                                                                                   // 적 웨이포인트들을 저장한 리스트

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

    public void SpawnEnemy(EnemyObject enemyObject)                         // 적 오브젝트를 생성하는 함수
    {
        var poolManager = PoolManager.Instance;                             // 풀 메니저를 가져옴

        if(!poolManager.HasThisPool(enemyObject.name))                      // 생성하려는 적의 풀이 있는지 확인 후 없을 경우
        {
            poolManager.AddNewPool(enemyObject.name, enemyObject.gameObject);       // 풀을 새로 만들어줌
        }

        var enemyGameObject = poolManager.SpawnFromPool(enemyObject.name, spawnPoint.position, spawnPoint.rotation);        // 적 오브젝트를 풀에서 가져옴

        EnemyController enemy;            // 적 스크립트

        if(!enemyGameObject.TryGetComponent<EnemyController>(out enemy))              // 만약 스크립트 컴포넌트가 없는 오브젝트라면
        {
            enemy = enemyGameObject.AddComponent<EnemyController>();                  // 할당해줌
        }

        enemy.enemy = enemyObject;                  // 적에게 해당하는 ScriptableObject를 넣어줌
        enemy.ResetEnemy();                         // 적의 데이터를 초기화 시킴
        enemy.wayPoints = wayPoints;                // 적에게 웨이포인트를 넣어줌
        activeEnemies.Add(enemy);                   // 지금 활성화된 몬스터 리스트에 추가함
    }

    public void ResetEnemyWayPoints()               // 모든 웨이포인트들을 제거하는 함수
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

    public bool IsEnemyActive(EnemyController enemyController)          // 이 적이 활성화 되어있는지 확인하는 함수
    {
        return activeEnemies.Contains(enemyController);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform spawnPoint;
    public List<Transform> wayPoints = new List<Transform>();

    public void SpawnEnemy(EnemyObject enemyObject)                         // 적 오브젝트를 생성하는 함수
    {
        var poolManager = PoolManager.Instance;                             // 풀 메니저를 가져옴

        if(!poolManager.HasThisPool(enemyObject.name))                      // 생성하려는 적의 풀이 있는지 확인 후 없을 경우
        {
            poolManager.AddNewPool(enemyObject.name, enemyObject.gameObject);       // 풀을 새로 만들어줌
        }

        var enemyGameObject = poolManager.SpawnFromPool(enemyObject.name, spawnPoint.position, spawnPoint.rotation);        // 적 오브젝트를 풀에서 가져옴

        Enemy enemy;            // 적 스크립트

        if(!enemyGameObject.TryGetComponent<Enemy>(out enemy))              // 만약 스크립트 컴포넌트가 없는 오브젝트라면
        {
            enemy = enemyGameObject.AddComponent<Enemy>();                  // 할당해줌
        }

        enemy.enemy = enemyObject;                  // 적에게 해당하는 ScriptableObject를 넣어줌
        enemy.ResetEnemy();                         // 적의 데이터를 초기화 시킴
        enemy.wayPoints = wayPoints;                // 적에게 웨이포인트를 넣어줌
    }
}

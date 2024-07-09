using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform spawnPoint;
    public List<Transform> wayPoints = new List<Transform>();

    public void SpawnEnemy(EnemyObject enemyObject)                         // �� ������Ʈ�� �����ϴ� �Լ�
    {
        var poolManager = PoolManager.Instance;                             // Ǯ �޴����� ������

        if(!poolManager.HasThisPool(enemyObject.name))                      // �����Ϸ��� ���� Ǯ�� �ִ��� Ȯ�� �� ���� ���
        {
            poolManager.AddNewPool(enemyObject.name, enemyObject.gameObject);       // Ǯ�� ���� �������
        }

        var enemyGameObject = poolManager.SpawnFromPool(enemyObject.name, spawnPoint.position, spawnPoint.rotation);        // �� ������Ʈ�� Ǯ���� ������

        Enemy enemy;            // �� ��ũ��Ʈ

        if(!enemyGameObject.TryGetComponent<Enemy>(out enemy))              // ���� ��ũ��Ʈ ������Ʈ�� ���� ������Ʈ���
        {
            enemy = enemyGameObject.AddComponent<Enemy>();                  // �Ҵ�����
        }

        enemy.enemy = enemyObject;                  // ������ �ش��ϴ� ScriptableObject�� �־���
        enemy.ResetEnemy();                         // ���� �����͸� �ʱ�ȭ ��Ŵ
        enemy.wayPoints = wayPoints;                // ������ ��������Ʈ�� �־���
    }
}

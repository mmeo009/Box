using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class TowerController : MonoBehaviour
{
    public TowerObject tower;           // Ÿ���� �⺻���� �Ӽ��� ������

    public int towerLevel = 1;          // Ÿ���� ������ ������

    public Timer attackRateTimer;       // Ÿ���� ���� �ӵ��� ����� Ÿ�̸�

    public List<Transform> enemys;      // ������ ������ ����Ʈ (�����Ÿ� �̳��� ����)

    private void Start()
    {
        attackRateTimer = new Timer(tower.tower[towerLevel - 1].baseAttackRate);          // Ÿ��������Ʈ�� ���� �ӵ��� ������
        attackRateTimer.Start();                                                          // Ÿ�̸Ӹ� �۵���Ŵ
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // ������ �Ͻ����� ���°� �ƴҰ��
        {
            if(attackRateTimer != null && attackRateTimer.IsRunning())      // Ÿ�̸Ӱ� �����ϰ� Ÿ�̸Ӱ� �۵����� ��
            {
                float deltaTime = Time.deltaTime;                   // �ð��� ������

                attackRateTimer.Update(deltaTime, GameManager.instance.gameSpeed);      // Ÿ�̸ӿ� �ð��� ���� ���Ӹ޴����� �ð����¸� ������

                if (!attackRateTimer.IsRunning())                   // Ÿ�̸Ӱ� ���������� ���� ��
                {
                    Attack();                                       // ������ ��
                }
            }

        }
    }

    private void Attack()
    {
        var colliders = Physics.OverlapSphere(transform.position, tower.tower[towerLevel - 1].baseRange);         // �����Ÿ� �̳��� ��� ������Ʈ�� ������

        enemys.Clear();                         // �� ����Ʈ �ʱ�ȭ

        foreach(var enemy in colliders)
        {
            if(enemy.tag == "Enemy")            // �ױװ� ���� ���
            {
                enemys.Add(enemy.transform);    // �� ����Ʈ�� �߰�
            }
        }

        if(enemys.Count > 0)                                // ����Ʈ�� ���� �Ѱ� �̻��� ���
        {
            if (tower.towerType == Enums.TowerType.BASIC)   // �Ѿ��� �߻��ϴ� �⺻���� Ÿ���� ���
            {
                FireProjectile(Enums.TowerType.BASIC, GetProximateEnemy());
            }
            else if(tower.towerType == Enums.TowerType.SLOWDOWN)
            {
                foreach(var enemy in enemys)
                {
                    enemy.GetComponent<Enemy>().Stun();
                    enemy.GetComponent<Enemy>().GetDMG(tower.tower[towerLevel].baseDamage);
                }
            }
        }
        else
        {
            return;
        }

        attackRateTimer.Start();                        // Ÿ�̸Ӹ� �ʱ�ȭ ��
    }
    private Transform GetProximateEnemy()
    {
        if (enemys.Count == 0) return null;

        Transform proximateEnemy = enemys[0].transform;
        float minDistance = tower.tower[towerLevel - 1].baseRange + 1f;

        if(enemys.Count == 1)
        {
            return proximateEnemy;
        }
        
        foreach (var enemy in enemys)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // ���� Ÿ���� �Ÿ��� ���

            if (distance < minDistance)             // ������ �Ÿ��� ���� ����� �Ÿ��� ���Ͽ�����
            {
                minDistance = distance;             // �� ���ٸ� ���� ����� �Ÿ��� ���� ������ �Ÿ��� ����
                proximateEnemy = enemy.transform;      // ���� ���� ���� ����� ������ ����
            }
        }
        return proximateEnemy;
    }

    private void FireProjectile(Enums.TowerType tower, Transform target)
    {

    }

    private void OnDrawGizmosSelected()             // Ÿ�� ������Ʈ�� �������� �� ��Ÿ���� �����
    {
        Gizmos.color = Color.blue;                  // ����� �׸� ���� �Ķ�
        Gizmos.DrawWireSphere(transform.position, tower.tower[towerLevel].baseAttackRate);          // �����Ÿ��� ǥ����

        if (enemys.Count > 0)
        {
            if(tower.towerType == Enums.TowerType.BASIC)            // Ÿ���� ���� ����� �⺻�� ���
            {
                Gizmos.color = Color.red;           // ����������
                Transform proximateEnemy = GetProximateEnemy();
                if(proximateEnemy != null)
                Gizmos.DrawLine(transform.position, proximateEnemy.position);        // Ÿ���κ��� ������ ���� �׸�
            }
            else if(tower.towerType == Enums.TowerType.SLOWDOWN)   // Ÿ���� ���� ����� ���ο�(����)�� ���
            {

                for(int i = 0; i < enemys.Count; i ++)
                {
                    if (enemys[i].GetComponent<Enemy>().isStun)
                    {
                        Gizmos.color = Color.yellow;       // �����ϰ�� ���������
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;         // �ƴҰ�� �ϴû�����
                    }
                    Gizmos.DrawSphere(enemys[i].transform.position, 1f);        // ���� ���� ���� ���鿡�� ���� �׸�
                }
            }
        }

    }
}

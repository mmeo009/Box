using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class Tower : MonoBehaviour
{
    public TowerObject tower;           // Ÿ���� �⺻���� �Ӽ��� ������

    public Timer attackRateTimer;       // Ÿ���� ���� �ӵ��� ����� Ÿ�̸�

    public List<Transform> enemys;      // ������ ������ ����Ʈ (�����Ÿ� �̳��� ����)

    private void Start()
    {
        attackRateTimer = new Timer(tower.baseAttackRate);          // Ÿ��������Ʈ�� ���� �ӵ��� ������
        attackRateTimer.Start();                                    // Ÿ�̸Ӹ� �۵���Ŵ
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
                    attackRateTimer.Start();                        // Ÿ�̸Ӹ� �ʱ�ȭ ��
                }
            }

        }
    }

    private void Attack()
    {
        var colliders = Physics.OverlapSphere(transform.position, tower.baseRange);         // �����Ÿ� �̳��� ��� ������Ʈ�� ������

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
                float minDistance = tower.baseRange + 1f;   // ���� ����� �Ÿ��� �����ϴ� ���� ã�Ƴ��� ���� �ʿ��� �ִܰŸ�
                Transform enemyNearby;                      // ���� ����� �Ÿ��� �����ϴ� ��(������ ��)

                if(enemys.Count == 1)
                {
                    enemyNearby = enemys[0].transform;      // �� ����Ʈ�� ���� �Ѱ� ���̶�� �� ���� ���� ����� ���� �߰�
                }
                else
                {
                    foreach (var enemy in enemys)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);        // ���� Ÿ���� �Ÿ��� ���

                        if (distance < minDistance)             // ������ �Ÿ��� ���� ����� �Ÿ��� ���Ͽ�����
                        {
                            minDistance = distance;             // �� ���ٸ� ���� ����� �Ÿ��� ���� ������ �Ÿ��� ����
                            enemyNearby = enemy.transform;      // ���� ���� ���� ����� ������ ����
                        }
                    }
                }
            }
        }
    }
}

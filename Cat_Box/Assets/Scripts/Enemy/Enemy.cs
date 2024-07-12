using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyObject enemy;                          // scriptableObject

    public float hp;                                    // ������ ���� ü��
    public bool isStun = false;
    public Timer StunTimer = new Timer(0.0f);           // ���� Ÿ�̸�
    public Timer StunCoolTimeTimer = new Timer(0.0f);   // ���� ��Ÿ�� Ÿ�̸�

    public List<Transform> wayPoints;                   // ��������
    private Transform nowWay;                           // ���� �����ִ� ��

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if(GameManager.instance.gameSpeed != CatBoxUtils.Enums.GameSpeed.Pause)             // ������ �Ͻ����� ���°� �ƴ� ���
        {
            if (!StunTimer.IsRunning() && wayPoints != null)                                // ���� ���°� �ƴϰ� ��������Ʈ�� ������ ���
            {
                MoveToWayPoint();               // ��������Ʈ�� �̵�
                StunCoolTimeTimer.Update(deltaTime, GameManager.instance.gameSpeed);
            }

            if(!StunTimer.IsRunning())
            {
                isStun = false;
            }
        }
    }
    public void ResetEnemy()            // ���� �ʱ� ���·� ����� �Լ�
    {
        hp = enemy.maxHp;               // ü���� �ִ�� ����
        StunTimer = new Timer(enemy.stunTime);                  // ���� Ÿ�̸��� �ð��� �ʱ�ȭ
        StunCoolTimeTimer = new Timer(enemy.stunCoolTime);      // ���� ��Ÿ�� Ÿ�̸��� �ð��� �ʱ�ȭ
    }

    private void MoveToWayPoint()
    {

    }

    public void Stun()
    {
        if (isStun) return;
        StunTimer.Start();
        isStun = true;
    }

    public void GetDMG(float damage)
    {
        hp -= damage;

        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}

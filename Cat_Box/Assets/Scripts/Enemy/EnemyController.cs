using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class EnemyController : MonoBehaviour
{
    public EnemyObject enemy;                           // scriptableObject

    public float hp;                                    // ������ ���� ü��
    public Enums.MoveState moveState = Enums.MoveState.DEFAULT;        // ������ ���� ���� ����

    public Timer StunTimer = new Timer(0.0f);           // ���� Ÿ�̸�
    public Timer StunCoolTimeTimer = new Timer(0.0f);   // ���� ��Ÿ�� Ÿ�̸�
    public Timer SlowDownTimer = new Timer(0.0f);       // ���ο� Ÿ�̸�

    public float moveSpeed = 0;                         // �ӵ�

    public List<Transform> wayPoints;                   // ��������
    public int nowWayIndex = 0;                         // ���� ���� �ִ� �� �ε���
    private void Update()
    {
        if(moveState == Enums.MoveState.DEFAULT)        // ���°� �⺻ ������ ���
        {
            if(hp == 0)                                 // ���� ����ִµ�(�Ѿ��� �¾Ƽ� ü���� ������ �ƴѵ�) ü���� 0�� ���
            {
                ResetEnemy();                           // ������ �ʱ�ȭ
            }
            else
            {
                moveState = Enums.MoveState.MOVE;       // �����Ͱ� ������ �̵����·�
            }
        }

        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause)             // ������ �Ͻ����� ���°� �ƴ� ���
        {
            float deltaTime = Time.deltaTime;

            if (moveState != Enums.MoveState.STUN && wayPoints != null)                                // ���� ���°� �ƴϰ� ��������Ʈ�� ������ ���
            {
                MoveToWayPoint(deltaTime);               // ��������Ʈ�� �̵�
                StunTimer.Update(deltaTime, GameManager.instance.gameSpeed);                    // ���� Ÿ�̸� �۵�
                StunCoolTimeTimer.Update(deltaTime, GameManager.instance.gameSpeed);            // ���� ��Ÿ�� Ÿ�̸� �۵�
                SlowDownTimer.Update(deltaTime, GameManager.instance.gameSpeed);                // ���ο� Ÿ�̸� �۵�
            }

            if(!StunTimer.IsRunning())                              // ���� Ÿ�̸Ӱ� ���� ���� ���
            {
                if (moveState == Enums.MoveState.STUN)              // ���� ���� ���·� �Ǿ��ִٸ�
                {
                    moveState = Enums.MoveState.DEFAULT;            // �⺻ ���·� ����
                    StunCoolTimeTimer.Start();                      // ��Ÿ�� Ÿ�̸� ����
                }
            }

            if (!SlowDownTimer.IsRunning())                         // ���ο� Ÿ�̸Ӱ� ���� ���� ���
            {
                if (moveState == Enums.MoveState.SLOWDOWN)          // ���� ���ο� ���¶��
                {
                    moveSpeed = enemy.moveSpeed;
                    moveState = Enums.MoveState.DEFAULT;            // �⺻ ���·� ����
                }
            }
        }
    }
    public void ResetEnemy()            // ���� �ʱ� ���·� ����� �Լ�
    {
        hp = enemy.maxHp;               // ü���� �ִ�� ����
        StunTimer = new Timer(enemy.stunTime);                  // ���� Ÿ�̸��� �ð��� �ʱ�ȭ
        StunCoolTimeTimer = new Timer(enemy.stunCoolTime);      // ���� ��Ÿ�� Ÿ�̸��� �ð��� �ʱ�ȭ
        moveSpeed = enemy.moveSpeed;                            // �⺻ �̵��ӵ��� �ʱ�ȭ
    }
    public float ToNextWay()            // ���� ���� �ִ� ��������Ʈ ���� ���� �Ÿ��� �����ϴ� �Լ�
    {
        return Vector3.Distance(transform.position, wayPoints[nowWayIndex].position);
    }

    private void MoveToWayPoint(float deltaTime)       // ���� ���� �ϴ� ��������Ʈ�� �̵��ϴ� �Լ�
    {
        if(GameManager.instance.gameSpeed == Enums.GameSpeed.Fast)
        {
            deltaTime *= 2;
        }
        else if (GameManager.instance.gameSpeed == Enums.GameSpeed.Slow)
        {
            deltaTime /= 2;
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[nowWayIndex].position, moveSpeed * deltaTime);

        if (IsReached())
        {
            if(wayPoints.Count < nowWayIndex + 2)
            {
                Attack();
                Die();
            }
            else
            {
                nowWayIndex++;
            }
        }

        transform.LookAt(wayPoints[nowWayIndex]);
    }

    private bool IsReached()
    {
        if(Vector3.Distance(transform.position, wayPoints[nowWayIndex].position) < 0.01f)
        {
            return true;
        }
        return false;
    }

    private void Attack()
    {
        GameManager.instance.gameController.GetDamage(enemy.damage);
    }

    public void Stun()
    {
        if(StunCoolTimeTimer.IsRunning()) return;
        if(moveState == Enums.MoveState.STUN) return;

        if(moveState == Enums.MoveState.SLOWDOWN)
        {
            SlowDownTimer.Reset();
        }

        StunTimer.Start();
        moveState = Enums.MoveState.STUN;
    }

    public void SlowDown(float amount, float time)
    {
        if (moveState == Enums.MoveState.SLOWDOWN || moveState == Enums.MoveState.SLOWDOWN) return;
        if (enemy.moveSpeed - amount <= 0) moveSpeed = 0.1f;

        SlowDownTimer = new Timer(time <= 0 ? 1 : time);
        SlowDownTimer.Start();
        moveState = Enums.MoveState.SLOWDOWN;
    }

    public void GetDMG(float damage)
    {
        hp -= damage;

        if(hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    private void Die()
    {
        EnemyManager.Instance.activeEnemies.Remove(this);
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}

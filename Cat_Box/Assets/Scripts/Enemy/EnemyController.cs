using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class EnemyController : MonoBehaviour
{
    public EnemyObject enemy;                          // scriptableObject

    public float hp;                                    // 몬스터의 현재 체력
    public Enums.MoveState moveState = Enums.MoveState.DEFAULT;        // 몬스터의 현재 동작 상태

    public Timer StunTimer = new Timer(0.0f);           // 기절 타이머
    public Timer StunCoolTimeTimer = new Timer(0.0f);   // 기절 쿨타임 타이머
    public Timer SlowDownTimer = new Timer(0.0f);       // 슬로우 타이머

    public float slowerMoveSpeed = 0;                   // 느려진 속도

    public List<Transform> wayPoints;                   // 목적지들
    public int nowWayIndex = 0;                         // 지금 가고 있는 곳 인덱스
    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if(moveState == Enums.MoveState.DEFAULT)
        {
            if(hp == 0)
            {
                ResetEnemy();
            }
            else
            {
                moveState = Enums.MoveState.MOVE;
            }
        }

        if(GameManager.instance.gameSpeed != CatBoxUtils.Enums.GameSpeed.Pause)             // 게임이 일시정지 상태가 아닐 경우
        {
            if (moveState != Enums.MoveState.STUN && wayPoints != null)                                // 스턴 상태가 아니고 웨이포인트가 존재할 경우
            {
                MoveToWayPoint();               // 웨이포인트로 이동
                StunTimer.Update(deltaTime, GameManager.instance.gameSpeed);                    // 스턴 타이머 작동
                StunCoolTimeTimer.Update(deltaTime, GameManager.instance.gameSpeed);            // 스턴 쿨타임 타이머 작동
                SlowDownTimer.Update(deltaTime, GameManager.instance.gameSpeed);                // 슬로우 타이머 작동
            }

            if(!StunTimer.IsRunning())                              // 스턴 타이머가 멈춰 있을 경우
            {
                if (moveState == Enums.MoveState.STUN)              // 만약 스턴 상태로 되어있다면
                {
                    moveState = Enums.MoveState.DEFAULT;            // 기본 상태로 변경
                    StunCoolTimeTimer.Start();                      // 쿨타임 타이머 실행
                }
            }

            if (!SlowDownTimer.IsRunning())                         // 슬로우 타이머가 멈춰 있을 경우
            {
                if (moveState == Enums.MoveState.SLOWDOWN)          // 만약 슬로우 상태라면
                {
                    moveState = Enums.MoveState.DEFAULT;            // 기본 상태로 변경
                }
            }
        }
    }
    public void ResetEnemy()            // 적을 초기 상태로 만드는 함수
    {
        hp = enemy.maxHp;               // 체력을 최대로 만듬
        StunTimer = new Timer(enemy.stunTime);                  // 스턴 타이머의 시간을 초기화
        StunCoolTimeTimer = new Timer(enemy.stunCoolTime);      // 스턴 쿨타임 타이머의 시간을 초기화
    }

    private void MoveToWayPoint()
    {
        if(moveState == Enums.MoveState.MOVE)
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[nowWayIndex].position, enemy.moveSpeed * Time.deltaTime);
        }
        else if(moveState == Enums.MoveState.SLOWDOWN)
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[nowWayIndex].position, slowerMoveSpeed * Time.deltaTime);
        }

        if(IsReached())
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
        if (enemy.moveSpeed - amount <= 0) slowerMoveSpeed = 0.1f;

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
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using Unity.VisualScripting;

public class EnemyController : MonoBehaviour
{
    public EnemyObject enemy;                           // scriptableObject

    public float hp;                                    // 몬스터의 현재 체력
    public Enums.MoveState moveState = Enums.MoveState.DEFAULT;        // 몬스터의 현재 동작 상태

    public Timer StunTimer = new Timer(0.0f);           // 기절 타이머
    public Timer StunCoolTimeTimer = new Timer(0.0f);   // 기절 쿨타임 타이머
    public Timer SlowDownTimer = new Timer(0.0f);       // 슬로우 타이머

    public float moveSpeed = 0;                         // 속도

    public List<Transform> wayPoints;                   // 목적지들
    public int nowWayIndex = 0;                         // 지금 가고 있는 곳 인덱스
    private void Start()
    {
        if(!TryGetComponent<Collider>(out var collider))
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    private void Update()
    {
        if(moveState == Enums.MoveState.DEFAULT)        // 상태가 기본 상태일 경우
        {
            if(hp == 0)                                 // 만약 살아있는데(총알을 맞아서 체력이 닳은게 아닌데) 체력이 0일 경우
            {
                ResetEnemy();                           // 데이터 초기화
            }
            else
            {
                moveState = Enums.MoveState.MOVE;       // 데이터가 있으면 이동상태로
            }
        }

        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause)             // 게임이 일시정지 상태가 아닐 경우
        {
            float deltaTime = Time.deltaTime;

            if (moveState != Enums.MoveState.STUN && wayPoints != null)                                // 스턴 상태가 아니고 웨이포인트가 존재할 경우
            {
                MoveToWayPoint(deltaTime);               // 웨이포인트로 이동
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
                    moveSpeed = enemy.moveSpeed;
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
        moveSpeed = enemy.moveSpeed;                            // 기본 이동속도로 초기화
    }
    public float ToNextWay()            // 지금 가고 있는 웨이포인트 까지 남은 거리를 리턴하는 함수
    {
        return Vector3.Distance(transform.position, wayPoints[nowWayIndex].position);
    }

    private void MoveToWayPoint(float deltaTime)       // 지금 가야 하는 웨이포인트로 이동하는 함수
    {
        if(GameManager.instance.gameSpeed == Enums.GameSpeed.Fast)          // 게임 속도가 2배일 때
        {
            deltaTime *= 2;
        }
        else if (GameManager.instance.gameSpeed == Enums.GameSpeed.Slow)        // 게임 속도가 0.5배일 때
        {
            deltaTime /= 2;
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[nowWayIndex].position, moveSpeed * deltaTime);       // 이동

        if (IsReached())        // 웨이 포인트에 도착했다면
        {
            if(wayPoints.Count < nowWayIndex + 2)       // 만약 마지만 웨이포인트라면
            {
                Attack();       // 본체(플레이어의 체력을 깎음)
                Die(true);          // 사망
            }
            else
            {
                nowWayIndex++;  // 다음 웨이포인트로
            }
        }

        transform.LookAt(wayPoints[nowWayIndex]);       // 다음 웨이포인트 방향으로 바라봄
    }

    private bool IsReached()
    {
        if(Vector3.Distance(transform.position, wayPoints[nowWayIndex].position) < 0.01f)       // 0.01보다 가깝다면
        {
            transform.position = wayPoints[nowWayIndex].position;       // 웨이 포인트 위치로 적의 위치를 이동 시킴
            return true;
        }
        return false;
    }

    private void Attack()           // 체력을 깎음
    {
        GameManager.instance.GetDamage(enemy.damage);
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

    private void Die(bool isSelf = false)
    {
        if (!isSelf)
        {
            GameManager.instance.playerData.inGameMoney += enemy.reward;
        }
        EnemyManager.Instance.activeEnemies.Remove(this);
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyObject enemy;                          // scriptableObject

    public float hp;                                    // 몬스터의 현재 체력

    public Timer StunTimer = new Timer(0.0f);           // 기절 타이머
    public Timer StunCoolTimeTimer = new Timer(0.0f);   // 기절 쿨타임 타이머

    public List<Transform> wayPoints;                   // 목적지들
    private Transform nowWay;                           // 지금 가고있는 곳

    private void Update()
    {
        if(GameManager.instance.gameSpeed != CatBoxUtils.Enums.GameSpeed.Pause)             // 게임이 일시정지 상태가 아닐 경우
        {
            if (!StunTimer.IsRunning() && wayPoints != null)                                // 스턴 상태가 아니고 웨이포인트가 존재할 경우
            {
                MoveToWayPoint();               // 웨이포인트로 이동
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

    }

    private void Die()
    {
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}

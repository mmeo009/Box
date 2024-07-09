using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class Tower : MonoBehaviour
{
    public TowerObject tower;           // 타워의 기본적인 속성을 가져옴

    public Timer attackRateTimer;       // 타워의 공격 속도를 계산할 타이머

    public List<Transform> enemys;      // 공격할 적들의 리스트 (사정거리 이내의 적들)

    private void Start()
    {
        attackRateTimer = new Timer(tower.baseAttackRate);          // 타워오브젝트의 공격 속도를 가져옴
        attackRateTimer.Start();                                    // 타이머를 작동시킴
    }
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause) // 게임이 일시정지 상태가 아닐경우
        {
            if(attackRateTimer != null && attackRateTimer.IsRunning())      // 타이머가 존재하고 타이머가 작동중일 때
            {
                float deltaTime = Time.deltaTime;                   // 시간을 가져옴

                attackRateTimer.Update(deltaTime, GameManager.instance.gameSpeed);      // 타이머에 시간과 현재 게임메니저의 시간상태를 가져옴

                if (!attackRateTimer.IsRunning())                   // 타이머가 실행중이지 않을 때
                {
                    Attack();                                       // 공격을 함
                    attackRateTimer.Start();                        // 타이머를 초기화 함
                }
            }

        }
    }

    private void Attack()
    {
        var colliders = Physics.OverlapSphere(transform.position, tower.baseRange);         // 사정거리 이내의 모든 오브젝트를 가져옴

        enemys.Clear();                         // 적 리스트 초기화

        foreach(var enemy in colliders)
        {
            if(enemy.tag == "Enemy")            // 테그가 적일 경우
            {
                enemys.Add(enemy.transform);    // 적 리스트에 추가
            }
        }

        if(enemys.Count > 0)                                // 리스트에 적이 한개 이상일 경우
        {
            if (tower.towerType == Enums.TowerType.BASIC)   // 총알을 발사하는 기본적인 타워일 경우
            {
                float minDistance = tower.baseRange + 1f;   // 가장 가까운 거리에 존재하는 적을 찾아내기 위해 필요한 최단거리
                Transform enemyNearby;                      // 가장 가까운 거리에 존재하는 적(공격할 적)

                if(enemys.Count == 1)
                {
                    enemyNearby = enemys[0].transform;      // 적 리스트에 적이 한개 뿐이라면 그 적을 가장 가까운 적에 추가
                }
                else
                {
                    foreach (var enemy in enemys)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);        // 적과 타워의 거리를 계산

                        if (distance < minDistance)             // 적과의 거리를 가장 가까운 거리와 비교하였을때
                        {
                            minDistance = distance;             // 더 적다면 가장 가까운 거리를 지금 적과의 거리로 변경
                            enemyNearby = enemy.transform;      // 지금 적을 가장 가까운 적으로 갱신
                        }
                    }
                }
            }
        }
    }
}

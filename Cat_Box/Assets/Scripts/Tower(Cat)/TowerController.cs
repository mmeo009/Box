using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class TowerController : MonoBehaviour
{
    public TowerObject tower;           // 타워의 기본적인 속성을 가져옴

    public int towerLevel = 1;          // 타워의 레벨을 가져옴

    public Timer attackRateTimer;       // 타워의 공격 속도를 계산할 타이머

    public List<Transform> enemys;      // 공격할 적들의 리스트 (사정거리 이내의 적들)

    private void Start()
    {
        attackRateTimer = new Timer(tower.tower[towerLevel - 1].baseAttackRate);          // 타워오브젝트의 공격 속도를 가져옴
        attackRateTimer.Start();                                                          // 타이머를 작동시킴
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
                }
            }

        }
    }

    private void Attack()
    {
        var colliders = Physics.OverlapSphere(transform.position, tower.tower[towerLevel - 1].baseRange);         // 사정거리 이내의 모든 오브젝트를 가져옴

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

        attackRateTimer.Start();                        // 타이머를 초기화 함
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
            float distance = Vector3.Distance(transform.position, enemy.transform.position);        // 적과 타워의 거리를 계산

            if (distance < minDistance)             // 적과의 거리를 가장 가까운 거리와 비교하였을때
            {
                minDistance = distance;             // 더 적다면 가장 가까운 거리를 지금 적과의 거리로 변경
                proximateEnemy = enemy.transform;      // 지금 적을 가장 가까운 적으로 갱신
            }
        }
        return proximateEnemy;
    }

    private void FireProjectile(Enums.TowerType tower, Transform target)
    {

    }

    private void OnDrawGizmosSelected()             // 타워 오브젝트를 선택했을 시 나타나는 기즈모
    {
        Gizmos.color = Color.blue;                  // 기즈모를 그릴 색상 파랑
        Gizmos.DrawWireSphere(transform.position, tower.tower[towerLevel].baseAttackRate);          // 사정거리를 표시함

        if (enemys.Count > 0)
        {
            if(tower.towerType == Enums.TowerType.BASIC)            // 타워의 공격 방식이 기본일 경우
            {
                Gizmos.color = Color.red;           // 빨간색으로
                Transform proximateEnemy = GetProximateEnemy();
                if(proximateEnemy != null)
                Gizmos.DrawLine(transform.position, proximateEnemy.position);        // 타워로부터 적까지 선을 그림
            }
            else if(tower.towerType == Enums.TowerType.SLOWDOWN)   // 타워의 공격 방식이 슬로우(스턴)일 경우
            {

                for(int i = 0; i < enemys.Count; i ++)
                {
                    if (enemys[i].GetComponent<Enemy>().isStun)
                    {
                        Gizmos.color = Color.yellow;       // 스턴일경우 노란색으로
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;         // 아닐경우 하늘색으로
                    }
                    Gizmos.DrawSphere(enemys[i].transform.position, 1f);        // 공격 범위 내의 적들에게 구를 그림
                }
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class BulletController : MonoBehaviour
{
    public TowerController myTower;         // 이 총알을 발사한 타워
    public Vector3 target;                  // 내가 날라갈 타겟
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause)
        {
            // 게임 속도에 따른 총알 속도 변경
            if(GameManager.instance.gameSpeed == Enums.GameSpeed.Slow)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, myTower.towerObject.tower[myTower.towerLevel -1].bulletSpeed / 2 * Time.deltaTime);
            }
            else if(GameManager.instance.gameSpeed == Enums.GameSpeed.Fast)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, myTower.towerObject.tower[myTower.towerLevel - 1].bulletSpeed * 2 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target, myTower.towerObject.tower[myTower.towerLevel -1].bulletSpeed * Time.deltaTime);
            }

            // 총알이 포탑 사거리 밖으로 나갔을 경우 제거
            if(Vector3.Distance(transform.position, myTower.transform.position) >= myTower.towerObject.tower[myTower.towerLevel -1].baseAttackRate + 2)
            {
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")                // 충돌한 오브젝트의 테그가 적일 경우
        {
            other.GetComponent<EnemyController>().GetDMG(myTower.towerObject.tower[myTower.towerLevel -1].baseDamage);      // 내 타워의 레벨 만큼 데미지를 줌
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        myTower = null;                 // 내 타워 삭제
        target = Vector3.zero;          // 타겟을 원점으로 바꿈
        PoolManager.Instance.ReturnToPool(gameObject);      // 이 총알 오브젝트 삭제
    }
}

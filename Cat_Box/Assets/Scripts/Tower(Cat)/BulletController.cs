using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class BulletController : MonoBehaviour
{
    public TowerController myTower;
    public Vector3 target;
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause)
        {
            if(GameManager.instance.gameSpeed == Enums.GameSpeed.Slow)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, myTower.tower.tower[myTower.towerLevel].bulletSpeed / 2 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target, myTower.tower.tower[myTower.towerLevel].bulletSpeed * Time.deltaTime);
            }

            if(Vector3.Distance(transform.position, myTower.transform.position) < myTower.tower.tower[myTower.towerLevel].baseAttackRate + 2)
            {
                PoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyController>().GetDMG(myTower.tower.tower[myTower.towerLevel].baseDamage);
            PoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}

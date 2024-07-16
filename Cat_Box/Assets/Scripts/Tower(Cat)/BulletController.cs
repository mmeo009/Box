using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using DG.Tweening;

public class BulletController : MonoBehaviour
{
    public TowerController myTower;         // �� �Ѿ��� �߻��� Ÿ��
    public Vector3 target;                  // ���� ���� Ÿ��
    public Tweener moveTween;               // ������ Ʈ��
    void Update()
    {
        if(GameManager.instance.gameSpeed != Enums.GameSpeed.Pause)
        {
            if (moveTween != null)
                moveTween.Kill();

            // ���� �ӵ��� ���� �Ѿ� �ӵ� ����
            if (GameManager.instance.gameSpeed == Enums.GameSpeed.Slow)
            {
                moveTween = transform.DOMove(target, Vector3.Distance(transform.position, target) / (myTower.towerObject.tower[myTower.towerLevel - 1].bulletSpeed / 2));
            }
            else if(GameManager.instance.gameSpeed == Enums.GameSpeed.Fast)
            {
                moveTween = transform.DOMove(target, Vector3.Distance(transform.position, target) / (myTower.towerObject.tower[myTower.towerLevel - 1].bulletSpeed * 2));
            }
            else
            {
                moveTween = transform.DOMove(target, Vector3.Distance(transform.position, target) / myTower.towerObject.tower[myTower.towerLevel - 1].bulletSpeed);
            }

            // �Ѿ��� ��ž ��Ÿ� ������ ������ ��� ����
            if(Vector3.Distance(transform.position, myTower.transform.position) >= myTower.towerObject.tower[myTower.towerLevel -1].baseAttackRate + 2)
            {
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")                // �浹�� ������Ʈ�� �ױװ� ���� ���
        {
            other.GetComponent<EnemyController>().GetDMG(myTower.towerObject.tower[myTower.towerLevel -1].baseDamage);      // �� Ÿ���� ���� ��ŭ �������� ��
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        myTower = null;                 // �� Ÿ�� ����
        target = Vector3.zero;          // Ÿ���� �������� �ٲ�
        PoolManager.Instance.ReturnToPool(gameObject);      // �� �Ѿ� ������Ʈ ����
    }
}

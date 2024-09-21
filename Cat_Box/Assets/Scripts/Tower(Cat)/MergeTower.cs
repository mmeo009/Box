using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTower : MonoBehaviour
{
    public bool isDragging = false;     // �巡�� ������
    public bool isFirst = false;        // ó�� �����Ȱ���
    public TowerController tower;       // Ÿ�� ��Ʈ�ѷ�

    public Vector3 origin;              // ���� ������ (�巡�� ���н� ���ư��� �ϴϱ�)
    public TowerGrid originGrid;        // ���� �׸���
    public Vector3 offset;
    private void OnMouseDown()
    {
        if(!isFirst)
        {
            origin = transform.position;                                // ���� ��ġ ����
            offset = gameObject.transform.position - GetWorldPositon(); // offset ��ġ ������
            isDragging = true;          // �巡�� �� Ȱ��ȭ
        }
        else
        {
            isDragging = false;                     // �巡�� ����
            EndDrag();
        }
    }
    private void OnMouseDrag()
    {
        if (isDragging && !isFirst)             // �巡�� ���̶��
        {
            transform.position = GetWorldPositon() + offset;            // ���콺 ����ٴϱ�
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;                     // �巡�� ����

        if (!isFirst)
        {
            EndDrag();
        }

        if (isFirst) isFirst = false;                        // ��� ������ ģ���� �̵��� �����⿡ FALSE�� �ٲ�

    }
    private void Update()
    {
        if(isDragging && isFirst)
        {
            FollowMousePos();
        }
    }

    public void FollowMousePos()
    {
        transform.position = GetWorldPositon();
    }
    private void EndDrag()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.8f);         // �ٹ��� ������Ʈ���� ������

        foreach (var coll in colliders)
        {
            if (coll.tag == "Tower" && coll.enabled)     // Ÿ�� �ױ׸� ������ ���� ���
            {
                var towerController = coll.GetComponent<TowerController>();     // Ÿ�� ��Ʈ�ѷ��� ������

                if (towerController != this)                // �� �ڽ��� �ƴϰ�
                {
                    if (CheckMerge(towerController))        // ���� �������� Ȯ���ϰ� ������ ���
                    {
                        towerController.towerLevel++;       // �� Ÿ���� ������ �ø�

                        if (towerController.merge.originGrid != null) towerController.merge.originGrid.ChangeBoxObject(towerController.towerLevel);  // �׸��� ���� ����

                        tower.towerLevel = 0;                     // ���� ������ �ʱ�ȭ

                        if (originGrid != null)     // ���� �׸��尡 ���� ���
                        {
                            originGrid.myTower = null;          // �׸��� �ʱ�ȭ
                            originGrid.ChangeBoxObject(1);      // �׸��� ���� �ʱ�ȭ
                        }

                        PoolManager.Instance.ReturnToPool(this.gameObject); // �� ������Ʈ�� ����
                        return;     // �ݺ��� ����
                    }
                }
            }
        }

        TowerGrid towerGrid;

        foreach (var coll in colliders)
        {
            if (coll.tag == "Grid")
            {
                towerGrid = coll.GetComponent<TowerGrid>();

                if (towerGrid.myTower == null)
                {
                    transform.position = new Vector3(towerGrid.transform.position.x, towerGrid.transform.position.y + 0.7f, towerGrid.transform.position.z);       // �׸��尡 ������ �׸��� ��ġ��
                    origin = Vector3.zero;             // ���� ��ġ ���� ���� �ʱ�ȭ

                    if (originGrid != null)     // ���� �׸��尡 ���� ���
                    {
                        originGrid.myTower = null;          // ���� �׸��� �ʱ�ȭ
                        originGrid.ChangeBoxObject(1);      // ���� �׸��� ���� �ʱ�ȭ
                    }

                    towerGrid.myTower = tower;          // ���ο� �׸��忡 ���� ����
                    towerGrid.ChangeBoxObject(tower.towerLevel);
                    originGrid = towerGrid;             // ���ο� �׸��带 �� �׸����

                    return;
                }
            }
        }
        if (origin != null)
        {
            OnMoveFail();
        }
        else
        {
            DestroyThisTower();
        }
    }
    private void DestroyThisTower()
    {
        GameManager.instance.ChangeMoney(CatBoxUtils.Enums.MoneyType.INGAME, tower.towerObject.costInGame);

        if (originGrid != null)
        {
            originGrid.myTower = null;
            originGrid.ChangeBoxObject(1);
        }

        PoolManager.Instance.ReturnToPool(this.gameObject); // �� ������Ʈ�� ����
    }
    private void OnMoveFail()
    {
        if(isFirst == true)
        {
            DestroyThisTower();
        }
        transform.position = origin;       // ������ �Ұ��� �Ұ�� ���� ��ġ�� �ű�
        origin = Vector3.zero;             // ���� ��ġ ���� ���� �ʱ�ȭ
    }
    private bool CheckMerge(TowerController hit)
    {
        if(hit == tower)            // ���� ����
        {
            return false;
        }

        if (hit.towerLevel == tower.towerLevel && hit.towerObject == tower.towerObject)      // ���� Ÿ�� ������ ���� Ÿ�� �Ӽ��� ���� ���
        {
            if(tower.towerObject.tower.Count > tower.towerLevel)                // Ÿ�� ������ �������� �Ѱ�����
            {
                return true;    // ���� ������
            }
            else
            {
                return false;       // ���� �Ұ���
            }
        }
        return false;       // ���� �Ұ�����
    }
    private Vector3 GetWorldPositon()
    {
        Vector3 mousePoint = Input.mousePosition;                   // ���콺 ��ġ�� ������
        Camera cam = Camera.main;                                   // ī�޶�� ���� ī�޶�
        mousePoint.z = cam.WorldToScreenPoint(gameObject.transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
}

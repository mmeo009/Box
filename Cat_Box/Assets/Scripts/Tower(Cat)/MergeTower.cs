using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTower : MonoBehaviour
{
    public bool isDragging = false;     // �巡�� ������
    public TowerController tower;       // Ÿ�� ��Ʈ�ѷ�

    public Vector3 origin;              // ���� ������ (�巡�� ���н� ���ư��� �ϴϱ�)
    public Vector3 offset;
    private void OnMouseDown()
    {
        origin = transform.position;                                // ���� ��ġ ����
        offset = gameObject.transform.position - GetWorldPositon(); // offset ��ġ ������
        isDragging = true;          // �巡�� �� Ȱ��ȭ
    }
    private void OnMouseDrag()
    {
        if (isDragging)             // �巡�� ���̶��
        {
            transform.position = GetWorldPositon() + offset;            // ���콺 ����ٴϱ�
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;                     // �巡�� ����
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
                        tower.towerLevel = 0;                     // ���� ������ �ʱ�ȭ
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
                    transform.position = new Vector3(towerGrid.transform.position.x, towerGrid.transform.position.y + 0.5f, towerGrid.transform.position.z);       // �׸��尡 ������ �׸��� ��ġ��
                    origin = Vector3.zero;             // ���� ��ġ ���� ���� �ʱ�ȭ
                    towerGrid.myTower = tower;
                    return;
                }
            }
        }

        if(origin != null)
        {
            transform.position = origin;       // ������ �Ұ��� �Ұ�� ���� ��ġ�� �ű�
            origin = Vector3.zero;             // ���� ��ġ ���� ���� �ʱ�ȭ
        }
        else
        {
            GameManager.instance.playerData.inGameMoney += tower.towerObject.costInGame;
            PoolManager.Instance.ReturnToPool(this.gameObject); // �� ������Ʈ�� ����
        }

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

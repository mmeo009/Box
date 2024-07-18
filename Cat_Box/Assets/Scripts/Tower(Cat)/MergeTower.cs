using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTower : MonoBehaviour
{
    public bool isDragging = false;     // 드래그 중인지
    public TowerController tower;       // 타워 컨트롤러

    public Vector3 origin;              // 원래 포지션 (드래그 실패시 돌아가야 하니까)
    public Vector3 offset;
    private void OnMouseDown()
    {
        origin = transform.position;                                // 기존 위치 저장
        offset = gameObject.transform.position - GetWorldPositon(); // offset 위치 가져옴
        isDragging = true;          // 드래그 중 활성화
    }
    private void OnMouseDrag()
    {
        if (isDragging)             // 드래그 중이라면
        {
            transform.position = GetWorldPositon() + offset;            // 마우스 따라다니기
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;                     // 드래그 종료
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.8f);         // 근방의 오브젝트들을 가져옴

        foreach (var coll in colliders)
        {
            if (coll.tag == "Tower" && coll.enabled)     // 타워 테그를 가지고 있을 경우
            {
                var towerController = coll.GetComponent<TowerController>();     // 타워 컨트롤러를 가져옴

                if (towerController != this)                // 나 자신은 아니고
                {
                    if (CheckMerge(towerController))        // 머지 가능한지 확인하고 가능할 경우
                    {
                        towerController.towerLevel++;       // 그 타워의 레벨을 올림
                        tower.towerLevel = 0;                     // 나의 레벨은 초기화
                        PoolManager.Instance.ReturnToPool(this.gameObject); // 이 오브젝트를 제거
                        return;     // 반복문 종료
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
                    transform.position = new Vector3(towerGrid.transform.position.x, towerGrid.transform.position.y + 0.5f, towerGrid.transform.position.z);       // 그리드가 있으면 그리드 위치로
                    origin = Vector3.zero;             // 기존 위치 관련 정보 초기화
                    towerGrid.myTower = tower;
                    return;
                }
            }
        }

        if(origin != null)
        {
            transform.position = origin;       // 머지가 불가능 할경우 기존 위치로 옮김
            origin = Vector3.zero;             // 기존 위치 관련 정보 초기화
        }
        else
        {
            GameManager.instance.playerData.inGameMoney += tower.towerObject.costInGame;
            PoolManager.Instance.ReturnToPool(this.gameObject); // 이 오브젝트를 제거
        }

    }
    private bool CheckMerge(TowerController hit)
    {
        if(hit == tower)            // 나는 제외
        {
            return false;
        }

        if (hit.towerLevel == tower.towerLevel && hit.towerObject == tower.towerObject)      // 나의 타워 레벨과 같고 타워 속성이 같을 경우
        {
            if(tower.towerObject.tower.Count > tower.towerLevel)                // 타워 레벨이 합쳐지는 한계인지
            {
                return true;    // 머지 가능함
            }
            else
            {
                return false;       // 머지 불가능
            }
        }
        return false;       // 머지 불가능함
    }
    private Vector3 GetWorldPositon()
    {
        Vector3 mousePoint = Input.mousePosition;                   // 마우스 위치를 가져옴
        Camera cam = Camera.main;                                   // 카메라는 메인 카메라
        mousePoint.z = cam.WorldToScreenPoint(gameObject.transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWayPoint : MonoBehaviour
{
    public bool isStartPoint;               // 스폰 포인트 인지
    public bool isEndPoint;                 // 종료 포인트 인지
    public int index;                       // 웨이포인트 순번
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;       // 자홍색으로

        if (isEndPoint || isStartPoint) Gizmos.color = Color.yellow;        // 시작이나 끝인경우 노란색으로

        Gizmos.DrawCube(transform.position, Vector3.one);                   // Scene창에서만 보이게 큐브로 표시
    }
}
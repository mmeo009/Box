using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWayPoint : MonoBehaviour
{
    public bool isStartPoint;               // ���� ����Ʈ ����
    public bool isEndPoint;                 // ���� ����Ʈ ����
    public int index;                       // ��������Ʈ ����
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;       // ��ȫ������

        if (isEndPoint || isStartPoint) Gizmos.color = Color.yellow;        // �����̳� ���ΰ�� ���������

        Gizmos.DrawCube(transform.position, Vector3.one);                   // Sceneâ������ ���̰� ť��� ǥ��
    }
}
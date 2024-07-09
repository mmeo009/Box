using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

[CreateAssetMenu(fileName = "CatObject",menuName = "ScriptableObject/TowerObject")]
public class TowerObject : ScriptableObject
{
    public string towerName;            // Ÿ���� �̸�
    public int costInStore;             // �������� �����ϴ� ����
    public int costInGame;              // �ΰ��ӿ��� ��ġ�ϴ� ����

    [Range(0,30)]
    public float baseDamage;            // �⺻ ������
    [Range(0, 10)]
    public float baseRange;             // �⺻ ��Ÿ�
    [Range(0, 10)]
    public float bulletSpeed;           // �Ѿ��� �ӵ�
    [Range(0, 10000)]
    public float baseAttackRate;        // �⺻ ���� �ӵ� (���� ������ �ð�)

    public int maxLevel;                // Ÿ���� �ִ� ����

    public Enums.TowerType towerType;         // Ÿ���� Ÿ��

    public GameObject towerObject;      // Ÿ�� ���� ������Ʈ


}

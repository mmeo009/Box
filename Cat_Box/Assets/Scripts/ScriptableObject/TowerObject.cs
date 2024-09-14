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

    public List<TowerStat> tower = new List<TowerStat>();       // ���� �� Ÿ����

    public int maxLevel;                // Ÿ���� �ִ� ����

    public Enums.TowerType towerType;   // Ÿ���� Ÿ��

    public GameObject towerObject;      // Ÿ�� ���� ������Ʈ
    public GameObject bulletObject;     // �Ѿ� ���� ������Ʈ
    public Sprite towerImage;           // Ÿ���� �̹���
}
[System.Serializable]
public class TowerStat
{
    [Range(0, 30)]
    public float baseDamage;            // �⺻ ������
    [Range(1, 10)]
    public float baseRange;             // �⺻ ��Ÿ�
    [Range(8, 20)]
    public float bulletSpeed;           // �Ѿ��� �ӵ�
    [Range(0, 200)]
    public float baseAttackRate;        // �⺻ ���� �ӵ� (���� ������ �ð�)
}

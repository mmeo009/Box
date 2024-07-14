using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

[CreateAssetMenu(fileName = "EnemyObject", menuName = "ScriptableObject/EnemyObject")]
public class EnemyObject : ScriptableObject
{
    public float maxHp;             // �ִ� ü��
    public float moveSpeed;         // �̵� �ӵ�
    public int damage;              // ���ݷ� (���̽��� ������ ������ ��)
    
    public float stunTime;          // �����Ǵ� �ð�
    public float stunCoolTime;      // ���� ��Ÿ��
    public int reward;              // óġ�� ��� ����� ��

    public GameObject gameObject;       // ���� ��
    public Enums.EnemyType enemyType;   // ������ Ÿ��
}

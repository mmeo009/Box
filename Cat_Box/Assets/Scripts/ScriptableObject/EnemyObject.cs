using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class EnemyObject : ScriptableObject
{
    public float maxHp;             // 최대 체력
    public float moveSpeed;         // 이동 속도
    public int damage;              // 공격력 (베이스에 입히는 데미지 양)
    
    public float stunTime;          // 경직되는 시간
    public float stunCoolTime;      // 경직 쿨타임
    public int reward;              // 처치시 얻는 골드의 양

    public GameObject gameObject;       // 몬스터 모델
    public Enums.EnemyType enemyType;   // 몬스터의 타입
}

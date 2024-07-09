using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

[CreateAssetMenu(fileName = "CatObject",menuName = "ScriptableObject/TowerObject")]
public class TowerObject : ScriptableObject
{
    public string towerName;            // 타워의 이름
    public int costInStore;             // 상점에서 구매하는 가격
    public int costInGame;              // 인게임에서 설치하는 가격

    [Range(0,30)]
    public float baseDamage;            // 기본 데미지
    [Range(0, 10)]
    public float baseRange;             // 기본 사거리
    [Range(0, 10)]
    public float bulletSpeed;           // 총알의 속도
    [Range(0, 10000)]
    public float baseAttackRate;        // 기본 공격 속도 (공격 사이의 시간)

    public int maxLevel;                // 타워의 최대 레벨

    public Enums.TowerType towerType;         // 타워의 타입

    public GameObject towerObject;      // 타워 게임 오브젝트


}

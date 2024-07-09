using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatObject",menuName = "ScriptableObject/TowerObject")]
public class TowerObject : ScriptableObject
{
    public string TowerName;
    public int costInStore;
    public int costInGame;
    [Range(0,30)]
    public int baseDamage;
    [Range(0, 10)]
    public int baseRange;
    [Range(0, 1)]
    public int baseAttackRate;
    public int Level;
}

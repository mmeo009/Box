using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TowerBalanceCalculator : EditorWindow
{
    public TowerObject towerObject;

    public float[] towerDPS;

    [MenuItem("CatBox/Tower DPS Calculator")]
    private static void ShwoWindow()
    {
        var window = GetWindow<TowerBalanceCalculator>();
        window.titleContent = new GUIContent("DPS Calculator");
        window.Show();
    }
    private float CalculateDPS(int num)
    {
        if (towerObject == null) return 0f;

        return towerObject.tower[num].baseDamage / towerObject.tower[num].baseAttackRate;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(400));

        towerObject = (TowerObject)EditorGUILayout.ObjectField("타워 오브젝트 : ", towerObject, typeof(TowerObject), false);

        if (towerObject == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("타워 리스트", EditorStyles.boldLabel);
        GUILayout.Space(10);
        for (int i = 0; i < towerObject.tower.Count; i ++)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label($"레벨 : {i + 1}, 가격 : {towerObject.costInGame * (i == 0 ? 1 : (i * 2))}", EditorStyles.boldLabel);
            GUILayout.Label($"DPS : {CalculateDPS(i)}", EditorStyles.boldLabel);
            GUILayout.Label($"(가격 / DPS) : {(towerObject.costInGame * (i + 1)) / CalculateDPS(i)}", EditorStyles.boldLabel);

            if (i > 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("전 레벨과의 차이", EditorStyles.boldLabel);
                GUILayout.Space(10);
                GUILayout.Label($"전 레벨과의 DPS 차 : {CalculateDPS(i) - CalculateDPS(i -1)}", EditorStyles.boldLabel);
                GUILayout.Label($"전 레벨과의 가격 /DPS 차 : {((towerObject.costInGame * (i + 1)) / CalculateDPS(i)) - ((towerObject.costInGame * (i)) / CalculateDPS(i -1))}", EditorStyles.boldLabel);
            }
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }
}

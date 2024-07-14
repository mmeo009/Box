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

        towerObject = (TowerObject)EditorGUILayout.ObjectField("Ÿ�� ������Ʈ : ", towerObject, typeof(TowerObject), false);

        if (towerObject == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Ÿ�� ����Ʈ", EditorStyles.boldLabel);
        GUILayout.Space(10);
        for (int i = 0; i < towerObject.tower.Count; i ++)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label($"���� : {i + 1}, ���� : {towerObject.costInGame * (i == 0 ? 1 : (i * 2))}", EditorStyles.boldLabel);
            GUILayout.Label($"DPS : {CalculateDPS(i)}", EditorStyles.boldLabel);
            GUILayout.Label($"(���� / DPS) : {(towerObject.costInGame * (i + 1)) / CalculateDPS(i)}", EditorStyles.boldLabel);

            if (i > 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("�� �������� ����", EditorStyles.boldLabel);
                GUILayout.Space(10);
                GUILayout.Label($"�� �������� DPS �� : {CalculateDPS(i) - CalculateDPS(i -1)}", EditorStyles.boldLabel);
                GUILayout.Label($"�� �������� ���� /DPS �� : {((towerObject.costInGame * (i + 1)) / CalculateDPS(i)) - ((towerObject.costInGame * (i)) / CalculateDPS(i -1))}", EditorStyles.boldLabel);
            }
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }
}

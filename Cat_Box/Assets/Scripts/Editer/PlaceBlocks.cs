using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using static UnityEditor.PlayerSettings;

public class PlaceBlocks : EditorWindow
{
    public static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    public Vector3Int positionToPlace = Vector3Int.zero;

    public bool isFill;

    public Vector3Int from;
    public Vector3Int to;

    public string selectedPrefabName;


    [MenuItem("CatBox/Prop Generator")]
    private static void ShwoWindow()
    {
        var window = GetWindow<PlaceBlocks>();
        window.titleContent = new GUIContent("Prop Generator");
        window.Show();
    }

    private string GetAssetName(string path)
    {
        string assetName = path.Substring(path.LastIndexOf('/') + 1);
        return assetName;
    }
    private void LoadPrefabsDatas()
    {
        string[] AssetGuids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/Prefabs/Map" });
        string[] AssetPathList = Array.ConvertAll<string, string>(AssetGuids, AssetDatabase.GUIDToAssetPath);


        foreach (string path in AssetPathList)
        {
            var prefabName = GetAssetName(path);

            if (!prefabs.ContainsKey(prefabName))
            {
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                prefabs.Add(prefabName, (GameObject)prefab);
                Debug.Log(prefabName + "을 불러왔습니다.");
            }
        }
    }
    private void SelectPrefab(string prefabName)
    {
        selectedPrefabName = prefabName;
    }
    private void CreatePrefab()
    {
        if(isFill)
        {
            for(int i = from.x; i <= to.x; i++) 
            {
                if(from.y != 0 && to.y != 0 && from.y != to.y)
                {
                    for (int j = from.y; j <= to.y; j++)
                    {
                        for (int k = from.z; k <= to.z; k++)
                        {
                            var position = new Vector3(i, j, k);
                            var temp = Instantiate(prefabs[selectedPrefabName], position, Quaternion.identity);
                            temp.transform.position = new Vector3(i, j, k);
                        }

                    }
                }
                for (int k = from.z; k <= to.z; k++)
                {
                    var position = new Vector3(i, 0, k);
                    var temp = Instantiate(prefabs[selectedPrefabName], position, Quaternion.identity);
                    temp.transform.position = new Vector3(i, 0, k);
                }
            }
        }
        else
        {
            var temp = Instantiate(prefabs[selectedPrefabName]);
            temp.transform.position = positionToPlace;
        }

        positionToPlace = Vector3Int.zero;
        from = Vector3Int.zero;
        to = Vector3Int.zero;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("프리팹 불러오기"))
        {
            LoadPrefabsDatas();
        }

        if(GUILayout.Button("생성하기"))
        {
            CreatePrefab();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        positionToPlace = EditorGUILayout.Vector3IntField("설치할 곳", positionToPlace);

        if (selectedPrefabName != null)
        {
            GUILayout.Label("선택된 오브젝트 :" +  selectedPrefabName);
        }

        GUILayout.EndHorizontal();

        isFill = EditorGUILayout.BeginToggleGroup("반복", isFill);
        if (isFill)
        {
            from = EditorGUILayout.Vector3IntField("시작지점", from);
            to = EditorGUILayout.Vector3IntField("끝나는 지점", to);
        }

        EditorGUILayout.EndToggleGroup();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical();

        foreach(var prefab in prefabs)
        {
            if(GUILayout.Button(prefab.Key))
            {
                SelectPrefab(prefab.Key);
            }
        }
    }

}

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
                            var temp = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[selectedPrefabName]);
                            Undo.RegisterCreatedObjectUndo(temp, "Instantiate Prefab");
                            Selection.activeGameObject = temp;
                            temp.transform.position = new Vector3(i, j, k);
                            temp.transform.rotation = Quaternion.identity;
                        }
                    }
                }
                for (int k = from.z; k <= to.z; k++)
                {
                    var temp = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[selectedPrefabName]);
                    Undo.RegisterCreatedObjectUndo(temp, "Instantiate Prefab");
                    Selection.activeGameObject = temp;
                    var position = new Vector3(i, 0, k);
                    temp.transform.position = new Vector3(i, 0, k);
                    temp.transform.rotation = Quaternion.identity;
                }
            }
        }
        else
        {
            var temp = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[selectedPrefabName]);
            Undo.RegisterCreatedObjectUndo(temp, "Instantiate Prefab");
            Selection.activeGameObject = temp;
            temp.transform.position = positionToPlace;
        }

        positionToPlace = Vector3Int.zero;
        from = Vector3Int.zero;
        to = Vector3Int.zero;
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        // 상단 버튼 영역
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("프리팹 불러오기"))
        {
            LoadPrefabsDatas();
        }

        if (GUILayout.Button("생성하기"))
        {
            CreatePrefab();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 중앙 정보 입력 영역
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        positionToPlace = EditorGUILayout.Vector3IntField("설치할 곳", positionToPlace);

        if (selectedPrefabName != null)
        {
            GUILayout.Label("선택된 오브젝트 :" + selectedPrefabName);
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

        // 하단 프리팹 선택 버튼 영역
        GUILayout.BeginVertical();

        foreach (var prefab in prefabs)
        {
            if (GUILayout.Button(prefab.Key))
            {
                SelectPrefab(prefab.Key);
            }
        }

        GUILayout.EndVertical();
    }
    public void OnSceneGUI(SceneView sceneView)
    {
        if(positionToPlace != null && !isFill)
        {
            Handles.color = Color.green;

            Handles.DrawWireCube(new Vector3(positionToPlace.x + 0.5f, positionToPlace.y - 1, positionToPlace.z + 0.5f), Vector3.one);
        }
        else if(from != null && to != null)
        {
            Vector3 average = (from + to) / 2;
            float width = Mathf.Abs(from.x - to.x + 1);
            float height = Mathf.Abs(from.y - to.y + 1);
            float depth = Mathf.Abs(from.z - to.z + 1);
            Handles.color = Color.blue;

            Handles.DrawWireCube(new Vector3(average.x + 0.5f, average.y - 1, average.z + 0.5f), new Vector3(width, height, depth));
        }
    }
}

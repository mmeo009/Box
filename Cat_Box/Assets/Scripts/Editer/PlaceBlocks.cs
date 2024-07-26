using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class PlaceBlocks : EditorWindow
{
    public List<GameObject> prefabs = new List<GameObject>();

    public Vector3 positionToPlace = Vector3.zero;

    public int repetition_X = 0;
    public int repetition_Y = 0;


    [MenuItem("CatBox/Prop Generator")]
    private static void ShwoWindow()
    {
        var window = GetWindow<PlaceBlocks>();
        window.titleContent = new GUIContent("Prop Generator");
        window.Show();
    }

    private void OnGUI()
    {
        prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath("Prefabs/Map")));

        for (int i = 0; i < prefabs.Count; i ++)
        {
            
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelExpose : MonoBehaviour
{
    public List<ExposePoint> points;
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelExpose))]
public class LevelExposeEditor : Editor
{
    public GameObject exposePrefab;

    public void OnEnable()
    {
        if (exposePrefab == null) {
            exposePrefab = Resources.Load("Prefabs/Other/ExposePoint") as GameObject;
        }
    }

    public override void OnInspectorGUI()
    {
        LevelExpose lvl = (LevelExpose)target;
        if (exposePrefab == null)
        {
            exposePrefab = Resources.Load("Prefabs/Other/ExposePoint") as GameObject;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Expose Point")) {
            GameObject o = Instantiate(exposePrefab, lvl.transform.position, Quaternion.identity, lvl.transform);
            lvl.points.Add(o.GetComponent<ExposePoint>());
        }
        if (GUILayout.Button("Clear Expose Points"))
        {
            while (lvl.points.Count > 0) {
                DestroyImmediate(lvl.points[0]);
                lvl.points.RemoveAt(0);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < lvl.points.Count; i++) {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
            GUILayout.Label("Point " + (i+1));
            if (GUILayout.Button("Select Point", GUILayout.Width(50))) {
                
            }
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("^", GUILayout.Width(20))) {
                
            }
            if (GUILayout.Button("v", GUILayout.Width(20))) {
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();
    }
}
#endif
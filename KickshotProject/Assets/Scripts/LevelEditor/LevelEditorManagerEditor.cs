using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
    [CustomEditor(typeof(LevelEditorManager))]
    public class LevelEditorManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //GUILayout.TextField(LevelEditorManager.objectTag);
            //EditorGUILayout.ObjectField(LevelEditorManager.activeSelection, typeof(GameObject));

            if (GUILayout.Button("Export Level"))
            {
                LevelEditorManager.Instance.Export();
            }
        }

        [MenuItem("Tools/Level Editor/Export")]
        public static void Export() {
            LevelEditorManager.Instance.Export();
        }
    }
}
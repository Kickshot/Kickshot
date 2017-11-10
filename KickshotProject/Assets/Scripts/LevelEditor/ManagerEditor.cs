using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Manager))]
    public class ManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //GUILayout.TextField(LevelEditorManager.objectTag);
            //EditorGUILayout.ObjectField(LevelEditorManager.activeSelection, typeof(GameObject));

            if (GUILayout.Button("Export Level"))
            {
                Manager.Instance.Export();
            }
        }

        [MenuItem("Tools/Level Editor/Export")]
        public static void Export()
        {
            Manager.Instance.Export();
        }
    }
#endif
}
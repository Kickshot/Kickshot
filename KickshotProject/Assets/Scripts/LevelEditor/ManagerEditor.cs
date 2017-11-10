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

            if (GUILayout.Button("Export Level"))
            {
                Manager.Instance.Export();
            }
        }

        [MenuItem("Tools/Level Editor/Export Level")]
        public static void Export()
        {
            Manager.Instance.Export();
        }
    }
#endif
}
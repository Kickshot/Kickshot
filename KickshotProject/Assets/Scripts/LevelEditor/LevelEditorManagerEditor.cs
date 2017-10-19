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

            LevelEditorManager lvlEManager = (LevelEditorManager)target;
            if (GUILayout.Button("Export Level"))
            {
                lvlEManager.Export();
            }
        }
    }
}
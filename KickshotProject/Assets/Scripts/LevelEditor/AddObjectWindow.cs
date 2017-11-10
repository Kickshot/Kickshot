using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
    public class AddObjectWindow : EditorWindow
    {
        GameObject prefab;
        static EditorWindow window;

        [MenuItem("Tools/Level Editor/Resources/Add Object")]
        public static void AddObject()
        {
            window = EditorWindow.GetWindow(typeof(AddObjectWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("Add Level Object", EditorStyles.boldLabel);
            prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Export"))
            {
                if (prefab == null)
                {
                    if (!EditorUtility.DisplayDialog("LevelObjectDatabase", "You need to specify which object to add!", "Okay", "Cancel")) {
                        window.Close();
                    }
                }
                else
                {
                    Debug.Log(string.Format("Adding {0} to database", prefab.name));
                    LevelObjectDatabase.Instance.AddLevelObject(AssetDatabase.GetAssetPath(prefab));
                    EditorUtility.DisplayDialog("LevelObjectDatabase", "Added to database!", "Okay", "Cancel");
                    window.Close();
                }

            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour
{

    [Header("Settings")]
    public int buffer = 100;
    [Header("Levels")]
    public List<Level> levels = new List<Level>();
    [Header("References")]
    public GameObject levelButtonPrefab;

    public void ButtonPressed(Level lvl) {
        Debug.Log(lvl.levelName);
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(LevelPanel))]
public class LevelPanelEditor : Editor
{
    private LevelPanel lp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        lp = (LevelPanel)target;

        if (GUILayout.Button("Update"))
        {
            UpdateLevelPanel();
        }

        DrawDefaultInspector();
    }

    //Editor Helper
    public void UpdateLevelPanel()
    {
        //Remove Children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform c in lp.transform)
        {
            children.Add(c.gameObject);
        }
        while (children.Count > 0)
        {
            GameObject child = children[0];
            children.RemoveAt(0);
            DestroyImmediate(child);
        }

        //Add in new children
        if (lp.levels.Count == 0)
        {
            Debug.LogError("No Levels in LevelPanel!");
            return;
        }

        int totalWidth = 0;
        RectTransform levelRect = lp.levelButtonPrefab.GetComponent<RectTransform>();
        if (levelRect == null)
        {
            Debug.LogError("Failed to get level prefab RectTransform");
            return;
        }

        int levelWidth = Mathf.CeilToInt(levelRect.rect.width);
        totalWidth += lp.levels.Count * levelWidth;
        totalWidth += lp.buffer * (lp.levels.Count - 1);

        for (int i = 0; i < lp.levels.Count; i++)
        {
            //Place each level button
            GameObject o = Instantiate(lp.levelButtonPrefab, Vector3.zero, Quaternion.identity, lp.transform);
            o.name = lp.levels[i].levelName;
            RectTransform trans = o.GetComponent<RectTransform>();
            int left = -Mathf.FloorToInt(totalWidth * 0.5f);
            left += Mathf.FloorToInt(levelWidth * 0.5f);
            left += Mathf.FloorToInt(i * levelWidth);
            left += i * lp.buffer;
            trans.anchoredPosition = new Vector2(left, 0);
            Text text = o.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = lp.levels[i].levelName;
            }
            Button lvlButton = o.GetComponent<Button>();
            LevelObject lvlObj = o.GetComponent<LevelObject>();
            lvlObj.level = lp.levels[i];
            lvlButton.onClick.AddListener(() => { lvlObj.LoadLevel(); });
        }
    }
}
#endif

[System.Serializable]
public class Level
{
    [Tooltip("The name of the actual scene")]
    public string sceneName;
    [Tooltip("The name to be displayed")]
    public string levelName;
}
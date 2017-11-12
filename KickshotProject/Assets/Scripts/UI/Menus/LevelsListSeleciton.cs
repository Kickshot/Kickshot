using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelsListSeleciton : MonoBehaviour {

    [SerializeField]
    private ToggleGroup levels;

    [SerializeField]
    private string[] levelNames;

    [SerializeField]
    private GameObject listEntryTemplate;

    [SerializeField]
    private Image screenShotDisplay;

    [Serializable]
    public struct LevelScreen{
        public string levelName;
        public Sprite screen;
    }
    [SerializeField]
    private LevelScreen[] levelScreens;

    private Dictionary<string, Sprite> levelScreensDict = new Dictionary<string, Sprite>();

    void Start(){
        foreach(LevelScreen ls in levelScreens){
            levelScreensDict.Add(ls.levelName, ls.screen);
        }

        foreach (string levelName in levelNames){
            GameObject listEntry = Instantiate(listEntryTemplate) as GameObject;
            listEntry.SetActive(true);
            listEntry.transform.Find("Label").GetComponent<Text>().text = levelName;
            listEntry.transform.SetParent(listEntryTemplate.transform.parent, false);
            listEntry.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value){
                    UpdateLevelSelectScreenShot(levelName);
                }
            });
        }
        // Below is for adding fake list items to test the scroll bar
        for(int i = 0; i < 20; i++){
            GameObject listEntry = Instantiate(listEntryTemplate) as GameObject;
            listEntry.SetActive(true);
            listEntry.transform.Find("Label").GetComponent<Text>().text = "Test Level Don't Select " + i;
            listEntry.transform.SetParent(listEntryTemplate.transform.parent, false);
        }
    }
    public void LoadSelectedLevel(){
        string levelName = "";
        //Although this returns "all" toggles, we know we will always have only 1.
        //Since ActiveToggles returns an IEnumerable, we have to iterate over it. 
        foreach(Toggle level in levels.ActiveToggles()){
            levelName = level.transform.Find("Label").GetComponent<Text>().text;
        }
        if (!levelName.Equals("")) {
            // Currently unsafe. Will crash with any typos in the scene name, including trailing whitespace
            SceneManager.LoadScene(levelName);
        }
    }

    private void UpdateLevelSelectScreenShot(string levelName){
        if (levelScreensDict.ContainsKey(levelName))
            screenShotDisplay.sprite = levelScreensDict[levelName];
        else
            screenShotDisplay.sprite = levelScreensDict["default"];
    }
}

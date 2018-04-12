using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    [Header ("Settings")]
    public float minDuration;
    [Header ("References")]
    public Slider progressBar;

    [HideInInspector]
    public string sceneName;

	public void Start()
	{
        Debug.Assert(progressBar != null);
        Debug.Assert(sceneName != null);

        //Check for load message
        GameObject loadReq = GameObject.FindWithTag("LOAD");
        if (loadReq != null) {
            LoadInfo info = loadReq.GetComponent<LoadInfo>();
            Debug.Log(info);
            sceneName = info != null ? info.sceneName : "MainMenu";
        } else {
            Debug.Log("Failed to find LOAD");
        }
        DestroyImmediate(loadReq);

        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        float timer = 0f;

        //Min wait
        while (timer < minDuration) {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Start loading
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        //Update progress bar
        while (!load.isDone)
        {
            progressBar.value = load.progress * progressBar.maxValue;
            yield return new WaitForEndOfFrame();
        }

        //Set active to level
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        //Unload loading scene
        //SceneManager.UnloadSceneAsync("Loading");

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObject : MonoBehaviour
{
    public Level level;

    public void LoadLevel()
    {
        Debug.Log("Loading Level");

        SceneManager.LoadScene(level.sceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButtonManager : MonoBehaviour {

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}

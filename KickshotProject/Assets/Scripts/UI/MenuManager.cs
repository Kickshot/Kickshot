using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public enum Menus
    {
        Main,
        WorldSelect,
        IslandsWorld,
        DesertWorld
    }

    [Header("General Settings")]
    public float lerpDuration;

    [Header("Camera Positions")]
    public Transform mainMenuView;
    public Transform worldSelectView;
    public Transform islandsView;
    public Transform desertView;

    [Header("Position Fog Colors")]
    public Color islandsFogColor;
    public Color desertFogColor;
    public Color defaultFogColor;

    [Header("Element References")]
    public Text buildLabel;

    private GameObject mainCamera;

    private void Start()
    {
        buildLabel.text = Application.version;
        Camera c = Camera.main;
        Debug.Assert(c != null, "Failed to find scene camera");
        mainCamera = c.gameObject;

        Camera.main.backgroundColor = defaultFogColor;
        RenderSettings.fogColor = defaultFogColor;
    }

    /// <summary>
    /// Uses scenemanagement to load a given scene. Needs to be passed the proper
    /// scene name or will fail.
    /// </summary>
    /// <param name="sceneName">Scene name</param>
    public void LoadLevel(string sceneName) {
        //TODO Add scene validity check
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Will lerp to a given menu transform, use Menus enum as number reference.
    /// </summary>
    /// <param name="menu">Menu integer (Menus enum)</param>
    public void ChangeMenu(int menu) {
        Transform target = mainMenuView;
        Color cTarget = defaultFogColor;

        Menus m = (Menus)menu;
        switch (m) {
            case Menus.Main:
                target = mainMenuView;
                break;
            case Menus.WorldSelect:
                target = worldSelectView;
                break;
            case Menus.IslandsWorld:
                target = islandsView;
                cTarget = islandsFogColor;
                break;
            case Menus.DesertWorld:
                target = desertView;
                cTarget = desertFogColor;
                break;
        }

        StopCoroutine("GoToMenu");
        StartCoroutine(GoToMenu(target));

        StopCoroutine("LerpFogColor");
        StartCoroutine(LerpBackColor(cTarget, 0.5f));
    }

    /// <summary>
    /// Quit game or stop playing if in the editor.
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Lerps position and rotation of the camera to a given target transform.
    /// </summary>
    /// <param name="target">Target transform</param>
    private IEnumerator GoToMenu(Transform target) {
        Vector3 camPos = mainCamera.transform.position;
        Quaternion camRot = mainCamera.transform.rotation;
        float curDur = 0f;

        while (curDur < lerpDuration) {
            curDur += Time.deltaTime;
            float percent = Mathf.Clamp01(curDur / lerpDuration);
            camPos = Vector3.Lerp(camPos, target.position, percent);
            camRot = Quaternion.Lerp(camRot, target.rotation, percent);
            mainCamera.transform.position = camPos;
            mainCamera.transform.rotation = camRot;
            yield return null;
        }
    }

    /// <summary>
    /// Lerps the color of the background camera color and fog color.
    /// </summary>
    /// <param name="target">Target color.</param>
    /// <param name="duration">Lerp duration.</param>
    private IEnumerator LerpBackColor(Color target, float duration) {
        float curDur = 0f;
        Color src = RenderSettings.fogColor;

        while (curDur < duration) {
            curDur += Time.deltaTime;
            float percent = Mathf.Clamp01(curDur / duration);
            src = Color.Lerp(src, target, percent);
            RenderSettings.fogColor = src;
            Camera.main.backgroundColor = src;
            yield return null;
        }
    }
}

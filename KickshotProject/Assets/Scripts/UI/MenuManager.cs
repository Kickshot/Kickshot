using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct MenuStruct {
    public string name;
    public int id;
    public Transform camPos;
    public Material skybox;
    public Color fogColor;
}

public class MenuManager : MonoBehaviour
{
    [Header("General Settings")]
    public float lerpDuration;
    public EasingFunction.Ease easeFunction; 

    public List<MenuStruct> menus = new List<MenuStruct>();

    [Header("Element References")]
    public Text buildLabel;
    public GameObject loadInfo;

    private GameObject mainCamera;

    private void Start()
    {
        buildLabel.text = Application.version;
        Camera c = Camera.main;
        Debug.Assert(c != null, "Failed to find scene camera");
        mainCamera = c.gameObject;

        Camera.main.backgroundColor = menus[0].fogColor;
        RenderSettings.fogColor = menus[0].fogColor;
    }

    /// <summary>
    /// Uses scenemanagement to load a given scene. Needs to be passed the proper
    /// scene name or will fail.
    /// </summary>
    /// <param name="sceneName">Scene name</param>
    public void LoadLevel(string sceneName) {
        //TODO Add scene validity check
        Debug.Log(sceneName);
        GameObject loadInstance = Instantiate(loadInfo) as GameObject;
        loadInstance.GetComponent<LoadInfo>().sceneName = sceneName;
        DontDestroyOnLoad(loadInstance);
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// Will lerp to a given menu transform, use Menus enum as number reference.
    /// </summary>
    /// <param name="menu">Menu integer (Menus enum)</param>
    public void ChangeMenu(int menuID) {
        MenuStruct targetMenu = menus[0];
        for (int i = 0; i < menus.Count; i++)
        {
            if (menuID == menus[i].id)
            {
                targetMenu = menus[i];
                break;
            }
        }

        StopCoroutine("GoToMenu");
        StartCoroutine(GoToMenu(targetMenu.camPos));

        StopCoroutine("LerpFogColor");
        StartCoroutine(LerpBackColor(targetMenu.fogColor, 0.5f));

        StopCoroutine("LerpSkybox");
        StartCoroutine(LerpSkybox(targetMenu.skybox));
    }

    /// <summary>
    /// Will lerp to a given menu transform, use Menus enum as number reference.
    /// </summary>
    /// <param name="menu">Menu integer (Menus enum)</param>
    public void ChangeMenu(string menuName)
    {
        MenuStruct targetMenu = menus[0];
        for (int i = 0; i < menus.Count; i++)
        {
            if (menuName == menus[i].name)
            {
                targetMenu = menus[i];
                break;
            }
        }

        StopCoroutine("GoToMenu");
        StartCoroutine(GoToMenu(targetMenu.camPos));

        StopCoroutine("LerpFogColor");
        StartCoroutine(LerpBackColor(targetMenu.fogColor, 0.5f));

        StopCoroutine("LerpSkybox");
        StartCoroutine(LerpSkybox(targetMenu.skybox));
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

    private IEnumerator LerpSkybox(Material targetSkybox) {
        Material curSkybox = RenderSettings.skybox;

        float sunSize = curSkybox.GetFloat("_SunSize");
        float sunConvergence = curSkybox.GetFloat("_SunSizeConvergence");
        float atmosphereThickness = curSkybox.GetFloat("_AtmosphereThickness");
        Color skyTint = curSkybox.GetColor("_SkyTint");
        Color groundColor = curSkybox.GetColor("_GroundColor");
        float exposure = curSkybox.GetFloat("_Exposure");

        float tSunSize = targetSkybox.GetFloat("_SunSize");
        float tSunConvergence = targetSkybox.GetFloat("_SunSizeConvergence");
        float tAtmosphereThickness = targetSkybox.GetFloat("_AtmosphereThickness");
        Color tSkyTint = targetSkybox.GetColor("_SkyTint");
        Color tGroundColor = targetSkybox.GetColor("_GroundColor");
        float tExposure = targetSkybox.GetFloat("_Exposure");

        float curDur = 0f;

        while (curDur < lerpDuration) {
            curDur += Time.deltaTime;
            float percent = Mathf.Clamp01(curDur / lerpDuration);

            curSkybox.SetFloat("_SunSize", Mathf.Lerp(sunSize, tSunSize, percent));
            curSkybox.SetFloat("_SunSizeConvergence", Mathf.Lerp(sunConvergence, tSunConvergence, percent));
            curSkybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(atmosphereThickness, tAtmosphereThickness, percent));
            curSkybox.SetColor("_SkyTint", Color.Lerp(skyTint, tSkyTint, percent));
            curSkybox.SetColor("_GroundColor", Color.Lerp(groundColor, tGroundColor, percent));
            curSkybox.SetFloat("_Exposure", Mathf.Lerp(exposure, tExposure, percent));

            RenderSettings.skybox = curSkybox;
            yield return null;
        }
    }

    /// <summary>
    /// Lerps position and rotation of the camera to a given target transform.
    /// </summary>
    /// <param name="target">Target transform</param>
    private IEnumerator GoToMenu(Transform target) {
        Vector3 camPos = mainCamera.transform.position;
        Quaternion camRot = mainCamera.transform.rotation;
        float curDur = 0f;

        EasingFunction.Function Ease = EasingFunction.GetEasingFunction(easeFunction);

        while (curDur < lerpDuration) {
            curDur += Time.deltaTime;
            float percent = Mathf.Clamp01(curDur / lerpDuration);
            mainCamera.transform.position = Vector3.Lerp(camPos, target.position, Ease(0f,1f,percent));
            mainCamera.transform.rotation = Quaternion.Lerp(camRot, target.rotation, Ease(0f, 1f, percent));
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

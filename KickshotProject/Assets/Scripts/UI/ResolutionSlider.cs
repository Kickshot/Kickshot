using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour {
    
    private Resolution[] res;
    private Slider resSlider;
    private Text resText;
    Resolution selected;
    public GameObject OptionsManager;
    private OptionsManager Options;
    private void Start() {

        Options = OptionsManager.GetComponent<OptionsManager>();
        resSlider = GetComponent<Slider>();
        if (resSlider == null)
            Debug.LogError("Failed to find required slider component");

        resText = GetComponentInChildren<Text>();
        if (resText == null)
            Debug.LogError("Failed to find required text component");

        UpdateSlider();
        InvokeRepeating("CheckResolution", 2f, 2f);
    }

    public void ValueChanged() {
        int index = Mathf.RoundToInt(resSlider.value);
        if (index >= res.Length) {
            Debug.LogError("Something is fucky");
            return;
        }
        selected = res[index];
        UpdateResText(selected);
    }

    private void UpdateResText(Resolution r) {
        resText.text = "resolution : " + r.width + " x " + r.height + " px";
    }

    private void UpdateSlider() {
        res = Screen.resolutions;
        resSlider.wholeNumbers = true;
        resSlider.minValue = 0;
        resSlider.maxValue = res.Length-1;
        Resolution curRes = Screen.currentResolution;
        int i;
        for (i = 0; i < res.Length; i++)
        {
            if (ResEqual(res[i], curRes))
                break;
        }
        resSlider.value = i;
    }

    private bool ResEqual(Resolution a, Resolution b) {
        if (a.width == b.width && a.height == b.height && a.refreshRate == b.refreshRate)
            return true;
        return false;
    }

    private void CheckResolution() {
        bool resChanged = false;
        Resolution[] newRes = Screen.resolutions;
        for (int i = 0; i < newRes.Length; i++)
        {
            if (!ResEqual(newRes[i], res[i])) {
                resChanged = true;
                break;
            }
        }
        if (resChanged) {
            UpdateSlider();
        }
    }

    public void ApplyRes()
    {

        Options.setScreenSize(selected);

        
    }
}


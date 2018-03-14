using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSlider : MonoBehaviour {

    public GlobalConstant variable;
    public string prefix;
    public string suffix;

    private Slider slider;
    private Text text;
    private OptionsManager manager;
    public GameObject OptionsManager;
    private void Start()
    {

        manager = OptionsManager.GetComponent<OptionsManager>();

  
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<Text>();
        if (slider == null || text == null)
            Debug.LogError("Failed to find required components!");

        switch (variable) {
            case GlobalConstant.FOV:
                slider.value = manager.getFov();
                break;
            case GlobalConstant.Sensitivity:
                slider.value = manager.getSensitivity();
                break;
            case GlobalConstant.SFXVolume:
                slider.value = manager.getSFXVolume();
                break;
            case GlobalConstant.MusicVolume:
                slider.value = manager.getMusicVolume();
                break;
        }
    }

    public void ValueChanged() {
        int value = Mathf.RoundToInt(slider.value);
        switch (variable)
        {
            case GlobalConstant.FOV:
                GlobalConstants.FOV = value;
                manager.setFov(value);
                break;
            case GlobalConstant.Sensitivity:
                GlobalConstants.Sensitivity = value;
                manager.setSensitivity(value);
                break;
            case GlobalConstant.SFXVolume:
                manager.setSFXVolume(value);
                GlobalConstants.SFXVolume = value;
                break;
            case GlobalConstant.MusicVolume:
                manager.setMusicVolume(value);
                GlobalConstants.MusicVolume = value;
                break;
        }
        text.text = prefix + value + suffix;
    }
}
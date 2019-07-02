using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolume, ambientNoise, ambientEffect;

    [Header("Res")]
    public Dropdown dropDownRes;
    Resolution[] resolutions;

    [Header("Other")]
    public GameObject escHolder;

    void Start()
    {
        escHolder.SetActive(false);
        masterVolume.value = PlayerPrefs.GetFloat("MasterVolumeMix", 0);
        ambientNoise.value = PlayerPrefs.GetFloat("AmbientNoise", 4);
        ambientEffect.value = PlayerPrefs.GetFloat("AmbientEffect", 0);

        resolutions = Screen.resolutions;

        dropDownRes.ClearOptions();
        List<string> vs = new List<string>();
        int curIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string options = resolutions[i].width + " X " + resolutions[i].height;
            vs.Add(options);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                curIndex = i;
            }
        }
        dropDownRes.AddOptions(vs);
        dropDownRes.value = curIndex;
        dropDownRes.RefreshShownValue();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            escHolder.SetActive(!escHolder.activeSelf);
        }
    }

    public void SwitchRes(int witch)
    {
        Resolution resol = resolutions[witch];
        Screen.SetResolution(resol.width, resol.height, Screen.fullScreen);
    }

    public void FullScreenToggle(bool tog)
    {
        Screen.fullScreen = tog;
    }

    public void CoppleMasterVolume(float amount)
    {
        audioMixer.SetFloat("MasterVolumeMix", amount);
    }

    public void CoppleAmbientNoiseVolume(float amount)
    {
        audioMixer.SetFloat("AmbientNoise", amount);
    }

    public void CoppleAmbientEffectVolume(float amount)
    {
        audioMixer.SetFloat("AmbientEffect", amount);
    }

    public void SetGraphics(int dropDown)
    {
        QualitySettings.SetQualityLevel(dropDown, true);
    }

    public void CloseOptions()
    {
        escHolder.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("DIKKE KABAB NEGER");
        Application.Quit();
    }
}
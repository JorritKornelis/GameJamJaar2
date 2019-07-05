using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolume;

    [Header("Other")]
    public GameObject escHolder;

    void Start()
    {
        escHolder.SetActive(false);

        masterVolume.value = PlayerPrefs.GetFloat("MasterVolumeMix", 0);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            escHolder.SetActive(!escHolder.activeSelf);
        }
    }

    public void FullScreenToggle(bool tog)
    {
        Screen.fullScreen = tog;
    }

    public void CoppleMasterVolume(float amount)
    {
        audioMixer.SetFloat("MasterVolumeMix", amount);
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
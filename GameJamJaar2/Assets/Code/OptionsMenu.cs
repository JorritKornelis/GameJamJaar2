using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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

        masterVolume.value = PlayerPrefs.GetFloat("MasterVolumeMix", -10);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            escHolder.SetActive(!escHolder.activeSelf);

        }
        if (Time.timeScale == 1 && escHolder.activeSelf == true)
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale == 0 && escHolder.activeSelf == false)
        {
            Time.timeScale = 1f;
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

    public void BackMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
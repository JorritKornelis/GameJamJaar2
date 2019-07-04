using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneSelecter(string loadScene)
    {
        SceneManager.LoadScene(loadScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
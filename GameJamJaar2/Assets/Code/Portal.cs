using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public ParticleSystem portalPar;
    public string sceneSwitchName;

    void PlayThePartical()
    {
        portalPar.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && portalPar.isEmitting)
        {
            SceneManager.LoadScene(sceneSwitchName);
        }
    }


}

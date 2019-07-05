using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    public AudioSource audioSource;
    public abstract void Damage();

    public abstract IEnumerator CheckAttack();

    public IEnumerator SlowTime()
    {
        while (Time.timeScale >= 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.03f);
            yield return null;
        }
        Time.timeScale = 0f;
    }

    public void PlayAudioClip(AudioClip clip)
    {
        audioSource.pitch = 1;
        audioSource.pitch += Random.Range(-0.05f, 0.05f);
        audioSource.PlayOneShot(clip);
    }
}

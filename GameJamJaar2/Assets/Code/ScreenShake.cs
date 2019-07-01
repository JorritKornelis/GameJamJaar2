using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Range(0,1)]
    public float returnSpeed;
    public int rounds;
    public float roundDelay;

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.parent.position, returnSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.parent.rotation,returnSpeed);
    }

    public IEnumerator Shake(float intensity)
    {
        int round = rounds;
        while(round != 0)
        {
            transform.Translate(new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)));
            transform.Rotate(new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)));
            round--;
            yield return new WaitForSeconds(roundDelay);
        }
    }

    public IEnumerator Shake(float intensity, float time)
    {
        while(time > 0)
        {
            transform.Translate(new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)));
            transform.Rotate(new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)));
            time -= Time.deltaTime * 1.3f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}

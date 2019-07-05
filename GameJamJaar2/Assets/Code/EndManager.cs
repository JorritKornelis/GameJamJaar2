using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    public Portal portal;
    public GameObject wall;

    public IEnumerator MoveWallDown()
    {
        float i = 3f;
        while(i > 0)
        {
            wall.transform.Translate(Vector3.up * -3 * Time.deltaTime);
            i -= Time.deltaTime;
            yield return null;
        }
    }

    public void Won()
    {
        portal.portalPar.gameObject.SetActive(true);
        StartCoroutine(MoveWallDown());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public GameObject focusPoint;
    public float movSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Focus();
    }

    public void Focus()
    {
        transform.LookAt(focusPoint.transform);

        transform.Translate(movSpeed * Time.deltaTime, 0, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerControler : MonoBehaviour
{
    Vector3 moveVector;
    public float moveSpeed;
    public bool mayMoveBool = true;
    public float dashAmount;
    public float dashTime;

    [Header("Death")]
    public GameObject vidHolder;

    private void Start()
    {
        vidHolder.SetActive(false);
    }

    void FixedUpdate()
    {
        if (mayMoveBool == true)
        {
            PlayerMove();
        }
        LookToMouse();
        StartDash();
    }

    void PlayerMove()
    {
        moveVector.x = -Input.GetAxis("Horizontal");
        moveVector.z = -Input.GetAxis("Vertical");
        transform.Translate(moveVector * Time.deltaTime * moveSpeed, Space.World);
    }

    void LookToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        float currentTime = dashTime;
        mayMoveBool = false;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            transform.Translate(Vector3.forward * dashAmount * Time.deltaTime);
            yield return null;
        }
        mayMoveBool = true;
    }

    void PlayerDeath()
    {
        vidHolder.SetActive(true);
        vidHolder.GetComponent<VideoPlayer>().Play();
    }
}

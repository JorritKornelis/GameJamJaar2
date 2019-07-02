using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerControler : MonoBehaviour
{
    Vector3 moveVector;
    public float moveSpeed;
    public bool mayMoveBool = true;
    Vector3 dashDir;

    [Header("Death")]
    public GameObject vidHolder;

    [Header("Dash")]
    public float dashAmount;
    public float dashTime;
    public float timerCoolDown;
    public float timerCoolDownReset;
    public bool mayDash = true;
    bool b = false;

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
        if (b)
        {
            RestetBool();
        }
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
            if (Input.GetAxis("Horizontal") >0 && mayDash)
            {
                dashDir = new Vector3(1, 0, 0);
                StartCoroutine(Dash());
                Debug.Log(dashDir);
            }
            else if (Input.GetAxis("Horizontal") < 0 && mayDash)
            {
                dashDir = new Vector3(-1, 0, 0);
                StartCoroutine(Dash());
                Debug.Log(dashDir);
            }
            else if (Input.GetAxis("Vertical") > 0 && mayDash)
            {
                dashDir = new Vector3(0, 0, 1);
                StartCoroutine(Dash());
                Debug.Log(dashDir);
            }
            else if (Input.GetAxis("Vertical") < 0 && mayDash)
            {
                dashDir = new Vector3(0, 0, -1);
                StartCoroutine(Dash());
                Debug.Log(dashDir);
            }
        }
    }

    IEnumerator Dash()
    {
        float currentTime = dashTime;
        mayMoveBool = false;
        mayDash = false;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            transform.Translate(dashDir * dashAmount * Time.deltaTime);
            yield return null;
        }
        mayMoveBool = true;
        b = true;
    }

    void RestetBool()
    {
        timerCoolDown -= Time.deltaTime;
        if (timerCoolDown <= 0)
        {
            mayDash = true;
            b = false;
            timerCoolDown = timerCoolDownReset;
        }
    }

    void PlayerDeath()
    {
        vidHolder.SetActive(true);
        vidHolder.GetComponent<VideoPlayer>().Play();
    }
}

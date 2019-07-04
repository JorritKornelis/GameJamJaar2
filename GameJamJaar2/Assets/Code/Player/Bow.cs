﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public GameObject arrowPos;
    public bool mayShootArrow = true;
    public bool gotArrow = true;
    public bool zoekPijl = false;
    public float flySpeed;
    public float flySpeedBack;
    GameObject player;
    public LayerMask collMask;
    float chargeTimer;
    float chargeTimerReset;
    bool b = false;
    GameObject mainCamera;
    public float zoomPosSpeed;
    Vector3 zoomPosOut;
    public float zoomAmount;

    private void Start()
    {
        arrow.GetComponent<Rigidbody>().isKinematic = true;
        player = GameObject.FindWithTag("Player");
        chargeTimerReset = 0f;
        mainCamera = Camera.main.gameObject;
        zoomPosOut = mainCamera.transform.position;
    }

    private void Update()
    {
        RetrieveArrow();
        FireBow();
        
        //get arrow
        if (mayShootArrow == false&& zoekPijl==true && Physics.CheckSphere(player.transform.position, player.GetComponent<SphereCollider>().radius, collMask))
        {
            arrow.GetComponent<Rigidbody>().isKinematic = true;
            arrow.transform.parent = player.transform;

            arrow.transform.position = arrowPos.transform.position;
            arrow.transform.rotation = arrowPos.transform.rotation;
            arrow.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            gotArrow = true;
            zoekPijl = false;
            if (zoekPijl == false)
            {
                StartCoroutine(WaitBetweenShots());
            }
        }

        if (b == true && arrow.transform.position.y <= 0.2f)
        {
            arrow.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (b==false && gotArrow == false)
        {
            arrow.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void FireBow()
    {
        if (Input.GetButton("Fire1") && mayShootArrow == true)
        {
            player.GetComponent<PlayerControler>().mayMoveBool = false;
            if (chargeTimer <= Mathf.Infinity)
            {
                chargeTimer += Time.deltaTime;
                mainCamera.GetComponent<ScreenShake>().enabled = false;
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, mainCamera.transform.parent.position + (player.transform.forward * zoomAmount), 0.5f * Time.deltaTime * zoomPosSpeed);
            }
        }
        else if (Input.GetButtonUp("Fire1") && mayShootArrow == true && chargeTimer >= 0.5f)
        {
            arrow.GetComponent<Rigidbody>().isKinematic = false;
            arrow.transform.parent = null;
            StartCoroutine(WaitBetweenShots());
            chargeTimer = chargeTimerReset;
            mainCamera.GetComponent<ScreenShake>().enabled = true;

            zoekPijl = true;
            gotArrow = false;

            arrow.GetComponent<Rigidbody>().AddForce(-transform.forward * flySpeed * 10f);
            b = true;
            player.GetComponent<PlayerControler>().mayMoveBool = true;
        }
        else
        {
            mainCamera.GetComponent<ScreenShake>().enabled = true;
        }
    }

    IEnumerator WaitBetweenShots()
    {
        yield return new WaitForSeconds(0.3f);
        mayShootArrow = !mayShootArrow;
        Debug.Log(mayShootArrow + " Got Arrow");
    }

    void RetrieveArrow()
    {
        if (Input.GetButton("Fire1") && mayShootArrow == false)
        {
            b = false;
            player.GetComponent<PlayerControler>().mayMoveBool = false;
            player.transform.LookAt(arrow.transform.position);

            Vector3 tempDir = arrow.transform.position - player.transform.position;
            arrow.transform.LookAt(player.transform);
            arrow.GetComponent<Rigidbody>().velocity = -tempDir * flySpeedBack;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            player.GetComponent<PlayerControler>().mayMoveBool = true;
            if (!mayShootArrow)
            {
                b = true;
            }
        }
    }
}
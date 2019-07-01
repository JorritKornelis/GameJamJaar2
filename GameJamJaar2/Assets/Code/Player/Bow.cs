﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public GameObject arrowPos;
    public bool gotArrow = true;
    public float flySpeed;
    float  arrowDis;
    GameObject player;
    public LayerMask collMask;

    private void Start()
    {
        arrow.GetComponent<Rigidbody>().isKinematic = true;
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        RetrieveArrow();
        FireBow();

        if (gotArrow == false && Physics.CheckSphere(player.transform.position, player.GetComponent<SphereCollider>().radius, collMask))
        {
            arrow.transform.position = arrowPos.transform.position;
            arrow.GetComponent<Rigidbody>().isKinematic = true;
            arrow.transform.parent = player.transform;
            StartCoroutine(WaitBetweenShots());
        }
    }

    void FireBow()
    {
        if (Input.GetButtonDown("Fire1") && gotArrow == true)
        {
            arrow.GetComponent<Rigidbody>().isKinematic = false;
            arrow.transform.parent = null;
            StartCoroutine(WaitBetweenShots());

            arrow.GetComponent<Rigidbody>().AddForce(transform.forward * flySpeed * 10f);
        }
    }

    IEnumerator WaitBetweenShots()
    {
        yield return new WaitForSeconds(1);
        gotArrow = !gotArrow;
    }

    void RetrieveArrow()
    {
        if (Input.GetButtonDown("Fire1") && gotArrow == false)
        {
            player.GetComponent<PlayerControler>().mayMoveBool = false;

            Vector3 tempDir = arrow.transform.position - player.transform.position;
            arrow.transform.LookAt(player.transform);
            arrow.GetComponent<Rigidbody>().AddForce(-tempDir * flySpeed);

        }

        if (Input.GetButtonUp("Fire1"))
        {
            player.GetComponent<PlayerControler>().mayMoveBool = true;
        }
    }
}

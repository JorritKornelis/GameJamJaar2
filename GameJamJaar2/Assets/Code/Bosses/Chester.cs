﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chester : BossBase
{
    public string playertag;
    public float moveSpeed;
    public float jumpHeight;
    public float jumpSpeed;
    public float moveCooldown;

    public int coinAmount;
    public GameObject coinObject;
    public float coinFireDelay;
    public Transform weakPoint;
    public float coinSize;
    public LayerMask coinMask;
    List<GameObject> currentCoins = new List<GameObject>();
    public Animator animationController;
    public int maxCoins;

    public void Start()
    {
        StartCoroutine(CheckAttack());
    }

    public override IEnumerator CheckAttack()
    {
        yield return new WaitForSeconds(moveCooldown);
        /*int index = Random.Range(0, 100);
        if (index < 60)
            StartCoroutine(JumpToPlayer());
        else
            StartCoroutine(CoinShotAir());*/
        StartCoroutine(Dash());
    }

    public IEnumerator Dash()
    {
        StartCoroutine(LookAtPlayer());
        yield return new WaitForSeconds(0.2f);
        animationController.SetTrigger("Dash");
        yield return new WaitForSeconds(0.6f);
        GameObject player = GameObject.FindWithTag(playertag);
        float currentY = transform.position.y;
        transform.position = player.transform.position + (Vector3.up * 8f) + (-transform.forward * 2);
        yield return new WaitForSeconds(0.2f);
        while(transform.position.y > currentY)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up * 4, Time.deltaTime * 6);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(CheckAttack());
    }

    public override void Damage()
    {

    }

    public Vector3 GetCointPos(Vector3 centerPos, float range)
    {
        return centerPos + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range)) * 2f;
    }

    public IEnumerator CoinShotAir()
    {
        if (currentCoins.Count <= maxCoins)
        {
            StartCoroutine(LookAtPlayer());
            animationController.SetTrigger("Shotgun");
            yield return new WaitForSeconds(0.3f);
            List<GameObject> coins = new List<GameObject>();
            float range = coinSize;
            for (int i = 0; i < coinAmount; i++)
            {
                GameObject g = Instantiate(coinObject, GetCointPos(GameObject.FindWithTag(playertag).transform.position, range), Quaternion.identity);
                coins.Add(g);
                range += coinSize;
            }

            GameObject[] tempCoins = coins.ToArray();
            System.Array.Reverse(tempCoins);
            coins = new List<GameObject>(tempCoins);

            for (int i = 0; i < coins.Count; i++)
            {
                coins[i].GetComponent<Collider>().enabled = false;
                if (Physics.CheckSphere(coins[i].transform.position, coinSize, coinMask))
                {
                    Destroy(coins[i]);
                    coins.RemoveAt(i);
                    i--;
                }
                else
                    coins[i].GetComponent<Collider>().enabled = true;
            }

            List<Vector3> Positions = new List<Vector3>();
            foreach (GameObject coin in coins)
            {
                Positions.Add(coin.transform.position);
                Destroy(coin);
            }
            foreach (Vector3 point in Positions)
            {
                StartCoroutine(ThrowCoin(Instantiate(coinObject, weakPoint.transform.position, weakPoint.transform.rotation), point));
                yield return new WaitForSeconds(coinFireDelay);
            }

            yield return new WaitForSeconds(0.4f);
            StartCoroutine(CheckAttack());
        }
        else
            StartCoroutine(JumpToPlayer());
    }

    public IEnumerator LookAtPlayer()
    {
        GameObject player = GameObject.FindWithTag(playertag);
        Vector3 lookPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        float i = 0.2f;
        while (i > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position, Vector3.up), Time.deltaTime * 14f);
            i -= Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ThrowCoin(GameObject coin, Vector3 goToPoint)
    {
        currentCoins.Add(coin);
        float height = 0.7f;
        while (true)
        {
            if(height > 0f)
            {
                coin.transform.position = Vector3.Lerp(coin.transform.position, goToPoint + Vector3.up * 20, 3 * Time.deltaTime);
                height -= Time.deltaTime;
            }
            else
                coin.transform.position = Vector3.Lerp(coin.transform.position, goToPoint - Vector3.up * 1, 7 * Time.deltaTime);
            if (Vector3.Distance(coin.transform.position, goToPoint) < 0.02f)
                break;
            
            yield return null;
        }

        yield return new WaitForSeconds(8f);

        for (int i = 0; i < currentCoins.Count; i++)
            if (currentCoins[i] == coin)
            {
                currentCoins.RemoveAt(i);
                Destroy(coin);
                break;
            }
    }

    public IEnumerator JumpToPlayer()
    {
        StartCoroutine(LookAtPlayer());
        yield return new WaitForSeconds(0.1f);
        float currentHeight = jumpSpeed;
        float normalHeight = transform.position.y;
        while(currentHeight > 0)
        {
            if (currentHeight > jumpSpeed / 2f)
                transform.Translate(Vector3.up * jumpHeight * Time.deltaTime);
            else
                transform.Translate(-Vector3.up * jumpHeight * Time.deltaTime);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            currentHeight -= Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, normalHeight, transform.position.z);
        StartCoroutine(CheckAttack());
    }
}

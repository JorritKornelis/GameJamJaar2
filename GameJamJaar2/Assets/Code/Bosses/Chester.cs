using System.Collections;
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
    public float coinFireDelay;

    public void Start()
    {
        StartCoroutine(CheckAttack());
    }

    public override IEnumerator CheckAttack()
    {
        yield return new WaitForSeconds(moveCooldown);
        StartCoroutine(JumpToPlayer());
    }

    public override void Damage()
    {

    }

    public IEnumerator CoinShotAir()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            yield return new WaitForSeconds(coinFireDelay);
        }
    }

    public IEnumerator JumpToPlayer()
    {
        GameObject player = GameObject.FindWithTag(playertag);
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
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

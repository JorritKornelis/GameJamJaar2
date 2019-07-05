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
    public LayerMask playerMask;
    public GameObject coinObject;
    public float coinFireDelay;
    public Transform weakPoint;
    public float coinSize;
    public LayerMask coinMask;
    List<GameObject> currentCoins = new List<GameObject>();
    public Animator animationController;
    public int maxCoins;
    public float contactOffset;
    public Vector3 contactSize;
    public LayerMask arrowMask;
    public float arrowDeathRange;
    public bool damaged;

    public int coinFireRounds;
    public Transform deathCamera;
    public AudioClip fireShotGun, fireOne, jump, impact, open;

    public override void Damage()
    {
        if (!damaged)
            StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        deathCamera.gameObject.SetActive(true);
        deathCamera.transform.rotation = Quaternion.identity;
        damaged = true;
        yield return new WaitForSeconds(0.3f);
        PlayAudioClip(open);
        animationController.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < coinFireRounds; i++)
        {
            StartCoroutine(CoinUp(Instantiate(coinObject, weakPoint.transform.position, weakPoint.transform.rotation)));
            StartCoroutine(CoinUp(Instantiate(coinObject, weakPoint.transform.position, weakPoint.transform.rotation)));
            StartCoroutine(CoinUp(Instantiate(coinObject, weakPoint.transform.position, weakPoint.transform.rotation)));
            yield return new WaitForSeconds(coinFireDelay);
        }
        yield return new WaitForSeconds(0.1f);
        deathCamera.gameObject.SetActive(false);
    }

    public IEnumerator CoinUp(GameObject coin)
    {
        Vector3 offset = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) / 3f;
        coin.transform.Rotate(Random.Range(45, -45), Random.Range(45, -45), Random.Range(45, -45));
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            coin.transform.Translate((Vector3.up + offset) * Time.deltaTime * 20f, Space.World);
            yield return null;
        }
        Destroy(coin);
    }

    public void Start()
    {
        StartCoroutine(CheckAttack());
    }

    public void Update()
    {
        if (Physics.CheckBox(weakPoint.position, contactSize + (weakPoint.transform.up * contactOffset), weakPoint.rotation, playerMask))
        {
            GameObject.FindWithTag(playertag).GetComponent<PlayerControler>().PlayerDeath();
            StartCoroutine(SlowTime());
        }

        if (Physics.CheckSphere(weakPoint.position, arrowDeathRange, arrowMask))
        {
            Damage();
        }
    }

    public override IEnumerator CheckAttack()
    {
        if (!damaged)
        {
            yield return new WaitForSeconds(moveCooldown);
            int index = Random.Range(0, 100);
            if (index < 60)
                if (index < 40)
                    if (index < 30)
                        StartCoroutine(JumpToPlayer());
                    else
                        StartCoroutine(Shot());
                else
                    StartCoroutine(Dash());
            else
                StartCoroutine(CoinShotAir());
        }
    }

    public IEnumerator Shot()
    {
        PlayAudioClip(open);
        StartCoroutine(LookAtPlayer());
        yield return new WaitForSeconds(0.2f);
        animationController.SetTrigger("Shot");
        yield return new WaitForSeconds(0.6f);
        GameObject g = Instantiate(coinObject, weakPoint.position, transform.rotation);
        PlayAudioClip(fireOne);
        StartCoroutine(FiredCoin(g));
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(CheckAttack());
    }

    public IEnumerator FiredCoin(GameObject coin)
    {
        Destroy(coin, 0.6f);
        while (coin != null)
        {
            if (Physics.CheckSphere(coin.transform.position, coinSize, playerMask))
            {
                GameObject.FindWithTag(playertag).GetComponent<PlayerControler>().PlayerDeath();
                StartCoroutine(SlowTime());
            }
            coin.transform.Translate(Vector3.forward * 50 * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator Dash()
    {
        StartCoroutine(LookAtPlayer());
        yield return new WaitForSeconds(0.2f);
        animationController.SetTrigger("Dash");
        yield return new WaitForSeconds(0.6f);
        GameObject player = GameObject.FindWithTag(playertag);
        float currentY = transform.position.y;
        transform.position = player.transform.position + (Vector3.up * 8f) + (-transform.forward);
        PlayAudioClip(open);
        yield return new WaitForSeconds(0.2f);
        while(transform.position.y > currentY)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up * 4, Time.deltaTime * 6);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
        PlayAudioClip(impact);
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(CheckAttack());
    }

    public Vector3 GetCointPos(Vector3 centerPos, float range)
    {
        return centerPos + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }

    public IEnumerator CoinShotAir()
    {
        if (currentCoins.Count <= maxCoins)
        {
            PlayAudioClip(open);
            StartCoroutine(LookAtPlayer());
            animationController.SetTrigger("Shotgun");
            yield return new WaitForSeconds(0.3f);
            List<GameObject> coins = new List<GameObject>();
            float range = coinSize;
            PlayAudioClip(fireShotGun);
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

            yield return new WaitForSeconds(0.55f);
            StartCoroutine(CheckAttack());
        }
        else
            StartCoroutine(JumpToPlayer());
    }

    public IEnumerator LookAtPlayer()
    {
        GameObject player = GameObject.FindWithTag(playertag);
        float i = 0.2f;
        while (i > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position, Vector3.up), Time.deltaTime * 14f);
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
                coin.transform.position = Vector3.Lerp(coin.transform.position, goToPoint, 9 * Time.deltaTime);
            if (Vector3.Distance(coin.transform.position, goToPoint) < 0.05f)
                break;
            if (Physics.CheckSphere(coin.transform.position, coinSize, playerMask))
            {
                GameObject.FindWithTag(playertag).GetComponent<PlayerControler>().PlayerDeath();
                StartCoroutine(SlowTime());
            }
            yield return null;
        }
        yield return new WaitForSeconds(6f);

        for (int i = 0; i < currentCoins.Count; i++)
            if (currentCoins[i].transform.position == coin.transform.position)
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
        PlayAudioClip(jump);
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
        PlayAudioClip(impact);
        transform.position = new Vector3(transform.position.x, normalHeight, transform.position.z);
        StartCoroutine(CheckAttack());
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(weakPoint.position + (weakPoint.transform.up * contactOffset), contactSize);
        if (weakPoint)
            Gizmos.DrawWireSphere(weakPoint.transform.position, arrowDeathRange);
    }
}

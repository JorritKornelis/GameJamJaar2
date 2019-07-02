using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubester : BossBase
{
    public float cubeSize;
    public float moveDelay;

    public float laserTime;
    public GameObject laser;
    [Range(0,100)]
    public int laserChance;

    public Transform eye;
    public float rotateSpeed;
    public string playerTag;
    public float heightChange;
    public float moveSpeed;
    public int laserCount;
    int currentAttackIndex;
    float normalHeight;
    bool up;
    bool activeLaser;
    public float upspeed;
    public GameObject groundImpact;

    [Header("Phase 2")]
    public float phaseMoveDelay;
    public GameObject lava;
    public GameObject lavaPool;
    public bool phase2;
    public float lavaLifetime;
    public GameObject phase2Explosion;
    public float arrowBackSpeed;
    public LayerMask arrowMask;
    public float checkSize;
    public Material rageMaterial;
    public bool canBeDamaged;

    public void Start()
    {
        StartCoroutine(CheckAttack());
        currentAttackIndex = laserCount;
        normalHeight = eye.position.y;
    }

    public void Update()
    {
        if (Physics.CheckBox(eye.position, new Vector3(checkSize, checkSize, 0.1f), eye.rotation, arrowMask) && Physics.CheckSphere(eye.position,checkSize,arrowMask))
            if (!Physics.OverlapSphere(eye.position, checkSize, arrowMask)[0].GetComponent<Rigidbody>().isKinematic)
            {
                Physics.OverlapSphere(eye.position, checkSize, arrowMask)[0].GetComponent<Rigidbody>().AddExplosionForce(arrowBackSpeed, eye.transform.position, arrowBackSpeed);
                Damage();
            }
    }

    public IEnumerator InvincibleFrames()
    {
        yield return new WaitForSeconds(0.5f);
        canBeDamaged = true;
    }

    public override void Damage()
    {
        if (!phase2)
        {
            phase2 = true;
            GetComponent<MeshRenderer>().material = rageMaterial;
            GameObject g = Instantiate(phase2Explosion, transform.position, Quaternion.identity);
            StartCoroutine(InvincibleFrames());
        }
        else if(canBeDamaged)
            Destroy(gameObject);
    }

    public override IEnumerator CheckAttack()
    {
        yield return new WaitForSeconds(phase2? phaseMoveDelay : moveDelay);
        randomAttack();
    }

    public void randomAttack()
    {
        if (currentAttackIndex == 0)
            StartCoroutine(Laser());
        else
            GoToPlayer();
    }

    public IEnumerator Laser()
    {
        activeLaser = true;
        GameObject tempLaser = Instantiate(laser,eye.position,eye.rotation, eye);
        if (eye.position.y < normalHeight)
            StartCoroutine(AirTime(transform.position));
        else
            StartCoroutine(Camera.main.GetComponent<ScreenShake>().Shake(0.2f,laserTime));
        yield return new WaitForSeconds(laserTime / 2);
        if (up)
            Destroy(tempLaser);
        up = false;
        yield return new WaitForSeconds(laserTime / 2);
        if(tempLaser)
            Destroy(tempLaser);
        currentAttackIndex = laserCount;
        activeLaser = false;
        StartCoroutine(CheckAttack());
    }

    public IEnumerator MoveLavaDown(GameObject lavaObject)
    {
        while (lavaObject != null)
        {
            lavaObject.transform.Translate(Vector3.down * Time.deltaTime * 0.1f);
            yield return null;
        }
    }

    public IEnumerator AirTime(Vector3 normalPos)
    {
        up = true;
        StartCoroutine(Camera.main.GetComponent<ScreenShake>().Shake(0.2f, laserTime / 2));
        while (activeLaser)
        {
            if (up)
                transform.Translate(Vector3.up * Time.deltaTime * upspeed * ((Vector3.Distance(transform.position, normalPos) * 0.3f) + 1f), Space.World);
            else
                transform.Translate(-Vector3.up * Time.deltaTime * upspeed * 3f, Space.World);
            yield return null;
        }
        transform.position = normalPos;
        if (phase2)
        {
            GameObject l = Instantiate(lavaPool, transform.position, Quaternion.identity);
            StartCoroutine(MoveLavaDown(l));
            Destroy(l, lavaLifetime);
        }
        StartCoroutine(Camera.main.GetComponent<ScreenShake>().Shake(0.4f));
        GameObject g = Instantiate(groundImpact, transform.position, Quaternion.identity);
        Destroy(g, 2f);
    }

    public void GoToPlayer()
    {
        Transform player = GameObject.FindWithTag(playerTag).transform;
        Vector3 tempDir = transform.position - player.position;
        Vector2Int direction = new Vector2Int();
        if (Mathf.Abs(tempDir.x) > Mathf.Abs(tempDir.z))
            direction.x = (tempDir.x < 0) ? 1 : -1;
        else
            direction.y = (tempDir.z < 0) ? 1 : -1;
        StartCoroutine(Move(direction));
    }

    public IEnumerator Move(Vector2Int direction)
    {
        if (phase2)
        {
            GameObject g = Instantiate(lava, transform.position, Quaternion.identity);
            StartCoroutine(MoveLavaDown(g));
            int i = Random.Range(0, 4);
            g.transform.Rotate(Vector3.up * i * 90);
            Destroy(g,lavaLifetime);
        }
        currentAttackIndex--;
        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;
        float currentRotate = (!phase2 ? 90f : 45f);
        Vector3 rotateDirection = new Vector3();
        if (direction.x == -1)
            rotateDirection = Vector3.forward;
        else if (direction.x == 1)
            rotateDirection = -Vector3.forward;
        if (direction.y == 1)
            rotateDirection = Vector3.right;
        else if (direction.y == -1)
            rotateDirection = -Vector3.right;

        Vector3 moveDirection = new Vector3(direction.x * cubeSize, 0,direction.y * cubeSize);
        while (currentRotate > 0)
        {
            if (currentRotate > (!phase2 ? 45f : 45f / 2f))
                transform.position += Vector3.up * heightChange * Time.deltaTime;
            else
                transform.position += -Vector3.up * heightChange * Time.deltaTime;
            transform.Translate(moveDirection * moveSpeed* Time.deltaTime * (phase2 ? 2 : 1), Space.World);
            currentRotate -= rotateSpeed * Time.deltaTime * (phase2? 2 : 1);
            transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime * (phase2 ? 2 : 1), Space.World);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (phase2)
        {
            GameObject g = Instantiate(lava, Vector3.Lerp(oldPos, oldPos + moveDirection, 0.5f), Quaternion.identity);
            StartCoroutine(MoveLavaDown(g));
            int i = Random.Range(0, 4);
            g.transform.Rotate(Vector3.up * i * 90);
            Destroy(g, lavaLifetime);
        }

        transform.position = oldPos + moveDirection;
        transform.rotation = oldRot;
        transform.Rotate(rotateDirection * 90, Space.World);
        StartCoroutine(Camera.main.GetComponent<ScreenShake>().Shake(0.1f));
        StartCoroutine(CheckAttack());
    }

    public void OnDrawGizmosSelected()
    {
        if(eye)
            Gizmos.DrawWireCube(eye.position,new Vector3(checkSize,checkSize,0.1f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubester : MonoBehaviour
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

    public void Start()
    {
        StartCoroutine(CheckAttack());
        currentAttackIndex = laserCount;
        normalHeight = eye.position.y;
    }

    public IEnumerator CheckAttack()
    {
        yield return new WaitForSeconds(moveDelay);
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

    public IEnumerator AirTime(Vector3 normalPos)
    {
        up = true;
        while (activeLaser)
        {
            if (up)
                transform.Translate(Vector3.up * Time.deltaTime * upspeed * ((Vector3.Distance(transform.position, normalPos) * 0.3f) + 1f), Space.World);
            else
                transform.Translate(-Vector3.up * Time.deltaTime * upspeed * 3f, Space.World);
            yield return null;
        }
        transform.position = normalPos;
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
        currentAttackIndex--;
        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;
        float currentRotate = 90;
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
            if (currentRotate > 45f)
                transform.position += Vector3.up * heightChange * Time.deltaTime;
            else
                transform.position += -Vector3.up * heightChange * Time.deltaTime;
            transform.Translate(moveDirection * moveSpeed* Time.deltaTime, Space.World);
            currentRotate -= rotateSpeed * Time.deltaTime;
            transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime, Space.World);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = oldPos + moveDirection;
        transform.rotation = oldRot;
        transform.Rotate(rotateDirection * 90, Space.World);
        StartCoroutine(CheckAttack());
    }
}

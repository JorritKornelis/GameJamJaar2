using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour
{
    Vector3 moveVector;
    public float moveSpeed;
    public bool mayMoveBool = true;
    Vector3 dashDir;
    public Animator ani;
    public ParticleSystem dust;

    [Header("Audio")]
    public AudioSource audioPlayerSource;
    public AudioClip move;
    public AudioClip shoot;
    public AudioClip shootCharge;
    public AudioClip roll;

    [Header("Death")]
    public GameObject vidHolder;

    [Header("Dash")]
    public float dashAmount;
    public float dashTime;
    public float timerCoolDown;
    float timerCoolDownReset;
    public bool mayDash = true;
    bool b = false;
    bool deleyBoolV2 = false;

    private void Start()
    {
        vidHolder.SetActive(false);
        timerCoolDownReset = timerCoolDown;
        ani = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (mayMoveBool == true)
        {
            PlayerMove();
            ani.SetBool("Run", true);
            dust.Play();
        }
        else
        {
            ani.SetBool("Run", false);
            dust.Stop();
        }
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            ani.SetBool("Run", false);
            dust.Stop();
        }
        else
        {
            if (deleyBoolV2 == false)
            {
                deleyBoolV2 = true;
                StartCoroutine(enumerator());
            }
        }

        LookToMouse();
        StartDash();
        if (b)
        {
            RestetBool();
        }
    }

    IEnumerator enumerator()
    {
        PlayPlayerAudioClip(move);
        while (audioPlayerSource.isPlaying)
            yield return null;
        deleyBoolV2 = false;
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
            dashDir = new Vector3();
            dashDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            if(dashDir != new Vector3())
            {
                StartCoroutine(Dash());
            }
        }
    }

    IEnumerator Dash()
    {
        float currentTime = dashTime;
        mayMoveBool = false;
        mayDash = false;

        ani.SetBool("Dodge", true);
        PlayPlayerAudioClip(roll);

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            transform.Translate(-dashDir * dashAmount * Time.deltaTime, Space.World);
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

    public void PlayerDeath()
    {
        vidHolder.SetActive(true);
        vidHolder.GetComponent<VideoPlayer>().Play();
        StartCoroutine(LoadMainMenuDeath());
    }

    public IEnumerator LoadMainMenuDeath()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayPlayerAudioClip(AudioClip clip)
    {
        audioPlayerSource.pitch = 1;
        audioPlayerSource.pitch += Random.Range(-0.05f, 0.05f);
        audioPlayerSource.PlayOneShot(clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesterLaser : MonoBehaviour
{
    public Transform center;
    public Vector3 size;
    public BossBase boss;

    public bool damage;
    public LayerMask playermask;
    public string playerTag;

    public IEnumerator CheckDamage()
    {
        yield return new WaitForSeconds(0.08f);

        while (damage)
        {
            if (Physics.CheckBox(center.position, size / 2, center.rotation, playermask))
            {
                GameObject.FindWithTag(playerTag).GetComponent<PlayerControler>().PlayerDeath();
                StartCoroutine(boss.SlowTime());
            }
            yield return null;
        }
    }

    public void SetInfo(bool _damage, BossBase _boss)
    {
        damage = _damage;
        boss = _boss;
        StartCoroutine(CheckDamage());
    }

    public void OnDrawGizmosSelected()
    {
        if (center)
            Gizmos.DrawWireCube(center.position, size);
    }
}

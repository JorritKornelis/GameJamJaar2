using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    public abstract void Damage();

    public abstract IEnumerator CheckAttack();
}

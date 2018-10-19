using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    int nDamage;
    float fRadius;

    public void Init(int nDamage, float fRadius)
    {
        this.nDamage = nDamage;
        this.fRadius = fRadius;
    }
    public int GetDamage()
    {
        return nDamage;
    }
    public float GetRadius()
    {
        return fRadius;
    }
}

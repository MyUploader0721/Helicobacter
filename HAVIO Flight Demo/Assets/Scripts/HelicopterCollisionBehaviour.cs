using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterCollisionBehaviour : MonoBehaviour
{
    bool bCollide = false;

    void OnCollisionEnter(Collision c)
    {
        bCollide = true;
    }

    public bool GetCollision() { return bCollide; }
    public void SetCollision(bool col) { bCollide = col; }
}

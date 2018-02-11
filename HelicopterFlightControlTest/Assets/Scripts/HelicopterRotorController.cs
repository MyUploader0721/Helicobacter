using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterRotorController : MonoBehaviour
{
    float fAngularSpeed = 0.0f;
    float fThrottleSpeed = 0.0f;

    public Vector3 v3RotationTo = Vector3.zero;

    public float fMaxAngularSpeed = 2.0f;
    public float fAccelarationInterval = 0.1f;
    public float fThrottleMultiplier = 10.0f;

    void FixedUpdate()
    {
        transform.Rotate(v3RotationTo, fAngularSpeed + (fThrottleSpeed * fThrottleMultiplier));
    }

    public void StartSpinning()
    {
        StopAllCoroutines();
        StartCoroutine(StartsSpinning());
    }

    public void EndSpinning()
    {
        StopAllCoroutines();
        StartCoroutine(EndsSpinning());
    }

    public void GetThrottlePercent(float fThrottle)
    {
        fThrottleSpeed = fThrottle;
    }

    IEnumerator StartsSpinning()
    {
        while (fAngularSpeed < fMaxAngularSpeed)
        {
            fAngularSpeed += 0.125f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }
    IEnumerator EndsSpinning()
    {
        while (fAngularSpeed > 0.0f)
        {
            fAngularSpeed -= 0.125f;
            if (fAngularSpeed < 0.0f)
                fAngularSpeed = 0.0f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }
}

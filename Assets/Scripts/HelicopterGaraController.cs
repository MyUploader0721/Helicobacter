using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterGaraController : MonoBehaviour
{
    [SerializeField] Transform trsMainRotor;
    [SerializeField] float fMainRotorSpeed;
    [Space]
    [SerializeField] Transform trsTailRotor;
    [SerializeField] float fTailRotorSpeed;
    [Space]

    bool bStartEngine = false;

    float fMainRotorCurrentSpeed = 0.0f;
    float fTailRotorCurrentSpeed = 0.0f;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !bStartEngine)
        {
            bStartEngine = true;
            StartCoroutine(StartMainRotorEngine());
            StartCoroutine(StartTailRotorEngine());
        }

        trsMainRotor.localEulerAngles += new Vector3(0.0f, -fMainRotorCurrentSpeed, 0.0f);
        trsTailRotor.localEulerAngles += new Vector3(fTailRotorCurrentSpeed, 0.0f, 0.0f);
    }

    IEnumerator StartMainRotorEngine()
    {
        while (fMainRotorCurrentSpeed < fMainRotorSpeed)
        {
            fMainRotorCurrentSpeed += (1.0f / 128.0f);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator StartTailRotorEngine()
    {
        while (fTailRotorCurrentSpeed < fTailRotorSpeed)
        {
            fTailRotorCurrentSpeed += (1.0f / 128.0f);
            yield return new WaitForEndOfFrame();
        }
    }
}

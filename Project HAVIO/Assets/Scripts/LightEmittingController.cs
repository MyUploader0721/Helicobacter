using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmittingController : MonoBehaviour
{
    [SerializeField] Light[] lights;
    [SerializeField] float fStartIntensity = 1.5f;
    [SerializeField] float fCurrentIntensity = 1.5f;

	IEnumerator Start ()
    {
        fCurrentIntensity = fStartIntensity;

        while(true)
        {
            while (fCurrentIntensity > 0.0f)
            {
                fCurrentIntensity -= (1.0f / 32.0f);
                foreach (Light l in lights)
                    l.intensity = fCurrentIntensity;

                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1.0f);
            while (fCurrentIntensity < fStartIntensity)
            {
                fCurrentIntensity += (1.0f / 32.0f);
                foreach (Light l in lights)
                    l.intensity = fCurrentIntensity;

                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}

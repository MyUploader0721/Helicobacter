using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelipadLightBehaviour : MonoBehaviour
{
    [SerializeField] Light[] lightLZ;
    public bool bTurnOn = false;

    IEnumerator Start()
    {
        while (!bTurnOn)
            yield return new WaitForEndOfFrame();

        foreach (Light l in lightLZ)
            l.enabled = true;

        while (true)
        {
            while (lightLZ[0].intensity < 5.0f)
            {
                foreach (Light l in lightLZ)
                    l.intensity += 0.25f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(2.0f);
            while (lightLZ[0].intensity > 0.0f)
            {
                foreach (Light l in lightLZ)
                    l.intensity -= 0.25f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}

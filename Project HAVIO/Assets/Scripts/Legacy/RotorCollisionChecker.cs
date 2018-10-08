using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorCollisionChecker : MonoBehaviour
{
    [SerializeField] HelicopterController helicopterController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            helicopterController.IsEngineAvailable = false;
        }
    }
}

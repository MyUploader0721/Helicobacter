using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageDeliveryLandingForEnd : MonoBehaviour
{
    [SerializeField] CarePackageDeliveryController cpdController;
    [SerializeField] HelicopterInfo helicopterInfo;
    [Space]
    [SerializeField] AudioClip sfxMissionClear;

    bool bLanded = false;

    AudioSource audioSource;

	void Start ()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
	
	void Update ()
    {
		
	}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !bLanded && helicopterInfo.bIsFlyable && !helicopterInfo.bIsEngineStart && cpdController.bMissionEnd)
        {
            bLanded = true;
            audioSource.PlayOneShot(sfxMissionClear);
            cpdController.bMissionAccomplished = true;
        }
    }
}

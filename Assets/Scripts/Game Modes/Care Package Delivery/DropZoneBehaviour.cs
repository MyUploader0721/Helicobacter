using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneBehaviour : MonoBehaviour
{
    [Header("Care Package Controller")]
    [SerializeField] CarePackageDeliveryController cpdController;
    [Space]
    [Header("Drop Zone Setting")]
    public bool bReceivedPackage = false;
    [Space]
    [SerializeField] AudioClip[] sfxDropSuccess;

    AudioSource audioSource;

	void Start ()
    {
        audioSource = cpdController.GetComponent<AudioSource>();
    }
	
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package") && !bReceivedPackage)
        {
            bReceivedPackage = true;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.PlayOneShot(sfxDropSuccess[cpdController.nCurrentReceiver]);
            cpdController.nCurrentReceiver++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package") && bReceivedPackage)
        {
            bReceivedPackage = false;

            cpdController.nCurrentReceiver--;
        }
    }
}

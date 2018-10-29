using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardTowerBehaviour : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] Transform trsPlayer;
    [Space]
    [SerializeField] Text txtSpotted;
    [Space]
    [Header("Guard Setting")]
    [SerializeField] Transform trsSearchlight;
    [SerializeField] float fRotationSpeed;
    [Space]
    [SerializeField] CarePackageDeliveryController cpdController;
    [Space]
    [SerializeField] AudioClip sfxWarning;

    AudioSource audioSource;

    bool bPlayerSpotted = false;

	void Start ()
    {
        audioSource = cpdController.GetComponent<AudioSource>();
    }
	
	void Update ()
    {
		
	}

    void FixedUpdate()
    {
        trsSearchlight.localEulerAngles += new Vector3(0.0f, fRotationSpeed * Time.fixedDeltaTime, 0.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !bPlayerSpotted)
        {
            bPlayerSpotted = true;
            StartCoroutine("SearchlightWarning");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && bPlayerSpotted)
        {
            bPlayerSpotted = false;
            if (txtSpotted.enabled)
                txtSpotted.enabled = false;
            StopCoroutine("SearchlightWarning");
        }
    }

    IEnumerator SearchlightWarning()
    {
        txtSpotted.enabled = true;

        audioSource.PlayOneShot(sfxWarning);
        txtSpotted.color = Color.yellow;
        txtSpotted.text = "AVOID THE SPOTLIGHT! 3";
        yield return new WaitForSeconds(1.0f);

        audioSource.PlayOneShot(sfxWarning);
        txtSpotted.color = Color.red;
        txtSpotted.text = "AVOID THE SPOTLIGHT! 2";
        yield return new WaitForSeconds(1.0f);

        audioSource.PlayOneShot(sfxWarning);
        txtSpotted.text = "AVOID THE SPOTLIGHT! 1";
        yield return new WaitForSeconds(1.0f);

        txtSpotted.enabled = false;

        cpdController.bMissionEnd = true;
        cpdController.bMissionFailed = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock6 : MonoBehaviour
{
    [Header("Tutorial Controller")]
    [SerializeField] TutorialController tutorialController;
    [Space]
    [Header("Player Setting")]
    [SerializeField] InputController inputController;
    [SerializeField] HelicopterInfo helicopterInfo;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip sfxTutNarr6_1;

    bool bLanded = false;
    bool bNarrated = false;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (bLanded && !bNarrated && helicopterInfo.bIsFlyable && !helicopterInfo.bIsEngineStart)
        {
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DoNarration());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) bLanded = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) bLanded = false;
    }

    IEnumerator DoNarration()
    {
        bNarrated = true;

        yield return StartCoroutine(tutorialController.StabilizeHelicopter());

        audioSource.PlayOneShot(sfxTutNarr6_1);
        yield return new WaitForSeconds(sfxTutNarr6_1.length);

        tutorialController.SetMissionValue(true);
        inputController.bControllable = false;

        Destroy(gameObject);
    }
}

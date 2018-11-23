using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock5 : MonoBehaviour
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
    [SerializeField] AudioClip sfxTutNarr5_1;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (!helicopterInfo.bIsFlyable)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            StopCoroutine(DoNarration());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && helicopterInfo.bIsFlyable)
        {
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DoNarration());
        }
    }

    IEnumerator DoNarration()
    {
        yield return StartCoroutine(tutorialController.StabilizeHelicopter());

        audioSource.PlayOneShot(sfxTutNarr5_1);
        yield return new WaitForSeconds(sfxTutNarr5_1.length);

        tutorialController.LightLandingZone();
        inputController.bControllable = true;

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock1 : MonoBehaviour
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
    [SerializeField] AudioClip sfxTutNarr2_1;
    [SerializeField] AudioClip sfxTutNarr2_2;
    [SerializeField] AudioClip sfxTutNarr2_3;

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

        audioSource.PlayOneShot(sfxTutNarr2_1);
        yield return new WaitForSeconds(sfxTutNarr2_1.length);

        inputController.bControllable = true;
        while (!inputController.GetComponentInChildren<SearchLightBehaviour>().bLightOn)
        {
            yield return new WaitForEndOfFrame();
        }

        // yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(tutorialController.StabilizeHelicopter());

        audioSource.PlayOneShot(sfxTutNarr2_2);
        yield return new WaitForSeconds(sfxTutNarr2_2.length + 0.5f);

        inputController.bControllable = true;
        audioSource.PlayOneShot(sfxTutNarr2_3);
        yield return new WaitForSeconds(sfxTutNarr2_3.length);

        Destroy(gameObject);
    }
}

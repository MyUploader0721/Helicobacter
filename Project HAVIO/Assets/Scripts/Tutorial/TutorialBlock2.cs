using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock2 : MonoBehaviour
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
    [SerializeField] AudioClip sfxTutNarr3_1;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (!helicopterInfo.bIsFlyable) StopCoroutine(DoNarration());
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
        audioSource.PlayOneShot(sfxTutNarr3_1);
        yield return new WaitForSeconds(sfxTutNarr3_1.length);

        Destroy(gameObject);
    }
}

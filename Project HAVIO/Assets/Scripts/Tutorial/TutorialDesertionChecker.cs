using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDesertionChecker : MonoBehaviour
{
    [Header("Tutorial Setting")]
    [SerializeField] TutorialController tutorialController;
    [Space]
    [Header("Player Setting")]
    [SerializeField] Transform trsPlayer;
    [Space]
    [Header("UI Setting")]
    [SerializeField] Text txtCountdown;
    [Space]
    [Header("SFX")]
    AudioSource audioSource;

    float fAltitude = 0.0f;

    bool bIsTooHigh = false;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
    }
	
	void Update ()
    {
        fAltitude = trsPlayer.position.y;

        if (fAltitude > 8.5f)
        {
            if (!bIsTooHigh)
            {
                StartCoroutine("AltitudeWarning");
            }
        }
        else
        {
            if (bIsTooHigh)
            {
                if (txtCountdown.gameObject.activeInHierarchy)
                    txtCountdown.gameObject.SetActive(false);
                StopCoroutine("AltitudeWarning");
                bIsTooHigh = false;
            }
        }
    }
    
    IEnumerator AltitudeWarning()
    {
        bIsTooHigh = true;
        txtCountdown.gameObject.SetActive(true);

        audioSource.Play();
        txtCountdown.color = Color.yellow;
        txtCountdown.text = "3";
        yield return new WaitForSeconds(1.5f);

        audioSource.Play();
        txtCountdown.color = Color.red;
        txtCountdown.text = "2";
        yield return new WaitForSeconds(1.5f);

        audioSource.Play();
        txtCountdown.text = "1";
        yield return new WaitForSeconds(1.5f);

        txtCountdown.gameObject.SetActive(false);

        tutorialController.SetMissionValue(false);
    }
}

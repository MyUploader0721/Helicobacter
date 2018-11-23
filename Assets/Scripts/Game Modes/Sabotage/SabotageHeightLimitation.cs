using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SabotageHeightLimitation : MonoBehaviour
{
    [Header("Sabotage Setting")]
    [SerializeField] SabotageController sabotageController;
    [SerializeField] float fHeightLimit;
    [Space]
    [Header("Player Setting")]
    [SerializeField] Transform trsPlayer;
    [Space]
    [Header("UI Setting")]
    [SerializeField] Text txtCountdown;
    [SerializeField] Text txtAltitude;
    [Space]
    [Header("SFX")]
    AudioSource audioSource;
    [SerializeField] AudioClip sfxWarning;

    float fAltitude = 0.0f;

    bool bIsTooHigh = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update ()
    {
        fAltitude = trsPlayer.position.y;

        if (fAltitude > fHeightLimit)
        {
            if (!bIsTooHigh)
            {
                txtAltitude.color = Color.red;
                StartCoroutine("AltitudeWarning");
            }
        }
        else
        {
            if (bIsTooHigh)
            {
                txtAltitude.color = new Color(0.9296875f, 0.9296875f, 0.9296875f);
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

        audioSource.PlayOneShot(sfxWarning);
        txtCountdown.color = Color.yellow;
        txtCountdown.text = "3";
        yield return new WaitForSeconds(1.5f);

        audioSource.PlayOneShot(sfxWarning);
        txtCountdown.color = Color.red;
        txtCountdown.text = "2";
        yield return new WaitForSeconds(1.5f);

        audioSource.PlayOneShot(sfxWarning);
        txtCountdown.text = "1";
        yield return new WaitForSeconds(1.5f);

        txtCountdown.gameObject.SetActive(false);

        sabotageController.SetMissionVariable(false);
    }
}

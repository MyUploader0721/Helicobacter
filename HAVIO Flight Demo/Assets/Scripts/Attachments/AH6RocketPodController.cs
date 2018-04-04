using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 3.
   Description : AH-6의 로켓 포드를 관리하는 스크립트
   Edit Log    : 
    - 로켓포드를 발사하는 등의 작업을 처리합니다. 
   ================================================================= */

public class AH6RocketPodController : MonoBehaviour
{
    [Header("Rocket Pod Objects")]
    public GameObject objLeftLauncher;
    public GameObject objRightLauncher;
    [Header("Rocket Prefab")]
    public GameObject objRocket;

    [Header("Pod Armament Setting")]
    public int nTotalArmament = 70;
    public int nMaxPodArmament = 14;
    public int nCurrentRound = 0;

    public float fInterval = 0.5f;
    public float fReloadTime = 5.0f;

    bool bIsShooting = false;
    bool bIsReloading = false;

    bool bTikTok = false;

    HelicopterFlightController hfc;

    [Header("SFX Setting")]
    AudioSource []audioSource;
    public AudioClip sfxReloading;
    public AudioClip sfxReloaded;
    public AudioClip[] sfxRocketLaunch;

    void Start ()
    {
        hfc = GameObject.FindGameObjectWithTag("Player").GetComponent<HelicopterFlightController>();
        audioSource = new AudioSource[2];
        audioSource[0] = GetComponent<AudioSource>();
        audioSource[0].maxDistance = 3.5f;
        audioSource[1] = GetComponent<AudioSource>();
        audioSource[1].maxDistance = 3.5f;

        nCurrentRound = nMaxPodArmament;
    }
	
	void Update ()
    {
        if (
            (hfc.bPlayWithJoystick && Input.GetButton("Secondary Trigger")) ||
            (!hfc.bPlayWithJoystick && Input.GetKey(KeyCode.LeftControl))
           )
        {
            if (!bIsShooting && !bIsReloading)
                StartCoroutine(ShotRocket());

            if (nCurrentRound == 0)
                StartCoroutine(Reloading());
        }
    }

    /// <summary>
    /// 로켓을 양쪽 포드에서 번갈아가면서 발사합니다. 
    /// </summary>
    /// <returns>코루틴 메소드입니다. </returns>
    IEnumerator ShotRocket()
    {
        if (nCurrentRound > 0)
        {
            bIsShooting = true;
            nCurrentRound--;

            if (bTikTok)
                Instantiate(objRocket, objLeftLauncher.transform.position, hfc.rigidBody.rotation);
            else
                Instantiate(objRocket, objRightLauncher.transform.position, hfc.rigidBody.rotation);
            audioSource[1].PlayOneShot(sfxRocketLaunch[Random.Range(0, sfxRocketLaunch.Length)]);

            bTikTok = !bTikTok;

            yield return new WaitForSeconds(fInterval);
            bIsShooting = false;
        }
    }
    /// <summary>
    /// 탄을 모두 소진하였을 경우 재장전을 합니다. 
    /// </summary>
    /// <returns>코루틴 메소드입니다. </returns>
    IEnumerator Reloading()
    {
        bIsReloading = true;

        if (nTotalArmament > 0)
        {
            audioSource[0].PlayOneShot(sfxReloading);

            if (nTotalArmament >= nMaxPodArmament)
            {
                nTotalArmament -= nMaxPodArmament;
                nCurrentRound += nMaxPodArmament;
            }
            else
            {
                nCurrentRound = nTotalArmament;
                nTotalArmament = 0;
            }

            yield return new WaitForSeconds(fReloadTime);

            audioSource[0].PlayOneShot(sfxReloaded);
        }

        bIsReloading = false;
    }
}

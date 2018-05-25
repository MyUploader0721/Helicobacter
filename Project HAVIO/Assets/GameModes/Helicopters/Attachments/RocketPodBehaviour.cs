using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RocketPodBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-07
 * DESCRIPTION: 헬리콥터 로켓 포드의 행위를 정의한 스크립트
 *     DEV LOG: 
 *  - 새로운 스크립트에 적용되는 로켓포드 스크립트입니다. 
 *  - 다양한 로켓포드에 대응하기 위하여 재사용 가능한
 *    스크립트로 작성하도록 노력하였습니다. 
 */

public class RocketPodBehaviour : MonoBehaviour
{
    [Header("Rocket Pod Objects")]
    [SerializeField] GameObject objPositionLeftLauncher;
    [SerializeField] GameObject objPositionRightLauncher;
    [Header("Rocket Prefab")]
    [SerializeField] GameObject objRocket;

    [Header("Pod Armament Setting")]
    [SerializeField] int nTotalArmament = 70;
    [SerializeField] int nMaxPodArmament = 14;
    [SerializeField] int nCurrentRound = 0;

    [SerializeField] float fInterval = 0.5f;
    [SerializeField] float fReloadTime = 5.0f;

    public enum PodPosition { NONE, INNER, OUTER }
    [Header("Pod Position Setting")]
    public PodPosition ppPosition = PodPosition.NONE;

    bool bIsShooting = false;
    bool bIsReloading = false;

    bool bTikTok = false;

    //HelicopterInfo helicopterInfo;
    InputController inputController;
    
    AudioSource []audioSource;
    enum SFX_List { LOADER = 0, LAUNCHER = 1 };
    [Header("SFX: Rocket Sounds")]
    public AudioClip sfxReloading;
    public AudioClip sfxReloaded;
    public AudioClip[] sfxRocketLaunch;

    void Start ()
    {
        //helicopterInfo = GetComponentInParent<HelicopterInfo>();
        inputController = GetComponentInParent<InputController>();

        audioSource = new AudioSource[2];
        audioSource[(int)SFX_List.LOADER] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.LOADER].maxDistance = 3.5f;
        audioSource[(int)SFX_List.LAUNCHER] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.LAUNCHER].maxDistance = 3.5f;

        nCurrentRound = nMaxPodArmament;
    }
	
	void Update ()
    {
        if (
            (ppPosition == PodPosition.INNER && inputController.bActivateInnerPod) ||
            (ppPosition == PodPosition.OUTER && inputController.bActivateOuterPod)
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
                Instantiate(objRocket, objPositionLeftLauncher.transform.position, transform.rotation, null);
            else
                Instantiate(objRocket, objPositionRightLauncher.transform.position, transform.rotation, null);

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
            audioSource[(int)SFX_List.LOADER].PlayOneShot(sfxReloading);

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

            audioSource[(int)SFX_List.LOADER].PlayOneShot(sfxReloaded);
        }

        bIsReloading = false;
    }
}

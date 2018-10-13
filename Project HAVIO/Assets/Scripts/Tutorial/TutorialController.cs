using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] InputController inputController;
    [SerializeField] HelicopterInfo helicopterInfo;
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] new Transform transform;
    [SerializeField] Transform trsMainCamera;
    MotionInput motionInput;
    [Space]
    [Header("Mission Setting")]
    [SerializeField] Transform trsMissionEndPosition;
    [SerializeField] GameObject objSuccessPanel;
    [SerializeField] GameObject objFailedPanel;
    [Space]
    [Header("Fader")]
    [SerializeField] SceneFadingController sfc;
    [Space]
    AudioSource audioSource;
    [Header("SFX")]
    [SerializeField] AudioClip sfxTutNarr1_1;
    [SerializeField] AudioClip sfxTutNarr1_2;
    [SerializeField] AudioClip sfxTutNarr1_3;
    [SerializeField] AudioClip sfxTutNarr1_4;
    [Space]
    [SerializeField] AudioClip sfxMissionSuccess;
    [SerializeField] AudioClip sfxMissionFailed;
    [Space]
    [Header("Etc.")]
    [SerializeField]Light[] lightLZ;

    [HideInInspector] public bool bIsStabilizing = false;

    [HideInInspector] public bool bTutorialEnd = false;
    bool bTutorialSuccess = false;
    bool bTutorialFailed = false;
    bool bFadeOutAndInCalled = false;

    void Start ()
    {
        audioSource = transform.gameObject.AddComponent<AudioSource>();
        motionInput = transform.GetComponent<MotionInput>();

        sfc.FadeIn();
        StartCoroutine("TutorialNarrStart");
	}
	
	void Update ()
    {
        if (!helicopterInfo.bIsFlyable) StopCoroutine(TutorialNarrStart());

        // 임무가 끝나버림
        if (bTutorialEnd && (bTutorialFailed || bTutorialSuccess))
        {
            inputController.bControllable = false;

            if (!bFadeOutAndInCalled)
            {
                if (bTutorialFailed)
                    audioSource.PlayOneShot(sfxMissionFailed);
                else if (bTutorialSuccess)
                    audioSource.PlayOneShot(sfxMissionSuccess);

                if (motionInput.UseAutoRotation)
                {
                    motionInput.UseAutoRotation = false;
                    motionInput.SetInputValues(0.0f);
                }

                bFadeOutAndInCalled = true;

                sfc.FadeOutAndIn(delegate {
                    trsMainCamera.SetParent(trsMissionEndPosition);
                    trsMainCamera.localPosition = Vector3.zero;
                    trsMainCamera.localRotation = Quaternion.identity;

                    if (bTutorialFailed)
                        objFailedPanel.SetActive(true);
                    else if (bTutorialSuccess)
                        objSuccessPanel.SetActive(true);
                });
            }
        }

        // 추락하여 망가짐
        if (!helicopterInfo.bIsFlyable)
        {
            SetMissionValue(false);
        }
	}

    IEnumerator TutorialNarrStart()
    {
        inputController.bControllable = false;
        yield return new WaitForSeconds(1.5f);

        audioSource.PlayOneShot(sfxTutNarr1_1);
        yield return new WaitForSeconds(sfxTutNarr1_1.length + 0.5f);
        audioSource.PlayOneShot(sfxTutNarr1_2);
        yield return new WaitForSeconds(sfxTutNarr1_2.length);

        inputController.bControllable = true;
        while (!helicopterInfo.bIsEngineStart)
        {
            yield return new WaitForEndOfFrame();
        }

        inputController.bControllable = false;
        yield return new WaitForSeconds(0.5f);

        audioSource.PlayOneShot(sfxTutNarr1_3);
        yield return new WaitForSeconds(sfxTutNarr1_3.length + 0.5f);

        yield return StartCoroutine("StabilizeHelicopter");
        audioSource.PlayOneShot(sfxTutNarr1_4);
        yield return new WaitForSeconds(sfxTutNarr1_4.length);

        inputController.bControllable = true;
    }

    public IEnumerator StabilizeHelicopter()
    {
        bIsStabilizing = true;
        inputController.bControllable = false;

        while (!IsBetween(1.025f, transform.eulerAngles.x, 1.075f) && !IsBetween(-0.075f, transform.eulerAngles.z, -0.025f))
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        bIsStabilizing = false;
    }

    bool IsBetween(float smaller, float target, float bigger)
    {
        if (smaller <= target && target <= bigger) return true;
        else return false;
    }

    public void SetMissionValue(bool isSuccess)
    {
        if (!bTutorialEnd)
        {
            bTutorialEnd = true;

            if (isSuccess) bTutorialSuccess = true;
            else bTutorialFailed = true;
        }
    }

    public void LightLandingZone()
    {
        StartCoroutine(_LightLandingZone());
    }
    IEnumerator _LightLandingZone()
    {
        foreach (Light l in lightLZ)
            l.enabled = true;

        while (true)
        {
            while (lightLZ[0].intensity < 5.0f)
            {
                foreach (Light l in lightLZ)
                    l.intensity += 0.25f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(2.0f);
            while (lightLZ[0].intensity > 0.0f)
            {
                foreach (Light l in lightLZ)
                    l.intensity -= 0.25f;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}

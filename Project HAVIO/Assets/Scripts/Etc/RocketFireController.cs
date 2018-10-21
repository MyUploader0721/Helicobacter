using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFireController : MonoBehaviour
{
    [Header("Rocket Setting")]
    [SerializeField] Transform trsLeftPod;
    [SerializeField] Transform trsRightPod;
    [Space]
    [SerializeField] int nMaxLeftRocket;
    [SerializeField] int nMaxRightRocket;
    [Space]
    [SerializeField] GameObject objRocketPref;
    [SerializeField] float fInterval = 1.0f;
    [SerializeField] float fReloadingTime;
    [Space]
    [Header("Settings")]
    [SerializeField] HelicopterInfo helicopterInfo;
    [Space]
    [SerializeField] Transform trsMainCamera;
    [SerializeField] Transform trsLaser;
    [Space]
    [SerializeField] float fHorizontalLimitAngle = 70.0f;
    [SerializeField] float fVerticalLimitAngle = 45.0f;
    [SerializeField] float fVerticalBaseAngle = 30.0f;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioClip sfxReloading;
    [SerializeField] AudioClip sfxReloaded;
    [SerializeField] AudioClip[] sfxPodShot;

    AudioSource audioSource;

    int nCurrentLeftRocket;
    int nCurrentRightRocket;

    bool bRightTurn;
    [HideInInspector] public bool bFireAvailable = true;
    bool bIsReloading;
    bool bFiring = false;

    Vector3 v3FireDirection;
    Vector3 v3FireDestination;

    LineRenderer lineRenderer;


    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        nCurrentLeftRocket = nMaxLeftRocket;
        nCurrentRightRocket = nMaxRightRocket;

        lineRenderer = GetComponent<LineRenderer>();
    }
	
	void Update ()
    {
		if (bFireAvailable && !bFiring && !bIsReloading && Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(FireRocket());
        }
        if (!bIsReloading && (nCurrentLeftRocket + nCurrentRightRocket) < (nMaxLeftRocket + nMaxRightRocket) && Input.GetKey(KeyCode.R))
        {
            StartCoroutine(Reloading());
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            lineRenderer.enabled = !lineRenderer.enabled;
        }

        bFireAvailable = helicopterInfo.bIsFlyable;

        Vector3 v3HorTrs = new Vector3(transform.forward.x, 0.0f, transform.forward.z),
                v3HorCam = new Vector3(trsMainCamera.forward.x, 0.0f, trsMainCamera.forward.z),
                v3VerTrs = new Vector3(0.0f, transform.forward.y, transform.forward.z),
                v3VerCam = new Vector3(0.0f, trsMainCamera.forward.y, trsMainCamera.forward.z);
        v3VerTrs = Quaternion.Euler(fVerticalBaseAngle, 0.0f, 0.0f) * v3VerTrs;
        if ((Mathf.Abs(Mathf.Acos(Vector3.Dot(v3HorTrs.normalized, v3HorCam.normalized)) * Mathf.Rad2Deg) < fHorizontalLimitAngle &&
             Mathf.Abs(Mathf.Acos(Vector3.Dot(v3VerTrs.normalized, v3VerCam.normalized)) * Mathf.Rad2Deg) < fVerticalLimitAngle))
        {
            v3FireDirection = trsMainCamera.forward;

            RaycastHit hit;
            Physics.Raycast(trsMainCamera.position, v3FireDirection, out hit, Mathf.Infinity);

            v3FireDestination = hit.point;
            lineRenderer.SetPosition(0, trsLaser.position);
            lineRenderer.SetPosition(1, v3FireDestination);

            /*
            if (Physics.Raycast(trsMainCamera.position, v3FireDirection, out hit, Mathf.Infinity))
            {
                v3FireDestination = hit.point;
                lineRenderer.SetPosition(0, trsLaser.position);
                lineRenderer.SetPosition(1, v3FireDestination);
            }
            else
            {
                lineRenderer.SetPosition(0, trsLaser.position);
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0) + v3FireDirection * 50.0f);
            }*/
        }
    }

    IEnumerator FireRocket()
    {
        bFiring = true;
        if (bRightTurn)
        {
            if (nCurrentRightRocket > 0)
            {
                Instantiate(objRocketPref, trsRightPod.position, Quaternion.LookRotation(v3FireDirection, transform.up), null);
                trsRightPod.GetComponent<AudioSource>().PlayOneShot(sfxPodShot[Random.Range(0, sfxPodShot.Length)]);
                nCurrentRightRocket--;
                yield return new WaitForSeconds(fInterval);
            }
            bRightTurn = false;
        }
        else
        {
            if (nCurrentLeftRocket > 0)
            {
                Instantiate(objRocketPref, trsLeftPod.position, Quaternion.LookRotation(v3FireDirection, transform.up), null);
                trsLeftPod.GetComponent<AudioSource>().PlayOneShot(sfxPodShot[Random.Range(0, sfxPodShot.Length)]);
                nCurrentLeftRocket--;
                yield return new WaitForSeconds(fInterval);
            }
            bRightTurn = true;
        }
        bFiring = false;

        if (nCurrentLeftRocket == 0 && nCurrentRightRocket == 0)
        {
            yield return StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        bIsReloading = true;
        trsLeftPod.GetComponent<AudioSource>().PlayOneShot(sfxReloading);
        trsRightPod.GetComponent<AudioSource>().PlayOneShot(sfxReloading);

        yield return new WaitForSeconds(fReloadingTime);
        trsLeftPod.GetComponent<AudioSource>().PlayOneShot(sfxReloaded);
        trsRightPod.GetComponent<AudioSource>().PlayOneShot(sfxReloaded);
        nCurrentLeftRocket = nMaxLeftRocket;
        nCurrentRightRocket = nMaxRightRocket;

        bIsReloading = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderBehaviour : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] Transform trsPlayer;
    [SerializeField] HelicopterInfo helicopterInfo;
    [SerializeField] InputController inputController;
    [Space]
    [SerializeField] float fBottomAwayFromHelicopter;
    [Space]
    [Header("Loader Info")]
    [SerializeField] Transform trsLift;
    [SerializeField] float fStartYPos;
    [SerializeField] float fEndYPos;
    [SerializeField] float fLoadingTime;
    [Space]
    [SerializeField] GameObject objPackage;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] AudioClip sfxStartLoad;
    [SerializeField] AudioClip sfxFinishLoad;

    bool bIsStabilizing = false;
    bool bIsLoading = false;

    MeshRenderer meshRenderer;
    
	void Start ()
    {
        helicopterInfo = trsPlayer.GetComponent<HelicopterInfo>();
        inputController = trsPlayer.GetComponent<InputController>();

        meshRenderer = GetComponent<MeshRenderer>();
    }
	
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (meshRenderer.enabled)
            meshRenderer.enabled = false;

        if (other.CompareTag("Player") && helicopterInfo.objCargo == null && !bIsLoading)
        {
            bIsLoading = true;
            StartCoroutine(LoadPackage());
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (meshRenderer.enabled)
            meshRenderer.enabled = false;
    }
    void OnTriggerExit(Collider other)
    {
        if (!meshRenderer.enabled)
            meshRenderer.enabled = true;
    }

    IEnumerator LoadPackage()
    {
        yield return StartCoroutine(StabilizeHelicopter());

        audioSource.PlayOneShot(sfxStartLoad);

        Vector3 v3Pos = trsLift.localPosition;
        while (trsLift.localPosition.y < fEndYPos)
        {
            v3Pos.y += 0.03125f;
            trsLift.localPosition = v3Pos;
            yield return new WaitForFixedUpdate();
        }

        audioSource.PlayOneShot(sfxFinishLoad);

        GameObject objInstantPackage = Instantiate(objPackage, trsPlayer.transform);
        objInstantPackage.transform.localPosition = new Vector3(0.0f, fBottomAwayFromHelicopter, 0.0f);
        objInstantPackage.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        trsPlayer.GetComponent<HelicopterInfo>().objCargo = objInstantPackage;

        inputController.bControllable = true;
        
        audioSource.PlayOneShot(sfxStartLoad);

        while (trsLift.localPosition.y > fStartYPos)
        {
            v3Pos.y -= 0.03125f;
            trsLift.localPosition = v3Pos;
            yield return new WaitForFixedUpdate();
        }

        bIsLoading = false;
    }

    public IEnumerator StabilizeHelicopter()
    {
        bIsStabilizing = true;
        inputController.bControllable = false;

        while (!IsBetween(1.025f, trsPlayer.eulerAngles.x, 1.075f) && !IsBetween(-0.075f, trsPlayer.eulerAngles.z, -0.025f))
        {
            Quaternion q = Quaternion.FromToRotation(trsPlayer.up, Vector3.up) * trsPlayer.rotation;
            trsPlayer.rotation = Quaternion.Slerp(trsPlayer.rotation, q, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        bIsStabilizing = false;
    }

    bool IsBetween(float smaller, float target, float bigger)
    {
        if (smaller <= target && target <= bigger) return true;
        else return false;
    }
}
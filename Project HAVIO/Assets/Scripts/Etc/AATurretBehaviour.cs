using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AATurretBehaviour : MonoBehaviour
{
    [Header("AA Turret Info")]
    [SerializeField] Transform trsBody;
    [SerializeField] Transform trsGun;
    [SerializeField] Transform trsFirePos;
    [Space]
    [SerializeField] GameObject objBullet;
    [Space]
    [Header("Target Info")]
    [SerializeField] Transform trsPlayer;
    [SerializeField] HelicopterInfo helicopterInfo;
    [Space]
    [SerializeField] float fTargetDistance;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioClip[] sfxFire;

    LineRenderer lineRenderer;
    AudioSource audioSource;

    HAVIODurability dur;

    float fLastShot;

	void Start ()
    {
        trsPlayer = GameObject.Find("Boeing AH-6").transform;
        helicopterInfo = trsPlayer.GetComponent<HelicopterInfo>();

        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        dur = GetComponent<HAVIODurability>();
    }
	
	void Update ()
    {
		if (dur.nCurrDurability > 0 && helicopterInfo.bIsFlyable && Vector3.Distance(transform.position, trsPlayer.position) < fTargetDistance)
        {
            Vector3 v = trsPlayer.position - trsBody.position; v.y = 0.0f;
            trsBody.forward = Vector3.Lerp(trsBody.forward, v.normalized, Time.deltaTime);

            v = trsPlayer.position - trsBody.position;
            float fAngle = Mathf.Atan2(v.y, Mathf.Sqrt(v.x*v.x+v.y*v.y)) * Mathf.Rad2Deg;
            trsGun.localEulerAngles = new Vector3(-fAngle, 0.0f, 0.0f);

            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            lineRenderer.SetPosition(0, trsFirePos.position);
            lineRenderer.SetPosition(1, trsFirePos.position + trsFirePos.forward * Vector3.Distance(transform.position, trsPlayer.position));

            if (Time.time - fLastShot > 5.0f)
            {
                fLastShot = Time.time;
                StartCoroutine(Shot());
            }
        }
        else
        {
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
        }

        if (dur.nCurrDurability <= 0)
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
                mr.enabled = false;
        }
	}

    IEnumerator Shot()
    {
        for (int i = 0; i < 5; i++)
        {
            audioSource.PlayOneShot(sfxFire[Random.Range(0, sfxFire.Length)]);
            Instantiate(objBullet, trsFirePos.position, trsFirePos.rotation, null);
            yield return new WaitForSeconds(0.2f);
        }
    }
}

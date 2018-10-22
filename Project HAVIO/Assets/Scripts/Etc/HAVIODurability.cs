using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAVIODurability : MonoBehaviour
{
    [Header("Durability Setting")]
    [SerializeField] int nMaxDurability;
    public int nCurrDurability;
    [Space]
    [Header("End")]
    [SerializeField] GameObject objExplosion;
    [SerializeField] AudioClip[] sfxExplosion;

    AudioSource audioSource;
    bool bIsExploded = false;

    //Material mat;

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        nCurrDurability = nMaxDurability;

        //mat = new Material(GetComponent<MeshRenderer>().material);
    }
	
	void Update ()
    {
        if (nCurrDurability < 0)
        {
            if (objExplosion)
                Instantiate(objExplosion);
            if (audioSource && !bIsExploded)
            {
                bIsExploded = true;
                StartCoroutine(KillAfterSFXDone());
            }

            foreach (Collider co in GetComponentsInChildren<Collider>())
                co.enabled = false;
        }
            

        /* //for the test
        mat.color = new Color(((float)nCurrDurability / nMaxDurability), 0.0f, 0.0f);
        GetComponent<MeshRenderer>().material = mat;
        */
	}

    void OnTriggerEnter(Collider co)
    {
        if (co.CompareTag("Explosion Radius"))
        {
            ExplosionRadius sdc = co.GetComponent<ExplosionRadius>();

            float fExpDistance = Vector3.Distance(co.ClosestPoint(transform.position), co.transform.position);
            if (fExpDistance < sdc.GetRadius())
            {
                Debug.Log("Rocket affects on enemy: " + (sdc.GetDamage() * Mathf.Cos(fExpDistance * Mathf.PI / (2.0f * sdc.GetRadius()))));
                nCurrDurability -= (int)(sdc.GetDamage() * Mathf.Cos(fExpDistance * Mathf.PI / (2.0f * sdc.GetRadius())));
            }
        }
    }

    IEnumerator KillAfterSFXDone()
    {
        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;

        if (audioSource)
        {
            audioSource.PlayOneShot(sfxExplosion[Random.Range(0, sfxExplosion.Length)]);

            while (audioSource.isPlaying)
                yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}

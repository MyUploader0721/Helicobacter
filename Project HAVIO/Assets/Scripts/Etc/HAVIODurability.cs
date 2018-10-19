using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAVIODurability : MonoBehaviour
{
    [Header("Durability Setting")]
    [SerializeField] int nMaxDurability;
    [SerializeField] int nCurrDurability;

    //Material mat;

    void Start ()
    {
        nCurrDurability = nMaxDurability;

        //mat = new Material(GetComponent<MeshRenderer>().material);
    }
	
	void Update ()
    {
        if (nCurrDurability < 0)
            Destroy(gameObject);

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

            float fExpDistance = Vector3.Distance(transform.position, co.transform.position);
            if (fExpDistance < sdc.GetRadius())
                nCurrDurability -= (int)(sdc.GetDamage() * Mathf.Cos(fExpDistance * Mathf.PI / (2.0f * sdc.GetRadius())));
        }
    }
}

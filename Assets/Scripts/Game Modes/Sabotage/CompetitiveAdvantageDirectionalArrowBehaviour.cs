using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetitiveAdvantageDirectionalArrowBehaviour : MonoBehaviour
{
    public Transform trsTarget;
	
	void Update ()
    {
		if (trsTarget)
        {
            transform.LookAt(trsTarget);
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
	}
}

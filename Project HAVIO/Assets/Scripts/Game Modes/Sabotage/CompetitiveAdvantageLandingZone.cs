using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetitiveAdvantageLandingZone : MonoBehaviour
{
    [SerializeField] SabotageController sbtgCtrl;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.attachedRigidbody.GetComponent<HelicopterInfo>().bIsFlyable && !other.attachedRigidbody.GetComponent<HelicopterInfo>().bIsEngineStart)
        {
            sbtgCtrl.SetMissionVariable(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterGroundChecker : MonoBehaviour
{
    [SerializeField] HelicopterController helicopterController;

	void Start ()
    {
        if (!helicopterController)
            helicopterController = GameObject.FindGameObjectWithTag("Player").GetComponent<HelicopterController>();
	}
	
	void Update ()
    {
		
	}

    void OnTriggerStay(Collider other)
    {
        // if the helicopter is on the ground(terrain)
        if (other.CompareTag("Terrain"))
        {
            helicopterController.IsOnTheGround = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        // if the helicopter is over the ground(terrain)
        if (other.CompareTag("Terrain"))
        {
            helicopterController.IsOnTheGround = false;
        }
    }
}

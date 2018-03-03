using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterCollisionController : MonoBehaviour
{
    public HelicopterCollisionBehaviour[] hcb;

    HelicopterFlightController hfc;
    Rigidbody rigidBody;

    float fDamage = 0.0f;

	void Start ()
    {
        hfc = GetComponent<HelicopterFlightController>();
        rigidBody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
    {
        if (hfc.bEngineStatus == true)
        {
            foreach (HelicopterCollisionBehaviour h in hcb)
            {
                if (h.GetCollision() == true)
                {
                    fDamage += rigidBody.velocity.magnitude;
                    h.SetCollision(false);
                }
            }

            Debug.Log("Damage: " + fDamage);
        }
	}
}

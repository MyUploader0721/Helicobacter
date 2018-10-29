using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTowerBehaviour : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] Transform trsPlayer;
    [Space]
    [Header("Guard Setting")]
    [SerializeField] Transform trsSearchlight;
    [SerializeField] float fRotationSpeed;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    void FixedUpdate()
    {
        trsSearchlight.localEulerAngles += new Vector3(0.0f, fRotationSpeed * Time.fixedDeltaTime, 0.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You Spotted!");
        }
    }
}

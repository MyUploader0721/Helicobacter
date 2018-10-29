using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip sfxDetach;

    HelicopterInfo helicopterInfo;
    AudioSource audioSource;

	void Start ()
    {
        helicopterInfo = transform.parent.GetComponent<HelicopterInfo>();
        audioSource = transform.parent.GetComponent<AudioSource>();
    }
	
	void Update ()
    {
		if (transform.parent != null && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("RightJoystickButton")))
        {
            audioSource.PlayOneShot(sfxDetach);

            transform.parent = null;
            helicopterInfo.objCargo = null;

            gameObject.AddComponent<Rigidbody>();
        }
	}
}

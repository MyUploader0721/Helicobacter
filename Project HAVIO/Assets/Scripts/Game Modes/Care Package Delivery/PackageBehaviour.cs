using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip sfxDetach;
    [Space]
    [SerializeField] AudioClip[] sfxWrong;

    HelicopterInfo helicopterInfo;
    AudioSource audioSource;

    bool bIsOnTerrain = false;

	void Start ()
    {
        helicopterInfo = transform.parent.GetComponent<HelicopterInfo>();
        audioSource = gameObject.AddComponent<AudioSource>();
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain") && !bIsOnTerrain && !transform.parent)
        {
            bIsOnTerrain = true;
            audioSource.PlayOneShot(sfxWrong[Random.Range(0, sfxWrong.Length)]);
        }
    }
}

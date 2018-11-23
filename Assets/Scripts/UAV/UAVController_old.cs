using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAVController_old : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] int nMaxDurability;
    [SerializeField] int nCurrentDurability;
    [Space]
    [Header("Rotor")]
    [SerializeField] Transform trsMainRotor;
    [SerializeField] Transform trsTailRotor;
    [Space]
    [SerializeField] float fMainRotorSpeed;
    [SerializeField] float fTailRotorSpeed;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioClip sfxRotor;

    AudioSource audioSource;
    Rigidbody rigidBody;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = sfxRotor;
        audioSource.Play();
        rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
        trsMainRotor.rotation *= Quaternion.Euler(0.0f, -fMainRotorSpeed, 0.0f);
        trsTailRotor.rotation *= Quaternion.Euler(fTailRotorSpeed, 0.0f, 0.0f);
    }
}

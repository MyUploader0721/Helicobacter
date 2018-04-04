using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 4.
   Description : 케어패키지가 어떻게 행동할 지 정의한 스크립트
   Edit Log    : 
    - 케어패키지는 보급포인트에서 얻습니다. 
    - 헬리콥터가 가지고 있는 보급품은 떨어뜨릴 수 있습니다. 
   ================================================================= */

public class CarePackageBehaviour : MonoBehaviour
{
    GameObject objPlayer;
    HelicopterInfoController hic;

    [Header("SFX Setting")]
    AudioSource []audioSource;
    public AudioClip sfxDetach;
    public AudioClip []sfxCollision;

    void Start ()
    {
        objPlayer = GameObject.FindGameObjectWithTag("Player");
        hic = objPlayer.GetComponent<HelicopterInfoController>();

        audioSource = new AudioSource[2];

        audioSource[0] = gameObject.AddComponent<AudioSource>();
        audioSource[0].clip = sfxDetach;

        audioSource[1] = gameObject.AddComponent<AudioSource>();
        audioSource[1].maxDistance = 5.0f;
	}
	
	void Update ()
    {
		if (
            (hic.bPlayWithJoystick && Input.GetButtonDown("DropPackage") && hic.objStowage != null) ||
            (!hic.bPlayWithJoystick && Input.GetKeyDown(KeyCode.V) && hic.objStowage != null)
           )
        {
            audioSource[0].Play();
            hic.objStowage.AddComponent<Rigidbody>().useGravity = true;
            hic.objStowage.transform.parent = null;
            hic.objStowage = null;
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Terrain") && GetComponent<Rigidbody>().velocity.magnitude > 0.5f)
        {
            int rand = Random.Range(0, sfxCollision.Length);
            audioSource[1].PlayOneShot(sfxCollision[rand]);
        }
    }
}

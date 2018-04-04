using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 3.
   Description : 로켓의 행동을 정의하는 스크립트
   Edit Log    : 
    - 로켓은 발사되어 죽는 순간까지 일을 열심히 합니다. 
   ================================================================= */

public class RocketBehaviour : MonoBehaviour
{
    [Header("Rocket Behaviour")]
    public float fDuration = 15.0f;
    public float fSpeed = 0.1f;

    float fShotTime;

	void Start ()
    {
        fShotTime = Time.time;
	}
	
	void FixedUpdate ()
    {
		if (Time.time - fShotTime > fDuration)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.forward * fSpeed);
	}

    void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject);
    }
}

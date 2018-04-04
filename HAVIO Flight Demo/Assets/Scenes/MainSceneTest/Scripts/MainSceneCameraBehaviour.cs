using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 3. 25.
   Description : 메인 메뉴 카메라의 회전을 담당하는 스크립트
   Edit Log    : 
    - 로그
   ================================================================= */

public class MainSceneCameraBehaviour : MonoBehaviour
{
    int nDirection = 0;

	void Start ()
    {
		
	}
	
	void FixedUpdate ()
    {
        if ((90.0f * nDirection) - 5.0f <= transform.rotation.eulerAngles.y && transform.rotation.eulerAngles.y <= (90.0f * nDirection) + 5.0f)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                nDirection++;
                if (nDirection >= 4) nDirection -= 4;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                nDirection--;
                if (nDirection < 0) nDirection += 4;
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 90.0f * nDirection, 0.0f), 0.0625f);
	}
}

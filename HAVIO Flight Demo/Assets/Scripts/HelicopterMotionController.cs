using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnoMotion;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 02. 27. 
   Description : 헬리콥터의 동작에 따른 모션을 재생하는 스크립트
   Edit Log    : 
    - 사용법: 
     * PlayStartEngine()는 HelicopterFlightController의 
       ToggleEngine() 내부에서밖에 사용되지 않습니다. 
   ================================================================= */

public class HelicopterMotionController : MonoBehaviour
{
    public TextAsset taVib5Sec;
    public TextAsset taVib1Sec;
    public TextAsset taCollision;

    MotionSource motionSource;

	void Start ()
    {
        motionSource = GetComponent<MotionSource>();
	}

    /// <summary>
    /// 엔진이 켜질 때 5초간 진동을 일으킵니다. 
    /// </summary>
    public void PlayStartEngine()
    {
        motionSource.LoadClip(taVib5Sec);
        motionSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 3. 28.
   Description : 헬리콥터에 장착되는 탐조등의 동작을 설정하는 스크립트
   Edit Log    : 
    - 헬리콥터의 탐조등에 관한 모든것을 다루게 됩니다. 
   ================================================================= */

public class SearchLightController : MonoBehaviour
{
    [Header("Main Camera")]
    public new Camera camera;

    public bool bLightOn = false;

    HelicopterFlightController hfc;
    AudioSource audioSource;

    void Start ()
    {
        hfc = GameObject.FindGameObjectWithTag("Player").GetComponent<HelicopterFlightController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (
            (hfc.bPlayWithJoystick && Input.GetButtonDown("SearchLightToggle")) ||
            (!hfc.bPlayWithJoystick && Input.GetKeyDown(KeyCode.E))
           )
        {
            audioSource.Play();
            bLightOn = !bLightOn;
        }
    }

    void FixedUpdate ()
    {
        SetLightOrientation();
        GetComponent<Light>().enabled = bLightOn;
    }

    /// <summary>
    /// 탐조등과 플레이어의 시야를 동기화합니다. 
    /// 단, 탐조등은 헬리콥터와 수평을 이루는 면 이상으로 올릴 수 없습니다. 
    /// 쉽게 표현하면, 회전가능공간이 하반구라고 할까요?
    /// </summary>
    void SetLightOrientation()
    {
        Vector3 v3CameraOrientation = camera.transform.localRotation.eulerAngles;

        if (v3CameraOrientation.x > 180.0f) v3CameraOrientation.x = 0.0f;
        else if (v3CameraOrientation.x < 0.0f) v3CameraOrientation.x = 180.0f;

        transform.localEulerAngles = v3CameraOrientation;
    }
}

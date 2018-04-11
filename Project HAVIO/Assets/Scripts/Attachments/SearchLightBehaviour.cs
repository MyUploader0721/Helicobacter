using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: SearchLightBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-07
 * DESCRIPTION: 헬리콥터의 탐조등의 행위를 정의한 스크립트
 *     DEV LOG: 
 *  - 새로운 스크립트에 적용되는 탐조등 스크립트입니다. 
 *  - 조종사의 시야에 따라 탐조등이 움직입니다. 다만 완전히 자유롭지는
 *    않고, 하반구 정도로만 움직이도록 고정되어있습니다. 
 */

public class SearchLightBehaviour : MonoBehaviour
{
    [SerializeField] new Camera camera;

    bool bLightOn = false;

    [SerializeField] GameObject objPlayer;
    HelicopterInfo helicopterInfo;
    AudioSource audioSource;

    void Start()
    {
        if (camera == null)
            camera = Camera.main;

        helicopterInfo = GetComponentInParent<HelicopterInfo>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (
            (helicopterInfo.bIsPlayWithGamePad && Input.GetButtonDown("SearchLightToggle")) ||
            (!helicopterInfo.bIsPlayWithGamePad && Input.GetKeyDown(KeyCode.E))
           )
        {
            audioSource.Play();
            bLightOn = !bLightOn;
        }
    }

    void FixedUpdate()
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

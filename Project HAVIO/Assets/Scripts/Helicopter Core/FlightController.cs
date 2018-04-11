﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: FlightController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-06
 * DESCRIPTION: 헬리콥터의 움직임을 담당하는 스크립트
 *     DEV LOG: 
 *  - 예전에 작성하던 스크립트가 완전히 재사용 불가능한 수준이었습니다. 
 *    (특정 헬리콥터에 종속적임) 따라서 재사용 가능한 스크립트 작성을 목표로 
 *    재작성하도록 하였습니다. 하...
 *  - InputController의 값으로 헬리콥터의 움직임(비행)을 다루게 됩니다. 
 */

public class FlightController : MonoBehaviour
{
    InputController inputController;
    HelicopterInfo helicopterInfo;

    new Rigidbody rigidbody;

    [Header("Helicopter Rotor Blades")]
    [SerializeField] GameObject objMainRotorBlade;
    [SerializeField] GameObject objTailRotorBlade;

    [Header("Helicopter Controllers")]
    [SerializeField] GameObject objCollectiveLever;
    [SerializeField] GameObject []objCycleControllers;

    [Header("Helicopter Speed Setting")]
    [SerializeField] float fCollectiveSpeed = 5.0f;
    [SerializeField] float fAntiTorqueSpeed = 1.5f;
    [SerializeField] float fCycleSpeed      = 2.0f;

    [HideInInspector] public float fVelocity = 0.0f;

    const float fVelocityLerpStep = 0.0625f;
    const float fTurbulenceSpeed  = 2.5f;

    AudioSource[] audioSource;
    enum SFX_List { HELICOPTER_FLIGHT = 0, TURBULENCE = 1 };
    [Header("SFX: Helicopter Flight Sound")]
    [SerializeField] AudioClip sfxHelicopterFlight;
    [SerializeField] AudioClip sfxHelicopterTurbulence;

    void Start ()
    {
        inputController = GetComponent<InputController>();
        helicopterInfo = GetComponent<HelicopterInfo>();

        rigidbody = GetComponent<Rigidbody>();

        audioSource = new AudioSource[2];

        audioSource[(int)SFX_List.HELICOPTER_FLIGHT] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.HELICOPTER_FLIGHT].clip = sfxHelicopterFlight;
        audioSource[(int)SFX_List.HELICOPTER_FLIGHT].loop = true;
        audioSource[(int)SFX_List.HELICOPTER_FLIGHT].pitch = 0.0f;
        audioSource[(int)SFX_List.HELICOPTER_FLIGHT].volume = 0.5f;
        audioSource[(int)SFX_List.HELICOPTER_FLIGHT].Play();

        audioSource[(int)SFX_List.TURBULENCE] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.TURBULENCE].clip = sfxHelicopterTurbulence;
        audioSource[(int)SFX_List.TURBULENCE].loop = true;
        audioSource[(int)SFX_List.TURBULENCE].volume = 0.0f;
        audioSource[(int)SFX_List.TURBULENCE].Play();
    }

	void Update ()
    {
        fVelocity = rigidbody.velocity.magnitude;

        if (helicopterInfo.bIsFlyable)
        {
            audioSource[(int)SFX_List.HELICOPTER_FLIGHT].pitch =
                inputController.fThrottle + (helicopterInfo.bIsEngineStart ? inputController.fCollective / 2.5f : 0.0f);
        }
        else
        {
            if (helicopterInfo.bIsEngineStart)
                inputController.ToggleEngine();
            audioSource[(int)SFX_List.HELICOPTER_FLIGHT].pitch = inputController.fThrottle;
        }

        if (fVelocity > fTurbulenceSpeed)
            audioSource[(int)SFX_List.TURBULENCE].volume = (fVelocity - fTurbulenceSpeed);
        else
            audioSource[(int)SFX_List.TURBULENCE].volume = 0.0f;
    }

    void FixedUpdate()
    {
        if (helicopterInfo.bIsEngineStart && helicopterInfo.bIsFlyable)
        {
            rigidbody.AddRelativeForce(Vector3.up * Physics.gravity.magnitude * inputController.fThrottle);

            ControlCollective();
            ControlAntiTorque();
            ControlCycle();

            float fUpReverseCosine = 1.0f - Vector3.Dot(Vector3.up, rigidbody.transform.up);
            rigidbody.velocity = Vector3.Lerp(
                rigidbody.velocity,
                rigidbody.velocity * (fUpReverseCosine > 1.0f ? 1.0f : fUpReverseCosine),
                fVelocityLerpStep
            );
        }

        RotateRotorBlades();
        RotateControllers();
    }

    void ControlCollective()
    {
        rigidbody.AddForce(rigidbody.transform.up * inputController.fCollective * fCollectiveSpeed);
    }
    void ControlAntiTorque()
    {
        rigidbody.angularVelocity = Vector3.Lerp(
            rigidbody.angularVelocity,
            rigidbody.transform.up * inputController.fAntiTorque * fAntiTorqueSpeed,
            fVelocityLerpStep
        );
    }
    void ControlCycle()
    {
        rigidbody.angularVelocity = Vector3.Lerp(
            rigidbody.angularVelocity,
            rigidbody.transform.right * inputController.fCyclePitch * fCycleSpeed,
            fVelocityLerpStep
        );

        rigidbody.angularVelocity = Vector3.Lerp(
            rigidbody.angularVelocity,
            -rigidbody.transform.forward * inputController.fCycleRoll * fCycleSpeed,
            fVelocityLerpStep
        );
    }

    void RotateRotorBlades()
    {
        objMainRotorBlade.transform.Rotate(
            -Vector3.up, 
            inputController.fThrottle * 10.0f + (helicopterInfo.bIsEngineStart ? inputController.fCollective : 0.0f));
        objTailRotorBlade.transform.Rotate(
            -Vector3.right, 
            inputController.fThrottle * 20.0f + (helicopterInfo.bIsEngineStart ? inputController.fAntiTorque : 0.0f));
    }

    void RotateControllers()
    {
        objCollectiveLever.transform.localRotation = Quaternion.Euler(-inputController.fCollective * 10.0f, 180.0f, 0.0f);

        Vector3 v3Cycle = new Vector3(inputController.fCyclePitch, 18.0f, -inputController.fCycleRoll);

        for (int i = 0; i < objCycleControllers.Length; i++)
        {
            objCycleControllers[i].transform.localEulerAngles = v3Cycle * 10.0f;
        }
    }
}

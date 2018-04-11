using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnoMotion;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 1. 16.
   Description : 헬리콥터의 전체적인 움직임을 담당하는 스크립트
   Edit Log    : 
    - 사용법: 
     * ToggleEngine(), Control-()함수들, Set-Rotation()함수들은
       해당 스크립트 내에서만 사용되는 함수입니다. 
     * 헬리콥터의 사이클 스틱의 방향을 알고 싶으면 GetCycleDirection()
     * 헬리콥터의 컬렉티브 레버의 값을 알고 싶으면 GetCollectiveValue()
    - Platform 열거형을 없애고 bPlayWithJoystick 부울 변수로 바꿨습니다
      어차피 선택이 키보드/마우스 또는 조이스틱밖에 없는걸요! - 18. 4. 2.
    - 로터의 회전을 HFC에서 다룰 수 있도록 수정했습니다. 새로 알게
      된 region 전처리기도 한번 사용해보았습니다. - 18. 4. 4.
   ================================================================= */

public class HelicopterFlightController : MonoBehaviour
{
    [Header("Helicopter Values")]
    public float fCollectiveVelocity = 1.0f;
    public float fCycleVelocity = 1.0f;
    public float fAntiTorqueVelocity = 1.0f;

    [Header("Helicopter Rotor Control")]
    public GameObject objMainRotor;
    public GameObject objTailRotor;
    float fRotorSpeed = 0.0f;
    float fMaxRotorSpeed = 15.0f;
    float fAccelarationInterval = 0.05f;

    [Header("Helicopter Control Sticks")]
    public GameObject[] objCycles;
    public GameObject objCollective;

    [Header("Platform Selector")]
    public bool bPlayWithJoystick = false;
    
    public bool bEngineStatus = false;
    public bool bAvailableFlight = true;

    float fThrottle = 0.0f;
    public float fCollective = 0.0f;
    float fAntiTorque = 0.0f;
    Vector3 v3CycleDir = Vector3.zero;

    public float fVelocity = 0.0f;
    public float fAltitude = 0.0f;

    const float PASSIVE_INPUT_STEP = 0.03125f;
    const float VELOCITY_LERP_STEP = 0.0625f;

    public Rigidbody rigidBody = null;
    MotionInput []motionInput;
    HelicopterSFXController sfxController;
    HelicopterMotionController motionController;

    float fUpReverseCosine = 0.0f;

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();

        motionInput = new MotionInput[2];
		motionInput[0] = GetComponent<MotionInput> ();
		motionInput[0].StartInput ();
        motionInput[1] = gameObject.AddComponent<MotionInput>();
        motionInput[1].UseAutoRotation = false;
        motionInput[1].UpdateMode = MotionInput.UpdateModeList.UserDirectCall;
        motionInput[1].StartInput();

        sfxController = GetComponent<HelicopterSFXController>();
        motionController = GetComponent<HelicopterMotionController>();
    }

    void Update()
    {
        if (
            (bPlayWithJoystick && Input.GetButtonDown("EngineToggle")) ||
            (!bPlayWithJoystick && Input.GetKeyDown(KeyCode.LeftShift))
           )
        {
            ToggleEngine();
        }

        MoveControllers();
    }

	void FixedUpdate ()
    {
        if (bAvailableFlight)
        {
            ControlCycle();

            if (bEngineStatus == true)
            {
                ControlCollective();
                ControlAntiTorque();
            }
        }
        else
        {
            motionInput[1].SetInputValues(0, 0, 0, 0, 0, 0);
        }

        //rigidBody.AddRelativeForce((v3UpForce + v3CycleDir * fCycleVelocity) * fThrottle);
        // 2018. 03. 24. - 사실적인 조종을 위해 기존 속도 기반 컬렉티브 제어를 힘 기반으로 변경합니다. 
        if (bAvailableFlight)
            rigidBody.AddForce(rigidBody.transform.up * Physics.gravity.magnitude * fThrottle);

        fUpReverseCosine = 1.0f - Vector3.Dot(Vector3.up, rigidBody.transform.up);
        rigidBody.velocity = Vector3.Lerp(
            rigidBody.velocity,
            rigidBody.velocity * (fUpReverseCosine > 1.0f ? 1.0f : fUpReverseCosine),
            VELOCITY_LERP_STEP
        );

        fVelocity = rigidBody.velocity.magnitude;

        ControlRotorBlades();
    }

    #region Engine/Throttle Control
    /// <summary>
    /// 엔진을 켜고 끕니다. FixedUpdate()에서 Key 입력으로 켜고 끌 수 있습니다. 
    /// </summary>
    void ToggleEngine()
    {
        bEngineStatus = !bEngineStatus;
        Debug.Log("Engine Status: " + bEngineStatus);

        if (bEngineStatus == true)
        {
            StopCoroutine(StopSpinRotorBlades());
            StartCoroutine(StartSpinRotorBlades());

            sfxController.PlayStartEngine();
            motionController.PlayStartEngine();

            StartCoroutine(SlightlyThrottleUp());
        }
        else
        {
            StopCoroutine(StartSpinRotorBlades());
            StartCoroutine(StopSpinRotorBlades());

            sfxController.PlayKillEngine();

            StartCoroutine(SlightlyThrottleDown());
        }
    }

    /// <summary>
    /// 엔진의 스로틀을 서서히 올립니다. ToggleEngine()에서 사용됩니다. 
    /// </summary>
    IEnumerator SlightlyThrottleUp()
    {
        while (fThrottle < 1.0f)
        {
            fThrottle += PASSIVE_INPUT_STEP;
            yield return new WaitForSeconds(0.1f);
        }

        fThrottle = 1.0f;
    }
    /// <summary>
    /// 엔진의 스로틀을 서서히 내립니다. ToggleEngine()에서 사용됩니다. 
    /// </summary>
    IEnumerator SlightlyThrottleDown()
    {
        while (fThrottle > 0.0f)
        {
            fThrottle -= PASSIVE_INPUT_STEP;
            yield return new WaitForSeconds(0.1f);
        }

        fThrottle = 0.0f;
    }
    #endregion

    #region Helicopter Control Methods
    /// <summary>
    /// 컬렉티브 레버를 조작합니다. 
    /// 엔진이 켜져 있을 경우 FixedUpdate()에서 입력을 받습니다. 
    /// <para>2018. 03. 24. - 사실적인 조종을 위해 기존 속도 기반 컬렉티브 제어를 힘 기반으로 변경합니다. </para>
    /// </summary>
    void ControlCollective()
    {
        if (bPlayWithJoystick)
            fCollective = Input.GetAxis("CollectiveVertical");
        else
            fCollective = Input.GetAxis("Vertical");

        //rigidBody.velocity = Vector3.Lerp(
        //    rigidBody.velocity,
        //    rigidBody.transform.up * fCollective * fCollectiveVelocity,
        //    VELOCITY_LERP_STEP
        //);

        motionInput[1].LinearValues.Heave = fCollective / 2.0f;

        rigidBody.AddForce(rigidBody.transform.up * fCollective * fCollectiveVelocity);
    }

    /// <summary>
    /// 안티 토크 페달을 조작합니다. 
    /// 엔진이 켜져 있을 경우 FixedUpdate()에서 입력을 받습니다. 
    /// </summary>
    void ControlAntiTorque()
    {
        if (bPlayWithJoystick)
            fAntiTorque = Input.GetAxis("AntiTorqueHorizontal");
        else
            fAntiTorque = Input.GetAxis("Horizontal");

        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            rigidBody.transform.up * fAntiTorque * fAntiTorqueVelocity, 
            VELOCITY_LERP_STEP
        );
    }

    /// <summary>
    /// 사이클 스틱을 조작합니다. 
    /// 엔진이 켜져 있을 경우 FixedUpdate()에서 입력을 받습니다. 
    /// </summary>
    void ControlCycle()
    {
        if (bPlayWithJoystick)
        {
            v3CycleDir.x = Input.GetAxis("CycleHorizontal");
            v3CycleDir.z = -Input.GetAxis("CycleVertical");
        }
        else
        {
            v3CycleDir.x = Input.GetAxis("KeyboardCycleHorizontal");
            v3CycleDir.z = Input.GetAxis("KeyboardCycleVertical");
        }

        // Pitch
        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            rigidBody.transform.right * v3CycleDir.z * fCycleVelocity, 
            VELOCITY_LERP_STEP
        );
        // Roll
        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            -rigidBody.transform.forward * v3CycleDir.x * fCycleVelocity, 
            VELOCITY_LERP_STEP
        );

        motionInput[1].RotationValues.Roll = v3CycleDir.x / 2.0f;
        motionInput[1].RotationValues.Pitch = v3CycleDir.z / 2.0f;
    }
    #endregion

    /// <summary>
    /// 조작의 사실감을 위해 조작장치들을 움직이도록 합니다. 
    /// </summary>
    void MoveControllers()
    {
        float fCycleRotationAngle = 10.0f;
        float fCollectiveRotationAngle = 10.0f;

        foreach (GameObject obj in objCycles)
        {
            obj.transform.localRotation = Quaternion.Euler(-v3CycleDir.z * fCycleRotationAngle, 0.0f, v3CycleDir.x * fCycleRotationAngle);
        }
        objCollective.transform.localRotation = Quaternion.AngleAxis(Input.GetAxis("Vertical") * fCollectiveRotationAngle, Vector3.right);
    }

    /// <summary>
    /// 헬리콥터가 추락하는 등 불시착하여 파괴될 때 사용합니다. 
    /// </summary>
    public void CrashHelicopter()
    {
        if (bEngineStatus == true)
            ToggleEngine();

        bEngineStatus = false;
        bAvailableFlight = false;
    }

    #region Rotating Helicopter Roter Blades
    /// <summary>
    /// 헬리콥터의 회전익을 회전하도록 만들어줍니다. 
    /// <para> - 2018. 4. 4. 추가</para>
    /// </summary>
    void ControlRotorBlades()
    {
        float fThrustMultiplier = 2.0f;
        objMainRotor.transform.Rotate(-Vector3.up, fRotorSpeed + (fCollective * fThrustMultiplier));
        objTailRotor.transform.Rotate(-Vector3.right, fRotorSpeed * 2.0f + (fAntiTorque * fThrustMultiplier));
    }
    IEnumerator StartSpinRotorBlades()
    {
        while (fRotorSpeed < fMaxRotorSpeed)
        {
            fRotorSpeed += 0.125f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }
    IEnumerator StopSpinRotorBlades()
    {
        while (fRotorSpeed > 0.0f)
        {
            fRotorSpeed -= 0.125f;
            if (fRotorSpeed < 0.0f)
                fRotorSpeed = 0.0f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }
    #endregion
}

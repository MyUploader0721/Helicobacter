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
   ================================================================= */

public class HelicopterFlightController : MonoBehaviour
{
    [Header("Helicopter Values")]
    public float fCollectiveVelocity = 1.0f;
    public float fCycleVelocity = 1.0f;
    public float fAntiTorqueVelocity = 1.0f;

    [Header("Helicopter Rotor Controls")]
    public GameObject objMainRotor;
    HelicopterRotorController hrcMainRotor;
    public GameObject objTailRotor;
    HelicopterRotorController hrcTailRotor;

    [Header("Helicopter Control Sticks")]
    public GameObject[] objCycles;
    public GameObject objCollective;

    [Header("Controller Rotation Values")]
    public float fCycleRotationAngle = 10.0f;
    public float fCollectiveRotationAngle = 10.0f;

    public enum Platform { Debug, Release }
    [Header("Platform Selector")]
    public Platform platform = Platform.Debug;

    Vector3 v3UpForce = Vector3.zero;
    public bool bEngineStatus = false;

    float fThrottle = 0.0f;
    public float fCollective = 0.0f;
    float fAntiTorque = 0.0f;
    Vector3 v3CycleDir = Vector3.zero;

    public float fVelocity = 0.0f;
    public float fAltitude = 0.0f;

    const float PASSIVE_INPUT_STEP = 0.03125f;
    const float VELOCITY_LERP_STEP = 0.0625f;

    public Rigidbody rigidBody = null;
    MotionInput motionInput;
    HelicopterSFXController sfxController;
    HelicopterMotionController motionController;

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        v3UpForce = -Physics.gravity * rigidBody.mass;

        hrcMainRotor = objMainRotor.GetComponent<HelicopterRotorController>();
        hrcTailRotor = objTailRotor.GetComponent<HelicopterRotorController>();

		motionInput = GetComponent<MotionInput> ();
		motionInput.StartInput ();

        sfxController = GetComponent<HelicopterSFXController>();
        motionController = GetComponent<HelicopterMotionController>();
    }
	
	void FixedUpdate ()
    {
        if (platform == Platform.Debug)
            if (Input.GetKeyDown(KeyCode.LeftShift))
                ToggleEngine();
        else if (platform == Platform.Release)
            if (Input.GetButtonDown("EngineToggle"))
                ToggleEngine();

        if (bEngineStatus == true)
        {
            ControlCollective();
            ControlAntiTorque();
            ControlCycle();
        }
        
        hrcMainRotor.GetThrottlePercent(fThrottle);
        hrcTailRotor.GetThrottlePercent(fAntiTorque);

        rigidBody.AddRelativeForce((v3UpForce + v3CycleDir) * fThrottle);

        fVelocity = rigidBody.velocity.magnitude;
    }

    /// <summary>
    /// 엔진을 켜고 끕니다. FixedUpdate()에서 Key 입력으로 켜고 끌 수 있습니다. 
    /// </summary>
    void ToggleEngine()
    {
        bEngineStatus = !bEngineStatus;
        Debug.Log("Engine Status: " + bEngineStatus);

        if (bEngineStatus == true)
        {
            hrcMainRotor.StartSpinning();
            hrcTailRotor.StartSpinning();

            sfxController.PlayStartEngine();
            motionController.PlayStartEngine();

            StartCoroutine(SlightlyThrottleUp());
        }
        else
        {
            hrcMainRotor.EndSpinning();
            hrcTailRotor.EndSpinning();

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

    /// <summary>
    /// 컬렉티브 레버를 조작합니다. 
    /// 엔진이 켜져 있을 경우 FixedUpdate()에서 입력을 받습니다. 
    /// </summary>
    void ControlCollective()
    {
        if (platform == Platform.Debug)
        {
            fCollective = Input.GetAxis("Vertical");
        }
        else if (platform == Platform.Release)
        {
            fCollective = Input.GetAxis("CollectiveVertical");
        }

        rigidBody.velocity = Vector3.Lerp(
            rigidBody.velocity,
            rigidBody.transform.up * fCollective * fCollectiveVelocity,
            VELOCITY_LERP_STEP
        );

        SetCycleStickRotation();
        SetCollectiveLeverRotation();
    }

    /// <summary>
    /// 안티 토크 페달을 조작합니다. 
    /// 엔진이 켜져 있을 경우 FixedUpdate()에서 입력을 받습니다. 
    /// </summary>
    void ControlAntiTorque()
    {
        if (platform == Platform.Debug)
            fAntiTorque = Input.GetAxis("Horizontal");
        else if (platform == Platform.Release)
            fAntiTorque = Input.GetAxis("AntiTorqueHorizontal");

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
        if (platform == Platform.Debug)
        {
            // U for Pitch Down
            if (Input.GetKey(KeyCode.U))
            {
                if (v3CycleDir.z < 1.0f)
                    v3CycleDir.z += PASSIVE_INPUT_STEP;
            }
            else
            {
                if (v3CycleDir.z > 0.0f)
                    v3CycleDir.z -= PASSIVE_INPUT_STEP;
            }

            // J for Pitch Up
            if (Input.GetKey(KeyCode.J))
            {
                if (v3CycleDir.z > -1.0f)
                    v3CycleDir.z -= PASSIVE_INPUT_STEP;
            }
            else
            {
                if (v3CycleDir.z < 0.0f)
                    v3CycleDir.z += PASSIVE_INPUT_STEP;
            }

            // K for Roll Right
            if (Input.GetKey(KeyCode.K))
            {
                if (v3CycleDir.x < 1.0f)
                    v3CycleDir.x += PASSIVE_INPUT_STEP;
            }
            else
            {
                if (v3CycleDir.x > 0.0f)
                    v3CycleDir.x -= PASSIVE_INPUT_STEP;
            }

            // H for Roll Left
            if (Input.GetKey(KeyCode.H))
            {
                if (v3CycleDir.x > -1.0f)
                    v3CycleDir.x -= PASSIVE_INPUT_STEP;
            }
            else
            {
                if (v3CycleDir.x < 0.0f)
                    v3CycleDir.x += PASSIVE_INPUT_STEP;
            }
        }
        else if (platform == Platform.Release)
        {
            v3CycleDir.x = Input.GetAxis("CycleHorizontal");
            v3CycleDir.z = -Input.GetAxis("CycleVertical");
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
    }

    /// <summary>
    /// 조작의 사실감을 위해 사이클 스틱을 움직이도록 합니다. 
    /// </summary>
    void SetCycleStickRotation()
    {
        foreach (GameObject obj in objCycles)
        {
            obj.transform.localRotation = Quaternion.Euler(-v3CycleDir.z * fCycleRotationAngle, 0.0f, v3CycleDir.x * fCycleRotationAngle);
        }
    }
    /// <summary>
    /// 조작의 사실감을 위해 컬렉티브 레버를 움직이도록 합니다. 
    /// </summary>
    void SetCollectiveLeverRotation()
    {
        objCollective.transform.localRotation = Quaternion.AngleAxis(fCollective * fCollectiveRotationAngle, Vector3.right);
    }

    /// <summary>
    /// 사이클 스틱의 방향을 얻는 함수입니다. 
    /// 다른 스크립트에서 값을 참조할 때 사용합니다. 
    /// </summary>
    /// <returns>사이클 스틱의 방향</returns>
    public Vector3 GetCycleDirection() { return v3CycleDir; }
    /// <summary>
    /// 컬렉티브 레버의 값을 얻는 함수입니다. 
    /// 다른 스크립트에서 값을 참조할 때 사용합니다. 
    /// </summary>
    /// <returns>컬렉티브 레버의 값</returns>
    public float  GetCollectiveValue() { return fCollective; }
}

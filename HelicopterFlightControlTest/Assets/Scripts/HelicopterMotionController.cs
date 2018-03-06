using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayStartEngine()
    {
        motionSource.LoadClip(taVib5Sec);
        motionSource.Play();
    }
}

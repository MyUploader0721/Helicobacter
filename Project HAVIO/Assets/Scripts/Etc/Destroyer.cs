using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] float fTimer;

	void Start ()
    {
        Destroy(gameObject, fTimer);
	}
}

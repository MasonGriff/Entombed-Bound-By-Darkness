using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleFore : MonoBehaviour {

    public float RotationRate = 5f;
    float RotationPassed;

    Quaternion OriginalRotation;

	// Use this for initialization
	void Start () {
        RotationPassed = 0;
        OriginalRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
       /* RotationPassed -= Time.deltaTime;

        if (RotationPassed <= 0)
        {
            RotationPassed = RotationTime;
            transform.rotation = OriginalRotation;
            transform.DORotate(new Vector3(0, 360, 0), RotationTime);
        }
        */
	}
    private void FixedUpdate()
    {
        if(transform.rotation.y >= 360)
        {
            transform.rotation = OriginalRotation;
        }
        transform.Rotate(0, RotationRate, 0);
    }
}

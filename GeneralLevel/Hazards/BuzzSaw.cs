using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuzzSaw : MonoBehaviour {

    public enum SawBladeOrientation { Horizontal, Vertical}


    public SawBladeOrientation MyBladeOrientation = SawBladeOrientation.Horizontal;
    public GameObject Sawblade;
    public GameObject SawbladeCenter;
    public Transform EndpointA, EndpointB;
    public float MoveSpeedTime = 5f;
    public float OccilationRate = 20f;

    public float MoveTimePassed;
    //public float RotateTimePassed;

    public int RotationSequence = 0;

    public int MoveSequence = 0;

    public bool MoveToNext;

    Quaternion DefaultRotation;

	// Use this for initialization
	void Start () {
        DefaultRotation = SawbladeCenter.transform.rotation;
        MoveSequence = 0;
        RotationSequence = 0;
        Sawblade.transform.position = EndpointA.position;
        MoveToNext = true;
        Sawblade.transform.DOMove(EndpointB.position, MoveSpeedTime).SetEase(Ease.InOutQuint);
        MoveTimePassed = MoveSpeedTime;
        
       // RotateTimePassed = RotationTime;
    }
	
	// Update is called once per frame
	void Update () {
        MoveTimePassed -= Time.deltaTime;
        //RotateTimePassed -= Time.deltaTime;
	}
    private void FixedUpdate()
    {
        //rotation
        switch (MyBladeOrientation)
        {
            case SawBladeOrientation.Horizontal:
                SawbladeCenter.transform.Rotate(0, OccilationRate, 0);
                break;
            case SawBladeOrientation.Vertical:
                SawbladeCenter.transform.Rotate(OccilationRate, 0, 90);
                break;
        }

        //Movement
        if (MoveToNext && MoveTimePassed <= 0)
        {
            MoveToNext = false;
            Sawblade.transform.DOMove(EndpointA.position, MoveSpeedTime).SetEase(Ease.InOutQuint);
            MoveTimePassed = MoveSpeedTime;
        }
        else if (!MoveToNext && MoveTimePassed <= 0)
        {
            MoveToNext = true;
            Sawblade.transform.DOMove(EndpointB.position, MoveSpeedTime).SetEase(Ease.InOutQuint);
            MoveTimePassed = MoveSpeedTime;
        }
    }

    void SetOffMoveSequence(bool ToB)
    {
        Sawblade.transform.DOMove(ToB ? EndpointB.position : EndpointA.position, MoveSpeedTime).SetEase(Ease.InOutQuint);
    }

}

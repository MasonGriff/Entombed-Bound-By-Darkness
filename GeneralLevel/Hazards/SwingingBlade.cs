using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwingingBlade : MonoBehaviour {
    public enum FacingDir { FacingLeft, RacingRight}

    [Tooltip("Which position will the Pendulum start at when the scene first loads?")]
    public FacingDir StartPosition = FacingDir.FacingLeft;


    public int SwingSequence = 0;

    [Tooltip("Model of the pendulum.")]
    public GameObject PendulumModel;
    [Tooltip("How long is it swinging?")]
    public float SwingTime = 0.5f;
    float SwingTimer;
    [Tooltip("Cooldown between swings.")]
    public float WaitTimer;


    [Tooltip("Reference for where the pendulum's leftmost position and rightmost positions are.")]
    public Transform LeftPosTransform, RightPosTransform;

    [Tooltip("Vector 3 of positions. Will be set automatically based on the above transforms.")]
    public Vector3 LeftPos, RightPos;

	// Use this for initialization
	void Start () {
        LeftPos = LeftPosTransform.eulerAngles;
        RightPos = RightPosTransform.eulerAngles;

        SwingTimer = 0;

        PendulumModel.transform.rotation = ((StartPosition == FacingDir.FacingLeft) ? LeftPosTransform.rotation : RightPosTransform.rotation);

        SwingSequence = ((StartPosition == FacingDir.FacingLeft) ? 0 : 3);

	}
	
	// Update is called once per frame
	void Update () {
        SwingTimer -= Time.deltaTime;
        switch (SwingSequence)
        {
            case 0:
                PendulumModel.transform.DOLocalRotate(RightPos, SwingTime).SetEase(Ease.InOutSine);
                SwingTimer = SwingTime;
                SwingSequence = 1;
                break;
            case 1:
                if (SwingTimer <= 0)
                {
                    SwingSequence = 2;
                    SwingTimer = WaitTimer;
                }
                break;
            case 2:
                if (SwingTimer <= 0)
                {
                    SwingSequence = 3;
                }
                break;
            case 3:
                PendulumModel.transform.DOLocalRotate(LeftPos, SwingTime).SetEase(Ease.InOutQuad);
                SwingTimer = SwingTime;
                SwingSequence = 4;
                break;
            case 4:
                if (SwingTimer <= 0)
                {
                    SwingSequence = 5;
                    SwingTimer = WaitTimer;
                }
                break;
            case 5:

                if (SwingTimer <= 0)
                {
                    SwingSequence = 0;
                }
                break;
        }

	}
}

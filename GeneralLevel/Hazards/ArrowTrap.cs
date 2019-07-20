using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour {

    

    Arrow ArrowScript;

    public Player_States.PlatformDirection ArrowDirection;

    public GameObject ArrowProjectile;
    GameObject firedArrow;
    public GameObject baseObject;
    public Transform muzzlePoint;
    public GameObject attachedWall;
    public bool fireIt = false;

    Quaternion OriginalRotation;
    [Header("Arrow Specific")]
    public bool ControlArrowSpeedFromHere = false;
    public float arrowSpeed = 2.5f;
    public float RangeAwayFromPlayerToDeathHorizontal = 24;
    public float RangeAwayFromPlayerToDeathVertical = 15;


    [HideInInspector]
    public Collider2D myColl;

	// Use this for initialization
	void Start () {
        OriginalRotation = baseObject.transform.rotation;
        myColl = GetComponent<Collider2D>();
        fireIt = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (fireIt)
        {
            fireIt = false;
            FireTheArrow();
        }
    }
    private void FixedUpdate()
    {
        
    }

    void FireTheArrow()
    {

        OriginalRotation = baseObject.transform.rotation;
        firedArrow = GameObject.Instantiate(ArrowProjectile, muzzlePoint.position, OriginalRotation);
       //firedArrow.GetComponent<Arrow>().ShotDirection = ArrowDirection;
        //ArrowScript = firedArrow.GetComponent<Arrow>();
        //Collider2D wallColl = attachedWall.GetComponent<Collider2D>();
        //Physics2D.IgnoreCollision(wallColl, ArrowScript.myColl);
        //ArrowScript.OffscreenCheckDistance = RangeAwayFromPlayerToDeathHorizontal;
        //ArrowScript.offscreenCheckHeight = RangeAwayFromPlayerToDeathVertical;
        //ArrowScript.ShotDirection = ArrowDirection;
        //ArrowScript.DeterminArrowVelocity(ArrowDirection);
        //if (ControlArrowSpeedFromHere)
        //{
        //    ArrowScript.ArrowSpeed = arrowSpeed;
        //}
        //ArrowScript.OriginPoint = muzzlePoint.gameObject;

        //ArrowScript.BeginMySequence = true;
    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            fireIt = true;
        }
    }

}

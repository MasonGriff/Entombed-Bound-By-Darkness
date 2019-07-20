using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTrap : MonoBehaviour {

    //string TypeTooltip = ;
    [Tooltip("Proximity checks if the player is in the collider, then activates in the specified time period and deactivates in a separate specified time period."+"\n" + "\n" + "Pattern has the laser beam go off for a set time, turn on for a set time, then repeat. This is regardless of if the player is in the proximity.")]
    public Player_States.LaserTripTypes LaserType;

    public GameObject PlayerObj;

    public GameObject LaserObj, LaserEnd1, LaserEnd2;

    //public bool LaserTripped = false;
    public bool LaserActive = false;

    float DelayTimer = 0;

    [Header("Proximity Variables")]
    public float DelayToLaserActivation = 1.5f;
    public float LaserDeactivationTime = .5f;
    public bool LaserTripped = false;
    bool LaserTimerStarted = false;
    bool ProxyDeactivationStarted = false;
    bool ProxyDeactivateFromTrigger = false;

    [Header("Pattern Variables")]
    public float LaserOffTime = 3f;
    public float LaserOnTime = 3f;
    public int StagesOfLaser = 0;

	// Use this for initialization
	void Start () {
        LaserTripped = false;
        LaserActive = false;
        LaserObj.SetActive(false);
        DelayTimer = 0;
        LaserTimerStarted = false;
        ProxyDeactivationStarted = false;
        ProxyDeactivateFromTrigger = false;
        StagesOfLaser = 0;
	}
	
	// Update is called once per frame
	void Update () {
        DelayTimer -= Time.deltaTime;

		switch (LaserType)
        {
            //Laser activates on player coming close.
            case Player_States.LaserTripTypes.Proximity:
                if (LaserTripped && !LaserTimerStarted)
                {
                    LaserTimerStarted = true;
                    DelayTimer = DelayToLaserActivation;
                    
                }
                if(LaserTripped && LaserTimerStarted)
                {
                    if (DelayTimer <= 0)
                    {
                        LaserActive = true;
                        LaserObj.SetActive(true);
                    }
                }
                if (LaserTripped && LaserTimerStarted && LaserActive)
                {
                    if (PlayerObj == null && !ProxyDeactivationStarted && !ProxyDeactivateFromTrigger)
                    {
                        ProxyDeactivationStarted = true;
                        ResetLaserProximity();
                        //Invoke("ResetLaserProximity", LaserDeactivationTime);
                    }
                }
                break;
            //Laser goes off and on every so-and-so seconds.
            case Player_States.LaserTripTypes.Pattern:
                switch (StagesOfLaser)
                {
                    case 0:
                        StagesOfLaser = 1;
                        DelayTimer = LaserOffTime;
                        break;
                    case 1:
                        if (DelayTimer <= 0)
                        {
                            LaserActive = true;
                            LaserObj.SetActive(true);
                            StagesOfLaser = 2;
                            DelayTimer = LaserOnTime;
                        }
                        break;
                    case 2:
                        if (DelayTimer <= 0)
                        {
                            StagesOfLaser = 0;
                            LaserActive = false;
                            LaserObj.SetActive(false);
                        }
                        break;
                }



                break;

            //If something happens where there's no laser type selected.
            default:
                LaserType = Player_States.LaserTripTypes.Pattern;
                break;
        }
	}

    void ResetLaserProximity()
    {
        LaserTripped = false;
        LaserTimerStarted = false;
        LaserActive = false;
        ProxyDeactivateFromTrigger = false;
        ProxyDeactivationStarted = false;
        LaserObj.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D myTrigg)
    {
        if (LaserType == Player_States.LaserTripTypes.Proximity)
        {
            if (myTrigg.tag == "Player" && !LaserTripped)
            {
                PlayerObj = myTrigg.gameObject;
                LaserTripped = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D LeaveTrigg)
    {
        if (LaserType == Player_States.LaserTripTypes.Proximity)
        {
            if (LeaveTrigg.tag == "Player")
            {
                ProxyDeactivateFromTrigger = true;
                PlayerObj = null;
                Invoke("ResetLaserProximity", LaserDeactivationTime);
            }
        }
    }
}

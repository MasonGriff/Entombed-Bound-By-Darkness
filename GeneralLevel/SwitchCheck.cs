using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCheck : MonoBehaviour {

    /// <summary>
    /// Generic script for any environmental triggers.
    /// </summary>

    [Tooltip("This bool switches on. Whatever the script for the switch's properties is, they'll get the info that the bool is active and do their thing.")]
    public bool SwitchOn = false;
    [Tooltip("Set this to the type of detection you want for the switch.")]
    public Player_States.SwitchCheckFor CheckFor;




	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnTriggerEnter)
        {
            if (myTrigg.tag == "Player")
            {
                SwitchOn = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnTriggerStay)
        {
            if (myTrigg.tag == "Player")
            {
                SwitchOn = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnTriggerExit && myTrigg.tag == "Player")
        {
                SwitchOn = true;
        }
        else
        {
            SwitchOn = false;
        }
    }
    private void OnCollisionExit2D(Collision2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnCollisionExit && myTrigg.gameObject.tag == "Player")
        {
                SwitchOn = true;
        }
        else
        {
            SwitchOn = false;
        }

    }

    private void OnCollisionStay2D(Collision2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnCollisonStay)
        {
            if (myTrigg.gameObject.tag == "Player")
            {
                SwitchOn = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D myTrigg)
    {
        if (CheckFor == Player_States.SwitchCheckFor.OnCollisionEnter)
        {
            if (myTrigg.gameObject.tag == "Player")
            {
                SwitchOn = true;
            }
        }
    }
}

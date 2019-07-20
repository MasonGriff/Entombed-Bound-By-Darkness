using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallDeath : MonoBehaviour {

    //PlayerController playCon;
    GameObject player;
    GameObject meObj;
    Player_Main PlayerCon;

	// Use this for initialization
	void Start () {
        meObj = this.gameObject;
	}

    // Update is called once per frame
    /*void Update () {
		
	}*/

    void OnTriggerEnter2D(Collider2D daCollBwoy)
    {  
        if (daCollBwoy.tag == "Player")
        {
            Debug.Log("fall to death");
            player = daCollBwoy.gameObject;
            PlayerCon = player.GetComponent<Player_Main>();
            PlayerCon.HealthScript.HP = 0;
            PlayerCon.PlayerHealthState = Player_States.PlayerHealthStates.Dead;
            //PlayerCon.DeathSetup();
            //meObj.SetActive(false);
        }
    }
}

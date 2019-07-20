using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamScript : MonoBehaviour {

    public int Damage = 5;

	/*// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/


    private void OnTriggerStay2D(Collider2D myColls)
    {
        if (myColls.tag == "Player")
        {
            Player_Main Plyr = myColls.gameObject.GetComponent<Player_Main>();
            Plyr.DamageTaken(Damage);
            //Plyr.DeathSetup();
        }
    }
}

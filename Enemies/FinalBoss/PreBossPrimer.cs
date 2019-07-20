using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreBossPrimer : MonoBehaviour {

    public PreFinalBossCutscene BossCutscene;

    /*// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            BossCutscene.EventsSequence = 0;
        }
    }
}

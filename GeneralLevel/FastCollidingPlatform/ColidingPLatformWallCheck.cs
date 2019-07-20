using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColidingPLatformWallCheck : MonoBehaviour {

    FastCollidingPlatform HeadScript;


	// Use this for initialization
	void Start () {
        HeadScript = transform.parent.GetComponent<FastCollidingPlatform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D myTrigg)
    {
        if (myTrigg.transform.tag == "Wall")
        {
            HeadScript.HitWall = true;
        }
    }
}

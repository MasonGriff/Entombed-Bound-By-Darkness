using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WallTouchCollisionCheck : MonoBehaviour {

    [Header("Player Script References")]
    public Player_Main Player;

    [Header("Wall Touching Object Reference")]
    public GameObject TouchedWall;
    //public GameObject FastMovingTouchedWall;
	// Use this for initialization
	/*void Start () {
		
	}*/
	
	// Update is called once per frame
	/*void Update () {
		
	}*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "FastWall")
        {
            Player.touchingWall = true;
            Debug.Log("Touching Wall");
            TouchedWall = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "FastWall")
        {
            Player.touchingWall = true;
            TouchedWall = collision.gameObject;

        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "FastWall")
        {
            Player.touchingWall = false;
            Debug.Log("Not on wall anymore");
            TouchedWall = null;
        }
    }

}

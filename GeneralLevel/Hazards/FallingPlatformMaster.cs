using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FallingPlatformMaster : MonoBehaviour {

    public GameObject PlatformChild;
    public FallingPlatform PlatformScript;

    public bool FallHasBegun = false;

    public Vector3 OriginalPosition;
    public float OffscreenCheckDistance = 30;
    public float offscreenCheckHeight = 10;
    public bool RespawnCheckBool;
    bool timerStarted = false;
    public float RespawnTimer = 2f;
    public float RespawnCheck;

    public static bool PlayerDead = false;

    //[HideInInspector]
    public GameObject PlayerObj;
   

    // Use this for initialization
    void Start () {
        OriginalPosition = PlatformChild.transform.position;
        PlatformScript = PlatformChild.GetComponent<FallingPlatform>();
        RespawnCheck = 0;
        RespawnCheckBool = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (RespawnCheckBool && FallHasBegun)
        {
            switch (RespawnCheck <= 0)
            {
                case false:
                    RespawnCheck -= Time.deltaTime;
                    break;
                case true:
                    RespawnCheckBool = false;
                    timerStarted = false;
                    RespawnCheck = 0;
                    RespawnPlatform();
                    break;
            }
        }

        //if (!FallHasBegun)
        //{
        //    if (PlatformChild != null)
        //    {

        //        PlatformChild.transform.position = OriginalPosition;
        //    }
        //}
        if (LevelGM.Instance.playDeath)
        {
            RespawnPlatform();
        }
        
	}

    private void FixedUpdate()
    {
        PlayerDead = LevelGM.Instance.playDeath;
        FindThePlayer();
        if (PlayerDead)
        {
            RespawnCheckBool = false;
            timerStarted = false;
            RespawnCheck = 0;
            PlayerDead = false;
            //PlayerObj = null;
            RespawnPlatform();
        }
        if (!FallHasBegun)
        {
            if (PlatformChild != null)
            {

                PlatformChild.transform.position = OriginalPosition;
            }
        }
        else
        {
            if (PlayerObj == null && PlatformChild.activeSelf == false)
            {
                RespawnCheckBool = true;
                if (!timerStarted)
                {
                    RespawnCheck = RespawnTimer;
                }
                timerStarted = true;
            }
        }
    }

    void RespawnPlatform()
    {

        PlatformScript.myRB.gravityScale = 0;
        FallHasBegun = false;

        PlatformScript.myRB.gravityScale = 0;
        PlatformScript.myDoMoveSequence.Kill();
        PlatformChild.transform.position = OriginalPosition;
        PlatformChild.SetActive(true);
        PlatformScript.myColl.enabled=true;
        PlatformChild.transform.position = OriginalPosition;
        PlatformScript.ResetTheStuff();
    }
    public GameObject FindThePlayer()
    {
        GameObject[] Players;
        Players = GameObject.FindGameObjectsWithTag("Player");
        PlayerObj = null;
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject player in Players)
        {
            Vector3 diff = player.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if ((player.transform.position.x <= (transform.position.x + OffscreenCheckDistance)) && (player.transform.position.x >= (transform.position.x - OffscreenCheckDistance))) //Checks if the nearest perch point is in range.
            {
                if ((player.transform.position.y <= (transform.position.y + offscreenCheckHeight)) && (player.transform.position.y >= (transform.position.y - offscreenCheckHeight)))
                {
                    PlayerObj = player;
                    distance = curDist;
                }
            }
        }



        return PlayerObj;
    }

    public static void AutoRecallPlatformSpawn()
    {
        PlayerDead = true;
    }
}

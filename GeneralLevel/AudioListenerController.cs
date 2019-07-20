using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerController : MonoBehaviour {

    public Transform targetPosition;
    GameObject playerObj;
    Player_Main plyr;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (LevelGM.Instance.PlayerInLevel != null)
        {
            playerObj = LevelGM.Instance.PlayerInLevel;
            plyr = playerObj.GetComponent<Player_Main>();
            targetPosition = ((plyr.HeadBonePosition != null) ? plyr.HeadBonePosition : playerObj.transform);
        }
        else
        {
            targetPosition = LevelGM.Instance.CurrentCheckpoint;
        }


        transform.position = targetPosition.position;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSet : MonoBehaviour {

    public Transform CheckpointRespawnSpot;

    public AudioSource SfxPlay;

	// Use this for initialization
	void Start () {
		if (!(Game.current.Progress.CurrentDifficulty == Player_States.DifficultySetting.Easy))
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		

	}

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if(myTrigg.gameObject.tag == "Player")
        {
            Debug.Log("Player entered checkpoint");
            //Sets the default minimum and maximum values of the camera position to these. If the player dies and they touched an event 
            //that changes the mins and maxes but haven't reached a checkpoint, this will reset the mins and maxes to what they were 
            //at this checkpoint
            if (LevelGM.Instance.CurrentCheckpoint != CheckpointRespawnSpot)
            {
                SfxPlay.Play();
            }
            LevelGM.Instance.DefaultHoriMaximum = CameraController.Instance.HoriMaximum;
            LevelGM.Instance.DefaultHoriMinimum = CameraController.Instance.HoriMinimum;
            LevelGM.Instance.DefaultVertMaximum = CameraController.Instance.VertMaximum;
            LevelGM.Instance.DefaultVertMinimum = CameraController.Instance.VertMinimum;

            LevelGM.Instance.CurrentCheckpoint = CheckpointRespawnSpot;
            Debug.Log("New Checkpoint Set");

            
        }
    }
}

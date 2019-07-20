using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour {

    /// <summary>
    /// It's a camera controller.
    /// </summary>

    public GameObject ClearMindGhostSpawn;

    public static CameraController Instance { get; set; } //Sets current instance of the camera controller to this gameobject. 
    //You can reference this script without getting needing to link to the object via CameraController.Instance

    public float CameraFollowDelay = .2f;

    //Reference
    GameObject MainCam; //The game object of the camera.
    public GameObject Player; //The gameobject of the current player prefab in the level.
    public float CameraZPos; //the Z position of the camera that never changes.
     
    public bool lockCamera = false; //Lock the Camera in place for something.

    //Position Min/Max
    [Tooltip("The minimim vertical position that the camera will follow the player to.")]
    public Vector3 VertMinimum;
    [Tooltip("The maximim vertical position that the camera will follow the player to.")]
    public Vector3 VertMaximum;
    [Tooltip("The minimim horizontal position that the camera will follow the player to.")]
    public Vector3 HoriMinimum;
    [Tooltip("The maximim horizontal position that the camera will follow the player to.")]
    public Vector3 HoriMaximum;
    

    private void Awake()
    {
        Instance = this; //Sets the instance of the script to this gameobject.
    }

    // Use this for initialization
    void Start () {
        //Instance = this;
        MainCam = this.gameObject;
        //Player = GameObject.Find("Zahra");
        CameraZPos = transform.position.z; //Gets the Z position of the camera at the scene start and sets the non-changing Zpos to it.
        AudioListener myEars = GetComponent<AudioListener>();
        if (myEars != null)
        {
            myEars.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        /*if (Player.transform.position.x >= GM.CamStart.transform.position.x && Player.transform.position.x <= GM.CamEnd.transform.position.x) //if player is not near the edges of the room.
            {
                Vector3 positionToPlayer = transform.position;
                positionToPlayer.x = Player.transform.position.x;
                transform.position = positionToPlayer; //camera tracks the player
            }*/


        if (!lockCamera && Player!=null) //If Camera isn't locked and there is a player prefab detected in the scene by the camera.
        {
            //horizontal camera controls
            if (Player.transform.position.x > HoriMinimum.x && Player.transform.position.x < HoriMaximum.x)
            {
                Vector3 positionToPlayer = transform.position;
                positionToPlayer.x = Player.transform.position.x;
                //transform.position = positionToPlayer;
                transform.DOMoveX(positionToPlayer.x, CameraFollowDelay);
            }
            else if (Player.transform.position.x <= HoriMinimum.x)
            {
                Vector3 positionToHoriMin = transform.position;
                positionToHoriMin.x = HoriMinimum.x;
                //MainCam.transform.position = positionToHoriMin;
                /*MainCam.*/
                transform.DOMoveX(positionToHoriMin.x, CameraFollowDelay);
            }
            else if (Player.transform.position.x >= HoriMaximum.x)
            {
                Vector3 positionToHoriMax = transform.position;
                positionToHoriMax.x = HoriMaximum.x;
                //transform.position = positionToHoriMax;
                /*MainCam.*/
                transform.DOMoveX(positionToHoriMax.x, CameraFollowDelay);
            }

            //vertical camera controls
            if (Player.transform.position.y > VertMinimum.y && Player.transform.position.y < VertMaximum.y)
            {
                Vector3 positionToVert = transform.position;
                positionToVert.y = Player.transform.position.y;
                //transform.position = positionToVert;
                transform.DOMoveY(positionToVert.y, CameraFollowDelay);
            }
            else if (Player.transform.position.y <= VertMinimum.y)
            {
                Vector3 positionToVertMin = transform.position;
                positionToVertMin.y = VertMinimum.y;
                //transform.position = positionToVertMin;
                transform.DOMoveY(positionToVertMin.y, CameraFollowDelay);
            }
            else if (Player.transform.position.y >= VertMaximum.y)
            {
                Vector3 positionToVertMax = transform.position;
                positionToVertMax.y = VertMaximum.y;
                //transform.position = positionToVertMax;
                transform.DOMoveY(positionToVertMax.y, CameraFollowDelay);
            }
        }

        //maintain same z controls
        Vector3 positionToZ = transform.position;
        positionToZ.z = CameraZPos;
        transform.position = positionToZ;
    }
}

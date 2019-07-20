using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVectorChange : MonoBehaviour {

    /// <summary>
    /// This script is for the objects that change the minimum and maximum positions the camera will follow the player.
    /// </summary>

    public GameObject ThisSwitch;
    SwitchCheck switchCheckScript;
    public GameObject CamObj;
    public CameraController Cam;

    public static GameObject CameraChange;

    //Read the tooltips for the similar named variables in the CameraController script
    public Vector3 newVertMinimum;
    public Vector3 newVertMaximum;
    public Vector3 newHoriMinimum;
    public Vector3 newHoriMaximum;
    
    // Use this for initialization
    void Start () {
        CamObj = GameObject.Find("Main Camera");
        //Cam = CamObj.GetComponent<CameraController>();
        Cam = CameraController.Instance;
        ThisSwitch = this.gameObject;
        switchCheckScript = ThisSwitch.GetComponent<SwitchCheck>();
	}
	
	// Update is called once per frame
	void Update () {
		if (switchCheckScript.SwitchOn == true && CameraChange != this.gameObject) //Checks if this was most recently hit camera switch point.
        {
            CameraChange = this.gameObject;
            switchCheckScript.SwitchOn = false;
            Cam.VertMinimum = newVertMinimum;
            Cam.VertMaximum = newVertMaximum;
            Cam.HoriMaximum = newHoriMaximum;
            Cam.HoriMinimum = newHoriMinimum;
        }
	}


    public static void ClearCameraChange()
    {
        CameraChange = null;
    }
}

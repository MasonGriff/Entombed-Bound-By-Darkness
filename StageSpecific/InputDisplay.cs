using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDisplay : MonoBehaviour {

    PlayerController Controls;

    public GameObject[] InputHelp;

    // Use this for initialization
    void Start () {

        Controls = PlayerController.CreateWithDefaultBindings();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;
        switch (LastDeviceUsed)
        {
            case "PlayStation4":
                DisplayButtonsTypes(1);
                break;
            case "XboxOne":
                DisplayButtonsTypes(2);
                break;
            default:
                DisplayButtonsTypes(0);
                break;
        }
    }

    void DisplayButtonsTypes(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputHelp)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputHelp[ControllerInputType].SetActive(true);
    }
}

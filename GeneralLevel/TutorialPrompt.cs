using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPrompt : MonoBehaviour {


    public bool PromptTriggered = false;

    PlayerController Controls;
    public GameObject[] InputHelp;
    public Image TextBox;
    public GameObject ButtonsMain, textMain;
    public float FadeInIncrement = .1f;

    // Use this for initialization
    void Start () {
        ButtonsMain.SetActive(false);
        textMain.SetActive(false);
        Color newColour = TextBox.color;
        newColour.a = 0;
        TextBox.color = newColour;

        Controls = PlayerController.CreateWithDefaultBindings();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;
        //Debug.Log(LastDeviceUsed);
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

        Color newColour = TextBox.color;
        switch (PromptTriggered)
        {
            case true:
                /*if (TextBox.color.a != 1)
                {
                    newColour.a += FadeInIncrement;
                    TextBox.color = newColour;
                }*/
                newColour.a = 1;
                TextBox.color = newColour;
                break;
            case false:

                /*newColour.a -= FadeInIncrement;
                TextBox.color = newColour;*/

                newColour.a = 0;
                TextBox.color = newColour;
                break;
        }

        if (TextBox.color.a >= 1 && PromptTriggered)
        {
            ButtonsMain.SetActive(true);
            textMain.SetActive(true);
        }
        if (!PromptTriggered)
        {
            ButtonsMain.SetActive(false);
            textMain.SetActive(false);
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

    void ResetEverythingHere()
    {

        textMain.SetActive(false);
        ButtonsMain.SetActive(false);
        Color newColour = TextBox.color;
        newColour.a = 0;
        TextBox.color = newColour;
    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player" && !(LevelGM.Instance.playDeath))
        {
            Debug.Log("Tutorial Prompt Triggered");
            PromptTriggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D myTrigg)
    {

        if (myTrigg.tag == "Player")
        {
            PromptTriggered = false;
            Debug.Log("Tutorial Prompt Exit");
        }
    }
}

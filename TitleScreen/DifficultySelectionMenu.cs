using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifficultySelectionMenu : MonoBehaviour {
    
    PlayerController Controls;

    public int CurrentSelection;
    public string NextSceneToGo = "PreIntroCutscene";

    public bool selectedButtonDone = false;

    float selectionTimer;
    public float selectionBuffer = 0.2f;

    public GameObject[] SelectionArrows;
    public GameObject[] DifficultyDescription;

    int move;
    public float InvokeTime = 1;

    public GameObject[] InputHelp;
    public GameObject NowLoading;

    // Use this for initialization
    void Start () {

        Controls = PlayerController.CreateWithDefaultBindings();

        CurrentSelection = 1;

        NowLoading.SetActive(false);
        SelectedDifficultyDisplayGo();

        selectionTimer = 1;
    }
	
	// Update is called once per frame
	void Update () {
        selectionTimer -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
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

        float moveInput = Controls.Move.Y; //Horizontal movement input.
        //float moveInput = Controls.Move.Y; //Horizontal movement input.
        {
            if (moveInput > .2)
            {
                moveInput = 1;
            }
            else if (moveInput <= .2 && moveInput >= -.2)
            {
                moveInput = 0;
            }
            else if (moveInput < -.2)
            {
                moveInput = -1;
            }
        } //Normalize Move Input
        move = Mathf.RoundToInt(moveInput);
        
        if (Controls.Return.WasPressed)
        {
            ReturnToTitle();
        }
        SelectedDifficultyDisplayGo();
        switch (CurrentSelection)
        {
            case 0:

                if (Controls.Confirm.WasPressed && selectionTimer <=0)
                {
                    ToBeginGame(Player_States.DifficultySetting.Easy);
                }

                if (selectionTimer <= 0)
                {
                    switch (move)
                    {
                        case 1:
                            CurrentSelection = 2;
                            break;
                        case -1:
                            CurrentSelection = 1;
                            break;
                    }
                    if (Mathf.Abs(move) > 0)
                    {
                        selectionTimer = selectionBuffer;
                    }
                }
                SelectedDifficultyDisplayGo();

                break;
            case 1:

                if (Controls.Confirm.WasPressed && selectionTimer <= 0)
                {
                    ToBeginGame(Player_States.DifficultySetting.Normal);
                }

                if (selectionTimer <= 0)
                {
                    switch (move)
                    {
                        case 1:
                            CurrentSelection = 0;
                            break;
                        case -1:
                            CurrentSelection = 2;
                            break;
                    }
                    if (Mathf.Abs(move) > 0)
                    {
                        selectionTimer = selectionBuffer;
                    }
                }
                SelectedDifficultyDisplayGo();

                break;
            case 2:

                if (Controls.Confirm.WasPressed && selectionTimer <= 0)
                {
                    ToBeginGame(Player_States.DifficultySetting.Hard);
                }

                if (selectionTimer <= 0)
                {
                    switch (move)
                    {
                        case 1:
                            CurrentSelection = 1;
                            break;
                        case -1:
                            CurrentSelection = 0;
                            break;
                    }
                    if (Mathf.Abs(move) > 0)
                    {
                        selectionTimer = selectionBuffer;
                    }
                }
                SelectedDifficultyDisplayGo();

                break;
        }
    }


    void ReturnToTitle()
    {

        SceneManager.LoadScene("_TitleScreen");

    }

    void DisplayButtonsTypes(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputHelp)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputHelp[ControllerInputType].SetActive(true);
    }

    void SelectedDifficultyDisplayGo()
    {
        foreach (GameObject gos in SelectionArrows)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in DifficultyDescription)
        {
            gos.SetActive(false);
        }
        SelectionArrows[CurrentSelection].SetActive(true);
        DifficultyDescription[CurrentSelection].SetActive(true);
    }


    void ToBeginGame(Player_States.DifficultySetting difficulty)
    {
        Game.current.Progress.CurrentDifficulty = difficulty;
        SaveAndLoad.Save(Game.current.Progress.FileNumber);


        SceneManager.LoadScene(NextSceneToGo);
    }
}

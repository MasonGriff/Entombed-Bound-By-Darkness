using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour {

    //Button TestButton;
    public int SelectedSaveFile = 0;
    public string NewGameScene, Levels1ClearedScene, Levels2ClearedScene, Levels3ClearedScene;
    public string StageSelectScene = "StageSelect";

    PlayerController Controls;
    public bool[] FileFound;

    public int Selections;

    public bool selectedButtonDone = false;

    public string[] LocationNames = new string[5];
    

    float selectionTimer;
    public float selectionBuffer = 0.2f;

    public GameObject[] LoadExistsObj, NoLoadObj;
    public GameObject NowLoading;
    public GameObject UIButtonToLoad;
    public Text[] SaveDate, Location;
    public Text[] DeathCountTextField;
    public Text[] DifficultyText;
    public Image[] Sword1, Sword2;

    float move;
    public float InvokeTime = 0;

   // [Header("Testing")]
   // public bool DualshockTesting = false;
   // public string ControllerInput = " Xbox";
    public GameObject[] InputHelp;

    [Header("Sword Icon Colours")]
    public Color SwordObtainedColour;
    public Color SwordIgnoredColour, SwordNotReachedColour;



    private void Awake()
    {
        InvokeTime = .01f;
    }

    // Use this for initialization
    void Start ()
    {
        Controls = PlayerController.CreateWithDefaultBindings();



        //see if save file exists
        SaveAndLoad.LoadExists();
        if (SaveAndLoad.SaveExists == true)
        {
            FileFound[0] = true;
            Debug.Log("save 1 found");
        }
        else if (SaveAndLoad.SaveExists == false)
        {
            FileFound[0] = false;
            Debug.Log("no save 1");
        }

        if (SaveAndLoad.Save2Exists == true)
        {
            FileFound[1] = true;
            Debug.Log("save 2 found");
        }
        else if (SaveAndLoad.Save2Exists == false)
        {
            FileFound[1] = false;
            Debug.Log("no save 2");
        }

        selectedButtonDone = false;

        //if (FileFound)
        //{
        //
        //}
        NowLoading.SetActive(false);
        //set menu based on if save file exists
        Selections = 1;
        if (FileFound[0])
        {
            LoadExistsObj[0].SetActive(true);
            NoLoadObj[0].SetActive(false);
            SetupDisplayForLoadData(0);
        }
        else if (!FileFound[0])
        {

            LoadExistsObj[0].SetActive(false);
            NoLoadObj[0].SetActive(true);
        }
        if (FileFound[1])
        {
            LoadExistsObj[1].SetActive(true);
            NoLoadObj[1].SetActive(false);
            SetupDisplayForLoadData(1);
        }
        else if (!FileFound[1])
        {

            LoadExistsObj[1].SetActive(false);
            NoLoadObj[1].SetActive(true);
        }

        selectionTimer = 0;
	}
    private void FixedUpdate()
    {
        //TestButton.Select();
        //Debug.Log("Last Device Style: " + Controls.LastDeviceStyle);
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
        move = moveInput;
        if (Controls.Return.WasPressed)
        {
            Invoke("ReturnToTitle", InvokeTime);
        }
        /*if (!FileFound)
        {
            if (Controls.Return.WasPressed)
            {
                Invoke("ReturnToTitle", InvokeTime);
            }
        }*/

        //Menu Nav
        {
            if (Mathf.Abs(move) > 0)
            {
                if (selectionTimer <= 0)
                {
                    SelectedSaveFile = (SelectedSaveFile == 0 ? 1:0);
                    selectionTimer = selectionBuffer;
                }
            }
            foreach (GameObject gos in LoadExistsObj)
            {
                gos.GetComponent<Outline>().enabled = false;
            }
            LoadExistsObj[SelectedSaveFile].GetComponent<Outline>().enabled = true;
            {
                if (FileFound[SelectedSaveFile] && !selectedButtonDone)
                {

                    if (Controls.Return.WasPressed)
                    {
                        LoadExistsObj[SelectedSaveFile].GetComponent<Outline>().enabled = false;
                        Invoke("ReturnToTitle", InvokeTime);
                    }
                    if (Controls.Confirm.WasPressed)
                    {
                        SaveAndLoad.Load(SelectedSaveFile + 1);

                        selectedButtonDone = true;
                        LoadSelected();
                    }
                    else if (Controls.Delete.WasPressed)
                    {
                        DeletingSaveFile(SelectedSaveFile);
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update ()
    {
        selectionTimer -= Time.deltaTime;
	}


    void DeletingSaveFile(int WhichSave)
    {
        SaveAndLoad.DeleteSave(SelectedSaveFile +1);
        FileFound[WhichSave] = false;
        SaveAndLoad.SaveExists = false;


        LoadExistsObj[WhichSave].SetActive(false);
        NoLoadObj[WhichSave].SetActive(true);
    }

    void ReturnToTitle()
    {
        SceneManager.LoadScene("_TitleScreen");
    }

    public void LoadSelected()
    {
        selectedButtonDone = true;
        NowLoading.SetActive(true);
        UIButtonToLoad.SetActive(false);
        Invoke("LoadGameFromFile", InvokeTime);
    }

    void LoadGameFromFile()
    {
        foreach (Game g in SaveAndLoad.savedGames)
        {
            //Debug.Log(g.Progress.Stage1Cleared.ToString());
            Game.current = g;
            Debug.Log("load successful");
            GoToLoadedGame();
        }
    }

    

    void GoToLoadedGame()
    {
        /*if (!Game.current.Progress.Stage1Cleared)
        {
            SceneManager.LoadScene(NewGameScene);
        }
        else if (Game.current.Progress.Stage1Cleared)
        {
            if (Game.current.Progress.Stage2Cleared)
            {
                if (Game.current.Progress.Stage3Cleared)
                {
                    SceneManager.LoadScene(Levels3ClearedScene);
                }
                else
                {
                    SceneManager.LoadScene(Levels2ClearedScene);
                }
            }
            else
            {
                SceneManager.LoadScene(Levels1ClearedScene);
            }
        }*/

        if (Game.current.Progress.Level1[0] != Player_States.LevelCompletion.Cleared)
        {
            SceneManager.LoadScene(NewGameScene);
        }
        else if (Game.current.Progress.Level1[2] == Player_States.LevelCompletion.Cleared && Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached)
        {
            SceneManager.LoadScene("Sword1");
        }
        else if (Game.current.Progress.Level2[2] == Player_States.LevelCompletion.Cleared && Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.NotReached)
        {
            SceneManager.LoadScene("Sword2");
        }
        else if (Game.current.Progress.NewGamePlus)
        {
            switch (Game.current.Progress.NewGamePlusStarted)
            {
                case false:
                    Game.current.Progress.Sword1 = Player_States.SwordsObtainedStatus.NotReached;
                    Game.current.Progress.Sword2 = Player_States.SwordsObtainedStatus.NotReached;
                    Game.current.Progress.NewGamePlusStarted = true;
                    SceneManager.LoadScene("DifficultySelect");
                    break;
                case true:
                    SceneManager.LoadScene(StageSelectScene);
                    break;
            }
        }
        else //Any other possible scene;
        {
            SceneManager.LoadScene(StageSelectScene);
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

    void SetupDisplayForLoadData(int WhichSave)
    {
        SaveAndLoad.Load(WhichSave + 1);
        {
            foreach (Game g in SaveAndLoad.savedGames)
            {
                if (g.Progress.Level1[2] == Player_States.LevelCompletion.Cleared)
                {
                    if (g.Progress.Level2[2] == Player_States.LevelCompletion.Cleared)
                    {
                        if (g.Progress.Level3[2] == Player_States.LevelCompletion.Cleared)
                        {

                            if (g.Progress.FinalBoss == Player_States.LevelCompletion.Cleared)
                            {
                                Location[WhichSave].text = LocationNames[4];
                            }
                            else
                            {
                                Location[WhichSave].text = LocationNames[3];
                            }
                        }
                        else
                        {

                            Location[WhichSave].text = LocationNames[2];
                        }
                    }
                    else
                    {

                        Location[WhichSave].text = LocationNames[1];
                    }
                }
                else
                {
                    Location[WhichSave].text = LocationNames[0];
                }

                SaveDate[WhichSave].text = g.Progress.SaveDate;

                switch (g.Progress.CurrentDifficulty)
                {
                    case Player_States.DifficultySetting.Easy:
                        DifficultyText[WhichSave].text = "Easy";
                        break;
                    case Player_States.DifficultySetting.Normal:
                        DifficultyText[WhichSave].text = "Normal";
                        break;
                    case Player_States.DifficultySetting.Hard:
                        DifficultyText[WhichSave].text = "Hard";
                        break;
                }

                switch (g.Progress.Sword1)
                {
                    case Player_States.SwordsObtainedStatus.Ignored:
                        Sword1[WhichSave].color = SwordIgnoredColour;
                        break;
                    case Player_States.SwordsObtainedStatus.Obtained:
                        Sword1[WhichSave].color = SwordObtainedColour;
                        break;
                    case Player_States.SwordsObtainedStatus.NotReached:
                        Sword1[WhichSave].color = SwordNotReachedColour;
                        break;
                }
                switch (g.Progress.Sword2)
                {
                    case Player_States.SwordsObtainedStatus.Ignored:
                        Sword2[WhichSave].color = SwordIgnoredColour;
                        break;
                    case Player_States.SwordsObtainedStatus.Obtained:
                        Sword2[WhichSave].color = SwordObtainedColour;
                        break;
                    case Player_States.SwordsObtainedStatus.NotReached:
                        Sword2[WhichSave].color = SwordNotReachedColour;
                        break;
                }


                DeathCountTextField[WhichSave].text = g.Progress.PlayerDeathCount.ToString() + " Deaths";
            }
            
        }
    }
}

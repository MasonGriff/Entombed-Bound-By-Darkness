using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StageSelect : MonoBehaviour {

    public static StageSelect Instance { get; set; }

    PlayerController Controls;
    public string[] NextLevelToGo = new string[10];

    public int SelectedStage = 0, StageCap = 9;
    public int SelectedLevel = 0, LevelCap = 3;

    public bool[] StageComplete;
    public bool[] LevelComplete;

    public Transform[] LevelGroupPosition;
    public GameObject myCamera;
    

    public Transform stageSelectOverheadOverlay;

    public Text[] ClearTimesText, DeathCountText;

    public GameObject[] StageInformation;
    public Outline[] ThumbnailHighlight;
    public Image[] StageThumbnail;
    public Image[] StageRating;

    public Color ActiveColour, DeactiveColour;

    public Sprite[] StarRatingSprites;

    public float buttonTimer = 0;
    public float buttonReset = .2f;
    public int horiMove;
    public int VertMove;

    public Transform CostumeSelectPosition, costumeSelWindow;
    public Transform[] presetPositionsForCosSelect;

    public float CameraMoveTime = 0.6f;

    public bool testingSaves = false;

    public GameObject[] InputHelp;
    public GameObject InputMain, NowLoading;

    public bool CheckingOutfits = false;

    public Text[] TitleScreenReturnScreen;

    private void Awake()
    {
        Instance = this;
        if (Game.current == null) //If this isn't on a save file.
        {
            Game.current = new Game(); //Creates new default save file.
        }
        if (GameOptions.current == null) //If there's no saved game options.
        {
            OptionsSaveLoad.LoadSettings();
        }
        if (testingSaves)
        {
            Game.current.Progress.Level1[0] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level1[1] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level1[2] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level2[0] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level2[1] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level2[2] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level3[0] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level3[1] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.Level3[2] = Player_States.LevelCompletion.Cleared;
            Game.current.Progress.FinalBoss = Player_States.LevelCompletion.Cleared;

            Game.current.Progress.Sword1 = Player_States.SwordsObtainedStatus.Ignored;
            Game.current.Progress.Sword2 = Player_States.SwordsObtainedStatus.Ignored;
        }
        CheckingOutfits = false;
    }

    // Use this for initialization
    void Start()
    {
        Controls = PlayerController.CreateWithDefaultBindings();
        myCamera.transform.position = new Vector3(LevelGroupPosition[0].position.x, LevelGroupPosition[0].position.y, myCamera.transform.position.z);
        SetupStageInfo();
        HowManyUnlocked();
        buttonTimer = buttonReset;
    }
	// Update is called once per frame
	void Update () {
        buttonTimer -= Time.deltaTime;

        foreach(Text gos in TitleScreenReturnScreen)
        {
            gos.text = ((CheckingOutfits) ? "Return" : "To Title Screen");
        }

	}

    private void FixedUpdate()
    {

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
        float moveInput2 = Controls.Move.X;
        {
            if (moveInput2 > .2)
            {
                moveInput2 = 1;
            }
            else if (moveInput2 <= .2 && moveInput2 >= -.2)
            {
                moveInput2 = 0;
            }
            else if (moveInput2 < -.2)
            {
                moveInput2 = -1;
            }
        }
        horiMove = Mathf.RoundToInt(moveInput2);

        moveInput2 = Controls.Move.Y;
        {
            if (moveInput2 > .2)
            {
                moveInput2 = 1;
            }
            else if (moveInput2 <= .2 && moveInput2 >= -.2)
            {
                moveInput2 = 0;
            }
            else if (moveInput2 < -.2)
            {
                moveInput2 = -1;
            }
        }

        CostumeSelectPosition.position = presetPositionsForCosSelect[SelectedLevel].position;
        VertMove = Mathf.RoundToInt(moveInput2);
        if (!CheckingOutfits)
        {
            CurrentStageHighlighted();
            if (buttonTimer <= Time.deltaTime)
            {
                if (VertMove == -1 || Controls.ClearMind.WasPressed)
                {
                    CheckingOutfits = true;
                    myCamera.transform.DOMoveY(costumeSelWindow.position.y, CameraMoveTime);
                }
                if (!CheckingOutfits)
                {
                    switch (horiMove)
                    {
                        case -1:
                            buttonTimer = buttonReset;
                            if (SelectedStage <= 0)
                            {
                                SelectedStage = StageCap;
                            }
                            else
                            {
                                SelectedStage--;
                            }
                            {
                                if (SelectedStage < 0)
                                {
                                    SelectedStage = 0;
                                }
                                if (SelectedStage > StageCap)
                                {
                                    SelectedStage = StageCap;
                                }
                            }

                            CurrentStageHighlighted();
                            break;
                        case 1:
                            buttonTimer = buttonReset;
                            if (SelectedStage >= StageCap)
                            {
                                SelectedStage = 0;
                            }
                            else
                            {
                                SelectedStage++;
                            }
                            {
                                if (SelectedStage < 0)
                                {
                                    SelectedStage = 0;
                                }
                                if (SelectedStage > StageCap)
                                {
                                    SelectedStage = StageCap;
                                }
                            }

                            CurrentStageHighlighted();
                            break;
                    }
                    if (Controls.Confirm.WasPressed)
                    {
                        SelectedAStage();
                    }
                    else if (Controls.Return.WasPressed && !CheckingOutfits && buttonTimer <=0)
                    {
                        SaveAndLoad.Save(Game.current.Progress.FileNumber);
                        SceneManager.LoadScene("_TitleScreen");
                    }
                }
            }

            if (SelectedStage >= 0 && SelectedStage < 3)
            {
                SelectedLevel = 0;
            }
            else if (SelectedStage >= 3 && SelectedStage < 6)
            {
                SelectedLevel = 1;
            }
            else if (SelectedStage >= 6 && SelectedStage < 9)
            {
                SelectedLevel = 2;
            }
            else if (SelectedStage == 9)
            {
                SelectedLevel = 3;
            }

            CostumeSelectPosition.position = presetPositionsForCosSelect[SelectedLevel].position;

            myCamera.transform.DOMoveX(LevelGroupPosition[SelectedLevel].position.x, CameraMoveTime);
            myCamera.transform.DOMoveY(LevelGroupPosition[SelectedLevel].position.y, CameraMoveTime);
        }

        if (CheckingOutfits)
        {

            myCamera.transform.DOMoveY(costumeSelWindow.position.y, CameraMoveTime);
        }

        CostumeSelectPosition.position = presetPositionsForCosSelect[SelectedLevel].position;
    }


    void HowManyUnlocked()
    {
        foreach(GameObject gos in StageInformation)
        {
            gos.SetActive(false);
        }
        foreach(Image gos in StageThumbnail)
        {
            gos.color = DeactiveColour;
        }
        foreach (Outline gos in ThumbnailHighlight)
        {
            gos.enabled = false;
        }
        foreach(Image gos in StageRating)
        {
            gos.gameObject.SetActive(false);
        }

        SelectedStage = 0;
        SelectedLevel = 0;

        //Level 1
        //Level 1: Stage 1
        if (Game.current.Progress.Level1[0] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[0].color = ActiveColour;
            StageCap = 0;
            LevelCap = 0;
        }
        else if(Game.current.Progress.Level1[0] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[0].color = ActiveColour;
            StageInformation[0].SetActive(true);
            StageCap = 1;
            LevelCap = 0;
            StageComplete[0] = true;
            StageRating[0].gameObject.SetActive(true);
            StageRating[0].sprite = StarRatingSprites[Game.current.Progress.Level1Rating[0]];
        }
        //Level 1: Stage 2
        if (Game.current.Progress.Level1[1] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[1].color = ActiveColour;
            StageCap = 1;
            LevelCap = 0;
        }
        else if (Game.current.Progress.Level1[1] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[1].color = ActiveColour;
            StageInformation[1].SetActive(true);
            StageCap = 2;
            LevelCap = 0;
            StageComplete[1] = true;
            StageRating[1].gameObject.SetActive(true);
            StageRating[1].sprite = StarRatingSprites[Game.current.Progress.Level1Rating[1]];
        }
        //Level 1: Stage 3
        if (Game.current.Progress.Level1[2] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[2].color = ActiveColour;
            StageCap = 2;
            LevelCap = 0;
        }
        else if (Game.current.Progress.Level1[2] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[2].color = ActiveColour;
            StageInformation[2].SetActive(true);
            StageCap = (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached) ? 2 : 3;
            LevelCap = (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached) ? 0 : 1;
            StageComplete[2] = true;
            LevelComplete[0] = true;
            StageRating[2].gameObject.SetActive(true);
            StageRating[2].sprite = StarRatingSprites[Game.current.Progress.Level1Rating[2]];
        }

        //Level 2
        //Level 2: Stage 1
        if (Game.current.Progress.Level2[0] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[3].color = ActiveColour;
            StageCap = 3;
            LevelCap = 1;
        }
        else if (Game.current.Progress.Level2[0] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[3].color = ActiveColour;
            StageInformation[3].SetActive(true);
            StageCap = 4;
            LevelCap = 1;
            StageComplete[3] = true;
            StageRating[3].gameObject.SetActive(true);
            StageRating[3].sprite = StarRatingSprites[Game.current.Progress.Level2Rating[0]];
        }
        //Level 2: Stage 2
        if (Game.current.Progress.Level2[1] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[4].color = ActiveColour;
            StageCap = 4;
            LevelCap = 1;
        }
        else if (Game.current.Progress.Level2[1] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[4].color = ActiveColour;
            StageInformation[4].SetActive(true);
            StageCap = 5;
            LevelCap = 1;
            StageComplete[4] = true;
            StageRating[4].gameObject.SetActive(true);
            StageRating[4].sprite = StarRatingSprites[Game.current.Progress.Level2Rating[1]];
        }
        //Level 2: Stage 3
        if (Game.current.Progress.Level2[2] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[5].color = ActiveColour;
            StageCap = 5;
            LevelCap = 1;
        }
        else if (Game.current.Progress.Level2[2] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[5].color = ActiveColour;
            StageInformation[5].SetActive(true);
            StageCap = (Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.NotReached) ? 5 : 6;
            LevelCap = (Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.NotReached) ? 1 : 2;
            StageComplete[5] = true;
            LevelComplete[1] = true;
            StageRating[5].gameObject.SetActive(true);
            StageRating[5].sprite = StarRatingSprites[Game.current.Progress.Level2Rating[2]];
        }
        //Level 3
        //Level 3: Stage 1
        if (Game.current.Progress.Level3[0] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[6].color = ActiveColour;
            StageCap = 6;
            LevelCap = 2;
        }
        else if (Game.current.Progress.Level3[0] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[6].color = ActiveColour;
            StageInformation[6].SetActive(true);
            StageCap = 7;
            LevelCap = 2;
            StageComplete[6] = true;
            StageRating[6].gameObject.SetActive(true);
            StageRating[6].sprite = StarRatingSprites[Game.current.Progress.Level3Rating[0]];
        }
        //Level 3: Stage 2
        if (Game.current.Progress.Level3[1] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[7].color = ActiveColour;
            StageCap = 7;
            LevelCap = 2;
        }
        else if (Game.current.Progress.Level3[1] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[7].color = ActiveColour;
            StageInformation[7].SetActive(true);
            StageCap = 8;
            LevelCap = 2;
            StageComplete[7] = true;
            StageRating[7].gameObject.SetActive(true);
            StageRating[7].sprite = StarRatingSprites[Game.current.Progress.Level3Rating[1]];
        }
        //Level 3: Stage 3
        if (Game.current.Progress.Level3[2] == Player_States.LevelCompletion.Incomplete)
        {
            StageThumbnail[8].color = ActiveColour;
            StageCap = 8;
            LevelCap = 2;
        }
        else if (Game.current.Progress.Level3[2] == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[8].color = ActiveColour;
            StageInformation[8].SetActive(true);

            StageCap = 9;
            LevelCap = 3;

            StageComplete[8] = true;
            LevelComplete[2] = true;
            StageRating[8].gameObject.SetActive(true);
            StageRating[8].sprite = StarRatingSprites[Game.current.Progress.Level3Rating[2]];
            //StageCap = (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached) ? 5 : 6;
            //LevelCap = (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached) ? 1 : 2;
        }

        //Final Boss
        else if (Game.current.Progress.Level3[2] == Player_States.LevelCompletion.Cleared && Game.current.Progress.FinalBoss != Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[9].color = ActiveColour;
            StageCap = 9;
            LevelCap = 3;
            StageComplete[9] = true;
            LevelComplete[3] = true;
            //StageRating[9].gameObject.SetActive(true);
            //StageRating[9].sprite = StarRatingSprites[Game.current.Progress.FinalBossRating];
        }

        else if (Game.current.Progress.FinalBoss == Player_States.LevelCompletion.Cleared)
        {
            StageThumbnail[9].color = ActiveColour;
            StageCap = 9;
            LevelCap = 3;
            StageComplete[9] = true;
            LevelComplete[3] = true;
            StageRating[9].gameObject.SetActive(true);
            StageRating[9].sprite = StarRatingSprites[Game.current.Progress.FinalBossRating];
        }

    }

    void SetupStageInfo()
    {
        //Clear Times
        ClearTimesStringSetup(Game.current.Progress.Level1ClearTime[0], ClearTimesText[0]);
        ClearTimesStringSetup(Game.current.Progress.Level1ClearTime[1], ClearTimesText[1]);
        ClearTimesStringSetup(Game.current.Progress.Level1ClearTime[2], ClearTimesText[2]);

        ClearTimesStringSetup(Game.current.Progress.Level2ClearTime[0], ClearTimesText[3]);
        ClearTimesStringSetup(Game.current.Progress.Level2ClearTime[1], ClearTimesText[4]);
        ClearTimesStringSetup(Game.current.Progress.Level2ClearTime[2], ClearTimesText[5]);

        ClearTimesStringSetup(Game.current.Progress.Level3ClearTime[0], ClearTimesText[6]);
        ClearTimesStringSetup(Game.current.Progress.Level3ClearTime[1], ClearTimesText[7]);
        ClearTimesStringSetup(Game.current.Progress.Level3ClearTime[2], ClearTimesText[8]);

        ClearTimesStringSetup(Game.current.Progress.FinalBossClearTime, ClearTimesText[9]);

        //Death Count
        DeathCountStringSetup(Game.current.Progress.Level1Deaths[0], DeathCountText[0]);
        DeathCountStringSetup(Game.current.Progress.Level1Deaths[1], DeathCountText[1]);
        DeathCountStringSetup(Game.current.Progress.Level1Deaths[2], DeathCountText[2]);

        DeathCountStringSetup(Game.current.Progress.Level2Deaths[0], DeathCountText[3]);
        DeathCountStringSetup(Game.current.Progress.Level2Deaths[1], DeathCountText[4]);
        DeathCountStringSetup(Game.current.Progress.Level2Deaths[2], DeathCountText[5]);

        DeathCountStringSetup(Game.current.Progress.Level3Deaths[0], DeathCountText[6]);
        DeathCountStringSetup(Game.current.Progress.Level3Deaths[1], DeathCountText[7]);
        DeathCountStringSetup(Game.current.Progress.Level3Deaths[2], DeathCountText[8]);

        DeathCountStringSetup(Game.current.Progress.FinalBossDeaths, DeathCountText[9]);

    }

    void ClearTimesStringSetup(float thatClearTime, Text ClearTime)
    {
        float TimeMin;
        float TimeSec;

        TimeSec = thatClearTime % 60;
        TimeMin = thatClearTime - TimeSec;
        TimeMin = (TimeMin / 60);
        string timeString = (TimeMin.ToString() + "min " + TimeSec.ToString() + "sec");
        ClearTime.text = timeString;
    }

    void DeathCountStringSetup(int deathCountSaves, Text DeathCount)
    {
        string myDeaths = (deathCountSaves.ToString() + " deaths");
        DeathCount.text = myDeaths;
    }

    void CurrentStageHighlighted()
    {

        foreach (Outline gos in ThumbnailHighlight)
        {
            gos.enabled = false;
        }
        ThumbnailHighlight[SelectedStage].enabled = true;
    }

    void SelectedAStage()
    {
        InputMain.SetActive(false);
        NowLoading.SetActive(true);

        SceneManager.LoadScene(NextLevelToGo[SelectedStage]);
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

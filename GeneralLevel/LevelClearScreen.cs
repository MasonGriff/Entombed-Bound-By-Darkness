using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelClearScreen : MonoBehaviour {

    PlayerController Controls;

    public int StageSequenceNumber;
    public string[] StangeNames = new string[10];

    public AudioSource myAudioSource, MenuAudioSource;

    public int MenuOpeningSequence;
    public float buttonPressReset = .2f;
    public float buttonPressTime = 0;
    public float fadeInIntervalDecimal = 0.001f;
    public float MenuDarkTargetAlphaColour = .75f;

    float VertInput = 0;
    public int move = 0;

    public GameObject myCanvasObj;

    public GameObject InputMain, NowLoading;

    public float GoodTime = 40f; 

    [Header("Menu Stuff")]
    public Image MenuDark;
    public GameObject[] MenuObjects;
    public GameObject[] InputHelp;
    public int MenuItemSelectionNumber;
    public GameObject[] SelectionArrows;
    public Text[] SelectionTexts;
    public Color DefaultTextColour, HighlightColour, SelectedColour;
    public Image RatingUI;
    public Sprite[] RatingStarsSprites;

    int myRating;

    public Text DeathCountText, ClearTimeText;
    public string DeathCountString;
    public string ClearTimeString;

    Player_Main plyr;

    // Use this for initialization
    void Start()
    {
        Controls = PlayerController.CreateWithDefaultBindings();
        LevelBeginSetup();
        MenuOpeningSequence = 0;
        buttonPressTime = 0;
        myCanvasObj.SetActive(false);
        MenuItemSelectionNumber = 0;
        move = 0;
        myRating = 0;
        Text newDeath = ClearTimeText;
        Text newClear = DeathCountText;
        DeathCountText = newDeath;
        ClearTimeText = newClear;
        fadeInIntervalDecimal = 0.005f;
    }

    private void Update()
    {
        buttonPressTime -= Time.unscaledDeltaTime;
        VertInput = Controls.Move.Y; //Vertical input.
        {
            if (VertInput > .2)
            {
                VertInput = 1;
            }
            else if (VertInput <= .2 && VertInput >= -.2)
            {
                VertInput = 0;
            }
            else if (VertInput < -.2)
            {
                VertInput = -1;
            }
        } //Normalize Vert Input

        move = Mathf.RoundToInt(VertInput);

        switch (MenuOpeningSequence)
        {
            //menu appeared
            case 3:
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
                switch (MenuItemSelectionNumber)
                {
                    case 0:
                        if (move < 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 1;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (move > 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 3;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                        {
                            buttonPressTime = buttonPressReset;
                            SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                            NextLevel();
                        }

                        break;
                    case 1:
                        if (move < 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 2;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (move > 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 0;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                        {
                            buttonPressTime = buttonPressReset;
                            SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                            RestartLevelChosen();
                        }

                        break;
                    case 2:
                        if (move < 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 3;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (move > 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 1;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                        {
                            buttonPressTime = buttonPressReset;
                            SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                            QuitToStageSelect();
                        }

                        break;
                    case 3:
                        if (move < 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 0;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (move > 0 && buttonPressTime <= 0)
                        {
                            MenuItemSelectionNumber = 2;
                            buttonPressTime = buttonPressReset;
                            MenuAudioSource.Play();
                            DisplayCurrentSelection(MenuItemSelectionNumber);
                        }
                        else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                        {
                            buttonPressTime = buttonPressReset;
                            SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                            QuitToTitleScreen();
                        }

                        break;
                }


                break;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        VertInput = Controls.Move.Y; //Vertical input.
        {
            if (VertInput > .2)
            {
                VertInput = 1;
            }
            else if (VertInput <= .2 && VertInput >= -.2)
            {
                VertInput = 0;
            }
            else if (VertInput < -.2)
            {
                VertInput = -1;
            }
        } //Normalize Vert Input

        move = Mathf.RoundToInt(VertInput);

        switch (MenuOpeningSequence)
        {
            case 1:
                LevelGM.Instance.UpdateTime();
                UpdateRatings();
                int WinMinutes;
                int WinSeconds = Mathf.RoundToInt(LevelGM.Instance.myPlaytime % 60);
                WinMinutes = Mathf.RoundToInt(LevelGM.Instance.myPlaytime) - WinSeconds;
                WinMinutes = (WinMinutes / 60);
                ClearTimeString = (WinMinutes.ToString() + "min " + WinSeconds.ToString() + "sec");
                ClearTimeText.text = ClearTimeString;
                RatingUI.sprite = RatingStarsSprites[myRating];
                


                DeathCountString = (LevelGM.Instance.DeathCount.ToString() + " deaths");
                DeathCountText.text = DeathCountString;

                MenuObjects[0].SetActive(true);

                MenuOpeningSequence = 2;
                SaveAndLoad.Save(Game.current.Progress.FileNumber);
                Game.current.Progress.GameplayPaused = true;
                //Time.timeScale = 0;
                break;

            //fade in
            case 2:
                if (MenuDark.color.a < MenuDarkTargetAlphaColour)
                {
                    Color addToMenuColour = MenuDark.color;
                    addToMenuColour.a = (addToMenuColour.a + fadeInIntervalDecimal);
                    MenuDark.color = addToMenuColour;
                }
                else
                {
                    TurnOnMenuObjects();
                    MenuItemSelectionNumber = 0;
                    DisplayCurrentSelection(MenuItemSelectionNumber);
                    buttonPressTime = 1;
                    Time.timeScale = 0;
                    MenuOpeningSequence = 3;
                }
                break;
            //menu appeared
            case 3:
                {
                    /*
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
                    switch (MenuItemSelectionNumber)
                    {
                        case 0:
                            if (move < 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 1;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (move > 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 3;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                            {
                                buttonPressTime = buttonPressReset;
                                SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                                NextLevel();
                            }

                            break;
                        case 1:
                            if (move < 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 2;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (move > 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 0;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                            {
                                buttonPressTime = buttonPressReset;
                                SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                                RestartLevelChosen();
                            }

                            break;
                        case 2:
                            if (move < 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 3;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (move > 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 1;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                            {
                                buttonPressTime = buttonPressReset;
                                SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                                QuitToStageSelect();
                            }

                            break;
                        case 3:
                            if (move < 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 0;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (move > 0 && buttonPressTime <= 0)
                            {
                                MenuItemSelectionNumber = 2;
                                buttonPressTime = buttonPressReset;
                                MenuAudioSource.Play();
                                DisplayCurrentSelection(MenuItemSelectionNumber);
                            }
                            else if (Controls.Confirm.WasPressed && buttonPressTime <= 0)
                            {
                                buttonPressTime = buttonPressReset;
                                SelectionTexts[MenuItemSelectionNumber].color = SelectedColour;
                                QuitToTitleScreen();
                            }

                            break;
                    }
                    */
                }
                break;
        }

    }



    public void LevelEnd()
    {
        myCanvasObj.SetActive(true);
        SaveLevelComplete();
        TurnOffMenuObjects();
        Color addToMenuColour = MenuDark.color;

        myRating = 0;
        if (LevelGM.Instance.myPlaytime <= GoodTime)
        {
            myRating++;
        }
        if (LevelGM.Instance.NoDamage)
        {
            myRating++;
        }
        if (LevelGM.Instance.DeathCount == 0)
        {
            myRating++;
        }


        addToMenuColour.a = 0;
        MenuDark.color = addToMenuColour;
        MenuOpeningSequence = 1;
        MenuItemSelectionNumber = 0;
        LevelGM.Instance.BackgroundMusicSource.Pause();
        myAudioSource.Play();
        //myAudioSource.Play();
    }

    void RestartLevelChosen()
    {
        myAudioSource.Stop();
        MenuAudioSource.Stop();
        //MenuOpeningSequence = 0;
        //TurnOffMenuObjects();
       // Color addToMenuColour = MenuDark.color;
       // addToMenuColour.a = 0;
        //MenuDark.color = addToMenuColour;

        //myCanvasObj.SetActive(false);
        Time.timeScale = 1;
        Game.current.Progress.GameplayPaused = false;
        InputMain.SetActive(false);
        NowLoading.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //LevelGM.Instance.PlayerRespawn();
    }

    void NextLevel()
    {
        myAudioSource.Stop();
        MenuAudioSource.Stop();
        //MenuOpeningSequence = 0;
        //TurnOffMenuObjects();
        //Color addToMenuColour = MenuDark.color;
       // addToMenuColour.a = 0;
       // MenuDark.color = addToMenuColour;

        //myCanvasObj.SetActive(false);
        Time.timeScale = 1;
        Game.current.Progress.GameplayPaused = false;
        InputMain.SetActive(false);
        NowLoading.SetActive(true);
        SceneManager.LoadScene(StangeNames[StageSequenceNumber + 1] );
        //LevelGM.Instance.PlayerRespawn();
    }

    void TurnOffMenuObjects()
    {
        foreach (GameObject gos in MenuObjects)
        {
            gos.SetActive(false);
        }
    }
    void TurnOnMenuObjects()
    {
        foreach (GameObject gos in MenuObjects)
        {
            gos.SetActive(true);
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

    void QuitToTitleScreen()
    {
        InputMain.SetActive(false);
        NowLoading.SetActive(true);
        Time.timeScale = 1;
        Game.current.Progress.GameplayPaused = false;
        SceneManager.LoadScene("_TitleScreen");
    }
    void QuitToStageSelect()
    {
        Time.timeScale = 1;
        Game.current.Progress.GameplayPaused = false;

        InputMain.SetActive(false);
        NowLoading.SetActive(true);

        SceneManager.LoadScene("StageSelect");
    }

    void SaveLevelComplete()
    {
        switch (StageSequenceNumber)
        {
            case 0:
                Game.current.Progress.Level1[0] = Player_States.LevelCompletion.Cleared;
                break;
            case 1:
                Game.current.Progress.Level1[1] = Player_States.LevelCompletion.Cleared;
                break;
            case 2:
                Game.current.Progress.Level1[2] = Player_States.LevelCompletion.Cleared;
                break;
            case 3:
                Game.current.Progress.Level2[0] = Player_States.LevelCompletion.Cleared;
                break;
            case 4:
                Game.current.Progress.Level2[1] = Player_States.LevelCompletion.Cleared;
                break;
            case 5:
                Game.current.Progress.Level2[2] = Player_States.LevelCompletion.Cleared;
                break;
            case 6:
                Game.current.Progress.Level3[0] = Player_States.LevelCompletion.Cleared;
                break;
            case 7:
                Game.current.Progress.Level3[1] = Player_States.LevelCompletion.Cleared;
                break;
            case 8:
                Game.current.Progress.Level3[2] = Player_States.LevelCompletion.Cleared;
                break;
            case 9:
                Game.current.Progress.FinalBoss = Player_States.LevelCompletion.Cleared;
                break;
        }
    }

    void UpdateRatings()
    {
        switch (StageSequenceNumber)
        {
            case 0:
                Game.current.Progress.Level1Rating[0] = myRating;
                break;
            case 1:
                Game.current.Progress.Level1Rating[1] = myRating;
                break;
            case 2:
                Game.current.Progress.Level1Rating[2] = myRating;
                break;
            case 3:
                Game.current.Progress.Level2Rating[0] = myRating;
                break;
            case 4:
                Game.current.Progress.Level2Rating[1] = myRating;
                break;
            case 5:
                Game.current.Progress.Level2Rating[2] = myRating;
                break;
            case 6:
                Game.current.Progress.Level3Rating[0] = myRating;
                break;
            case 7:
                Game.current.Progress.Level3Rating[1] = myRating;
                break;
            case 8:
                Game.current.Progress.Level3Rating[2] = myRating;
                break;
            case 9:
                Game.current.Progress.FinalBossRating = myRating;
                break;
        }
    }
    void LevelBeginSetup()
    {
        switch (StageSequenceNumber)
        {
            case 0:
                LevelCompletionStatusSetup(Game.current.Progress.Level1[0]);
                break;
            case 1:
                LevelCompletionStatusSetup(Game.current.Progress.Level1[1]);
                break;
            case 2:
                LevelCompletionStatusSetup(Game.current.Progress.Level1[2]);
                break;
            case 3:
                LevelCompletionStatusSetup(Game.current.Progress.Level2[0]);
                break;
            case 4:
                LevelCompletionStatusSetup(Game.current.Progress.Level2[1]);
                break;
            case 5:
                LevelCompletionStatusSetup(Game.current.Progress.Level2[2]);
                break;
            case 6:
                LevelCompletionStatusSetup(Game.current.Progress.Level3[0]);
                break;
            case 7:
                LevelCompletionStatusSetup(Game.current.Progress.Level3[1]);
                break;
            case 8:
                LevelCompletionStatusSetup(Game.current.Progress.Level3[2]);
                break;
            case 9:
                LevelCompletionStatusSetup(Game.current.Progress.FinalBoss);
                break;
        }
    }

    void LevelCompletionStatusSetup(Player_States.LevelCompletion CompletionStatus)
    {
        if (CompletionStatus == Player_States.LevelCompletion.NotStarted)
        {
            CompletionStatus = Player_States.LevelCompletion.Incomplete;
        }
        else if (CompletionStatus == Player_States.LevelCompletion.Cleared)
        {
            CompletionStatus = Player_States.LevelCompletion.Cleared;
        }

        Debug.Log(Game.current.Progress.Level1[0].ToString());
        Debug.Log(Game.current.Progress.Level1[1].ToString());
        Debug.Log(Game.current.Progress.Level1[2].ToString());
        Debug.Log(Game.current.Progress.Level2[0].ToString());
        Debug.Log(Game.current.Progress.Level2[1].ToString());
        Debug.Log(Game.current.Progress.Level2[2].ToString());
        Debug.Log(Game.current.Progress.Level3[0].ToString());
        Debug.Log(Game.current.Progress.Level3[1].ToString());
        Debug.Log(Game.current.Progress.Level3[2].ToString());
        Debug.Log(Game.current.Progress.FinalBoss.ToString());
    }

    void DisplayCurrentSelection(int currentSel)
    {
        foreach (GameObject gos in SelectionArrows)
        {
            gos.SetActive(false);
        }
        foreach (Text teckst in SelectionTexts)
        {
            teckst.color = DefaultTextColour;
        }
        SelectionTexts[currentSel].color = HighlightColour;
        SelectionArrows[currentSel].SetActive(true);
    }



    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag== "Player")
        {
            plyr = CameraController.Instance.Player.GetComponent<Player_Main>();
            //plyr.myRB.velocity = new Vector2(0, 0);
            plyr.PlayerState = Player_States.PlayerStates.Normal;
            plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            plyr.myAnim.SetInteger("AnimState", 0);
            plyr.myAnim.SetBool("Dashing", false);
            plyr.myAnim.SetBool("Jump", false);
            plyr.myAnim.SetBool("Airborne", false);
            plyr.myAnim.SetBool("Dashing", false);
            plyr.MeleeScript.ResetMeleeState();
            plyr.myAnim.SetBool("ClearMind", false);

            plyr.myAnim.SetTrigger("Win");

            LevelEnd();
        }
    }
}

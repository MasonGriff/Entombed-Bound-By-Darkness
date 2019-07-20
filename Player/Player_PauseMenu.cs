using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Player_Main))]
public class Player_PauseMenu : MonoBehaviour {

    public enum enumPauseScreenOptions { None, Continue, Controls, Volume, StageSelect, Quit}

    Player_Main Player;
    PlayerController Controls;

    // public bool optionOpen = false;
    public bool MenuIsOpen = false;
    public float buttonPressTime = 0;
    public float buttonPressReset = .2f;

    public Color DefaultColour, HighlightedColour, SelectedColour;

    public int MenuNav;

    public float move;
    public float horiMove;

    public AudioSource mySoundSource;
   // public int MenuSelectionNumber = 0;

    public GameObject RegularPauseScreen, ControlsPauseScreen, VolumePauseScreen;

    [Header("Regular Pause Screen")]
    public int MenuSelectionNumber = 0;
    public enumPauseScreenOptions PauseScreenOption = enumPauseScreenOptions.None;
    public enumPauseScreenOptions PauseScreenOptionOpened = enumPauseScreenOptions.None;
    public GameObject[] MenuSelectionArrows;
    public GameObject[] InputHelp;
    public Text[] MenuTextSelections;
    public GameObject[] RegularMenusItems;

    [Header("Volume Screen")]
    public int VolumeSliderCurrentlySelected = 0;
    public GameObject[] VolumeSliderSelectedHighlight;
    public GameObject[] InputForVolume;
    public RectTransform[] MasterVolumePositions, MusicVolumePositions, SFXVolumePositions;
    public GameObject MasterVolumeArrow, MusicVolumeArrow, SFXVolumeArrow;
    public AudioSource[] VolumeTestSources;

    [Header("Controls Screen")]
    public GameObject[] InputForControls;


    private void Awake()
    {
        //PauseScript = this;
        //Game.current
    }

    // Use this for initialization
    void Start () {
        //InputManager.Setup()
        Controls = PlayerController.CreateWithDefaultBindings();
        Player = GetComponent<Player_Main>();
        ResetPauseMenuOptions();
        Player.PauseMenuObj.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        buttonPressTime -= Time.unscaledDeltaTime;
        //InputManager.Update()
		if (Player.pauseMenuCheck)
        {
            //Debug.Log("Last Device Style: " + Controls.LastDeviceStyle);
            string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
            string LastCheckDeviceUsed = LastDeviceUsed;
            //Debug.Log (InputDeviceStyle.PlayStation4.ToString());
            switch (LastDeviceUsed)
            {
                default:
                    DisplayButtonsTypes(0);
                    break;
                case "PlayStation4":
                    DisplayButtonsTypes(1);
                    break;
                case "XboxOne":
                    DisplayButtonsTypes(2);
                    break;
            }

            float moveInput = Controls.Move.Y;
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
            horiMove = moveInput2;
            {
                /*if ()
                {
                if (Controls.Start.WasPressed && buttonPressTime <= 0)
                {
                    buttonPressTime = buttonPressReset;
                    Player.pauseMenuCheck = false;
                    Game.current.Progress.GameplayPaused = false;
                    Debug.Log("Unpause");
                    Time.timeScale = 1;
                    //InControlManager.Instance.useFixedUpdate = false;
                }

                }*/
            }
            if (!MenuIsOpen)
            {
                if (Controls.Start.WasPressed && buttonPressTime <= 0)
                {
                    ReturnToGame();
                }
                if (Controls.Return.WasPressed)
                {
                    ReturnToGame();
                }
                switch (PauseScreenOption)
                {
                    case enumPauseScreenOptions.Continue:
                        DisplayArrowsOnSelection(0);
                        if (moveInput == 1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Quit;
                        }
                        else if (moveInput == -1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Controls;
                        }

                        if (Controls.Confirm.WasPressed)
                        {
                            mySoundSource.Play();
                            ReturnToGame();
                            //OpenMenuItem(MenuTextSelections[0]);
                        }
                        break;
                    case enumPauseScreenOptions.Controls:
                        DisplayArrowsOnSelection(1);
                        if (moveInput == 1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Continue;
                        }
                        else if (moveInput == -1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Volume;
                        }

                        if (Controls.Confirm.WasPressed)
                        {
                            mySoundSource.Play();
                            OpenMenuItem(MenuTextSelections[1]);
                        }
                        break;
                    case enumPauseScreenOptions.Volume:
                        DisplayArrowsOnSelection(2);
                        if (moveInput == 1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Controls;
                        }
                        else if (moveInput == -1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.StageSelect;
                        }

                        if (Controls.Confirm.WasPressed)
                        {
                            mySoundSource.Play();
                            OpenMenuItem(MenuTextSelections[2]);
                        }
                        break;
                    case enumPauseScreenOptions.StageSelect:
                        DisplayArrowsOnSelection(3);
                        if (moveInput == 1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Volume;
                        }
                        else if (moveInput == -1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Quit;
                        }

                        if (Controls.Confirm.WasPressed)
                        {
                            mySoundSource.Play();
                            MenuTextSelections[3].color = SelectedColour;
                            //QuitToTitleScreen();
                            ToStageSelect();
                        }
                        break;
                    case enumPauseScreenOptions.Quit:
                        DisplayArrowsOnSelection(4);
                        if (moveInput == 1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.StageSelect;
                        }
                        else if (moveInput == -1 && buttonPressTime <= 0)
                        {
                            mySoundSource.Play();
                            buttonPressTime = buttonPressReset;
                            PauseScreenOption = enumPauseScreenOptions.Continue;
                        }

                        if (Controls.Confirm.WasPressed)
                        {
                            mySoundSource.Play();
                            MenuTextSelections[4].color = SelectedColour;
                            QuitToTitleScreen();
                        }
                        break;
                }
            }
            else
            {
                if (Controls.Return.WasPressed)
                {
                    CloseCurrentlyOpenMenu();
                }
                else
                {
                    switch (PauseScreenOptionOpened)
                    {
                        case enumPauseScreenOptions.Continue:
                            ReturnToGame();
                            //GraphicsMenuControls();
                            break;
                        case enumPauseScreenOptions.Controls:
                            switch (LastDeviceUsed)
                            {
                                default:
                                    ControlsMenusControl(0);
                                    break;
                                case "PlayStation4":
                                    ControlsMenusControl(1);
                                    break;
                                case "XboxOne":
                                    ControlsMenusControl(2);
                                    break;
                            }
                            break;
                        case enumPauseScreenOptions.Volume:
                            VolumeMenuControls();
                            break;
                    }
                }
            }


        }
	}


    private void FixedUpdate()
    {
        if (!Player.pauseMenuCheck && !Player.pauseCheck && !(Player.PlayerHealthState== Player_States.PlayerHealthStates.Dead ))
        {
            if (Controls.Start.WasPressed && buttonPressTime<=0)
            {
                Game.current.Progress.GameplayPaused = true;
                Player.pauseMenuCheck = true;
                Debug.Log("paused");
                buttonPressTime = buttonPressReset;
                Player.PauseMenuObj.SetActive(true);
                ResetPauseMenuOptions();
                Time.timeScale = 0;
            }
        }
    }

    void ReturnToGame()
    {
        ResetPauseMenuOptions();
        buttonPressTime = buttonPressReset;
        Player.pauseMenuCheck = false;
        Game.current.Progress.GameplayPaused = false;
        Debug.Log("Unpause");
        Player.PauseMenuObj.SetActive(false);
        Time.timeScale = 1;
        //InControlManager.Instance.useFixedUpdate = false;
    }

    void ResetPauseMenuOptions()
    {

        VolumeSliderCurrentlySelected = 0;
        MenuIsOpen = false;
        move = 0;
        buttonPressTime = 0;
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;

        //GraphicsMenuObj.SetActive(false);
        ControlsPauseScreen.SetActive(false);
        VolumePauseScreen.SetActive(false);
        ReopenRegularMenu();
    }


    void DisplayButtonsTypes(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputHelp)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputHelp[ControllerInputType].SetActive(true);
    }


    void VolumeMenuControls()
    {
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;
        //Debug.Log (InputDeviceStyle.PlayStation4.ToString());
        //int ButtonTypesGoEre = 0;
        switch (LastDeviceUsed)
        {
            default:

                VolumeMenuButtonPrompts(0);
                break;
            case "PlayStation4":

                VolumeMenuButtonPrompts(1);
                break;
            case "XboxOne":

                VolumeMenuButtonPrompts(2);
                break;
        }
        //VolumeMenuButtonPrompts()
        VolumeSettingsArrowPositioningSetup();
        VolumeWindowMovement();


    }

    void VolumeSettingsArrowPositioningSetup()
    {
        ArrowSetupUndertone(MasterVolumeArrow, MasterVolumePositions, GameOptions.current.Settings.VolumeMaster);
        ArrowSetupUndertone(MusicVolumeArrow, MusicVolumePositions, GameOptions.current.Settings.VolumeMusic + .1f);
        ArrowSetupUndertone(SFXVolumeArrow, SFXVolumePositions, GameOptions.current.Settings.VolumeSFX + .1f);
    }

    void ArrowSetupUndertone(GameObject ArrowObj, RectTransform[] PossibleLocations, float SavedVolume)
    {
        int MyCurrentLocation = 0;

        float CurrentVolSettingFloat = SavedVolume;
        CurrentVolSettingFloat *= 10;

        CurrentVolSettingFloat -= 1;

        MyCurrentLocation = Mathf.RoundToInt(CurrentVolSettingFloat);

        ArrowObj.GetComponent<Image>().rectTransform.position = PossibleLocations[MyCurrentLocation].position;

    }

    void VolumeHighlightsCheck(int CurrentSelectionForVol)
    {
        foreach (GameObject arrowsMan in VolumeSliderSelectedHighlight)
        {
            arrowsMan.SetActive(false);

        }
        VolumeSliderSelectedHighlight[CurrentSelectionForVol].SetActive(true);
    }

    void VolumeSourceForTestingSetDeactive(int CurrentSelectionForVol)
    {
        foreach (AudioSource thisEreAudioSource in VolumeTestSources)
        {
            if (thisEreAudioSource != VolumeTestSources[CurrentSelectionForVol])
            { thisEreAudioSource.gameObject.SetActive(false); }
        }
        VolumeTestSources[CurrentSelectionForVol].gameObject.SetActive(true);
    }

    void VolumeWindowMovement()
    {
        VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
        VolumeSourceForTestingSetDeactive(VolumeSliderCurrentlySelected);

        switch (VolumeSliderCurrentlySelected)
        {
            //Master Volume
            case 0:
                VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                if (buttonPressTime <= 0)
                {
                    switch (Mathf.RoundToInt(move))
                    {
                        case 1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 2;
                            buttonPressTime = buttonPressReset;
                            break;
                        case -1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 1;
                            buttonPressTime = buttonPressReset;
                            break;
                    }
                }
                //ChangeVolumeSettingHere(MasterVolumeArrow, MasterVolumePositions[0]);
                if (buttonPressTime <= 0 && VolumeSliderCurrentlySelected == 0)
                {
                    switch (Mathf.RoundToInt(horiMove))
                    {
                        case 1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeMaster += .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                        case -1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeMaster -= .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                    }
                    if (GameOptions.current.Settings.VolumeMaster > 1)
                    {
                        GameOptions.current.Settings.VolumeMaster = 1;
                    }
                    if (GameOptions.current.Settings.VolumeMaster <= 0)
                    {
                        GameOptions.current.Settings.VolumeMaster = .1f;
                    }
                    ArrowSetupUndertone(MasterVolumeArrow, MasterVolumePositions, GameOptions.current.Settings.VolumeMaster);
                    VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                }

                break;
            //Music Volume
            case 1:
                VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                if (buttonPressTime <= 0)
                {
                    switch (Mathf.RoundToInt(move))
                    {
                        case 1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 0;
                            buttonPressTime = buttonPressReset;
                            break;
                        case -1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 2;
                            buttonPressTime = buttonPressReset;
                            break;
                    }
                }
                //ChangeVolumeSettingHere(MasterVolumeArrow, MasterVolumePositions[0]);
                if (buttonPressTime <= 0 && VolumeSliderCurrentlySelected == 1)
                {
                    switch (Mathf.RoundToInt(horiMove))
                    {
                        case 1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeMusic += .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                        case -1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeMusic -= .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                    }
                    if (GameOptions.current.Settings.VolumeMusic > .9)
                    {
                        GameOptions.current.Settings.VolumeMusic = .9f;
                    }
                    if (GameOptions.current.Settings.VolumeMusic < 0)
                    {
                        GameOptions.current.Settings.VolumeMusic = 0;
                    }
                    ArrowSetupUndertone(MusicVolumeArrow, MusicVolumePositions, GameOptions.current.Settings.VolumeMusic + .1f);
                    VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                }

                break;
            //Sound Effects Volume
            case 2:
                VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                if (buttonPressTime <= 0)
                {
                    
                    switch (Mathf.RoundToInt(move))
                    {
                        case 1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 1;
                            buttonPressTime = buttonPressReset;
                            break;
                        case -1:
                            mySoundSource.Play();
                            VolumeSliderCurrentlySelected = 0;
                            buttonPressTime = buttonPressReset;
                            break;
                    }
                }
                //ChangeVolumeSettingHere(MasterVolumeArrow, MasterVolumePositions[0]);
                if (buttonPressTime <= 0 && VolumeSliderCurrentlySelected == 2)
                {
                    switch (Mathf.RoundToInt(horiMove))
                    {
                        case 1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeSFX += .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                        case -1:
                            buttonPressTime = buttonPressReset;
                            GameOptions.current.Settings.VolumeSFX -= .1f;
                            VolumeTestSources[VolumeSliderCurrentlySelected].Play();
                            break;
                    }
                    if (GameOptions.current.Settings.VolumeSFX > .9)
                    {
                        GameOptions.current.Settings.VolumeSFX = .9f;
                    }
                    if (GameOptions.current.Settings.VolumeSFX < 0)
                    {
                        GameOptions.current.Settings.VolumeSFX = 0;
                    }
                    ArrowSetupUndertone(SFXVolumeArrow, SFXVolumePositions, GameOptions.current.Settings.VolumeSFX + .1f);
                    VolumeHighlightsCheck(VolumeSliderCurrentlySelected);
                }

                break;
        }


    }

    void ChangeVolumeSettingHere(GameObject arrowObj, RectTransform NewPositionTransform)
    {
        switch (Mathf.RoundToInt(horiMove))
        {
            case 1:

                break;
            case -1:

                break;
        }
    }
    void VolumeMenuButtonPrompts(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputForVolume)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputForVolume[ControllerInputType].SetActive(true);
    }

    void DisplayArrowsOnSelection(int SelNumb)
    {
        foreach (GameObject gos in MenuSelectionArrows)
        {
            gos.SetActive(false);
        }
        MenuSelectionArrows[SelNumb].SetActive(true);

        foreach (Text myText in MenuTextSelections)
        {
            myText.color = DefaultColour;
        }
        MenuTextSelections[SelNumb].color = HighlightedColour;
    }

    void QuitToTitleScreen()
    {
        Time.timeScale = 1;
        OptionsSaveLoad.SaveSettings();
        Game.current.Progress.GameplayPaused = false;
        SaveAndLoad.Save(Game.current.Progress.FileNumber);
        SceneManager.LoadScene("_TitleScreen");
    }

    void CloseRegularMenu()
    {
        foreach (GameObject gos in RegularMenusItems)
        {
            gos.SetActive(false);
        }
    }
    void ReopenRegularMenu()
    {
        foreach (GameObject gos in RegularMenusItems)
        {
            gos.SetActive(true);
        }
    }

    void OpenMenuItem(Text MenuToOpenObj)
    {
        MenuToOpenObj.color = SelectedColour;
        PauseScreenOptionOpened = PauseScreenOption;
        OpenMenuItemInvokeFuction();
        //Invoke("OpenMenuItemInvokeFuction", buttonPressReset);
    }

    void OpenMenuItemInvokeFuction()
    {
        GameObject MenuToOpenObj = null;
        switch (PauseScreenOptionOpened)
        {
            case enumPauseScreenOptions.Controls:
                MenuToOpenObj = ControlsPauseScreen;
                break;
            case enumPauseScreenOptions.Volume:
                MenuToOpenObj = VolumePauseScreen;
                break;
        }
        //GraphicsMenuObj.SetActive(false);
        ControlsPauseScreen.SetActive(false);
        VolumePauseScreen.SetActive(false);
        MenuToOpenObj.SetActive(true);
        MenuIsOpen = true;
        CloseRegularMenu();
        PauseScreenOptionOpened = PauseScreenOption;
    }

    void CloseCurrentlyOpenMenu()
    {
        ReopenRegularMenu();
        //GraphicsMenuObj.SetActive(false);
        ControlsPauseScreen.SetActive(false);
        VolumePauseScreen.SetActive(false);
        PauseScreenOptionOpened = enumPauseScreenOptions.None;
        MenuIsOpen = false;

    }


    void ControlsMenusControl(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputForControls)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputForControls[ControllerInputType].SetActive(true);
    }

    void ToStageSelect()
    {
        Time.timeScale = 1;
        Game.current.Progress.GameplayPaused = false;
        OptionsSaveLoad.SaveSettings();
        SaveAndLoad.Save(Game.current.Progress.FileNumber);
        SceneManager.LoadScene("StageSelect");
    }

}

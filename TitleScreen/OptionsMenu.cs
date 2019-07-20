using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InControl;

public class OptionsMenu : MonoBehaviour {

    public enum OptionsMenuNavigation { None, Graphics, Controls, Volume, Title}

    public Camera myCurrentCamera;
    public AudioSource mySoundSource;

    PlayerController Controls;

    public OptionsMenuNavigation MenuNav = OptionsMenuNavigation.Graphics;
    public OptionsMenuNavigation OptionsOpen = OptionsMenuNavigation.None;

    public bool MenuIsOpen = false;


    public Color DefaultColour, HighlightedColour, SelectedColour;

    public GameObject[] InputHelp;
    public GameObject[] InputForControls;
    public GameObject[] InputForVolume;

    public GameObject GraphicsMenuObj, ControllsMenuObj, VolumeMenuObj;

    public GameObject[] MenuSelections = new GameObject[4];
    public Text[] MenuTextSelections;


    public GameObject[] RegularMenusItems;

    float move;
    float horiMove;
    public float buttonPressReset = 0.02f;
    float buttonPressTime;
    public float SelectionBuffer = 0.1f;
    float SelectionTimer = 0f;

    public RectTransform[] MasterVolumePositions, MusicVolumePositions, SFXVolumePositions;
    public GameObject MasterVolumeArrow, MusicVolumeArrow, SFXVolumeArrow;

    public int VolumeSliderCurrentlySelected;
    public GameObject[] VolumeSliderSelectedHighlight = new GameObject[3];
    public AudioSource[] VolumeTestSources = new AudioSource[3];

    public Text[] FullScreenSettingsText;
    public Color SettingIsOnColour, SettingIsOffColour;

    public int GraphicsMenuSelectionNumber = 0;
    public GameObject[] GraphicsMenuSelectionItems;
    public GameObject[] GraphicsMenuButtonPrompts;
    public Vector2 myGameWindowCurrentResolution;
    public int GraphicsMenuResolutionOptions = 0;
    public Text[] ResolutionOptionsToChange;
    
    public float[] volumeSettingsCurrentlyCheck = new float[4];

    public bool fullscreenIsActive;
    // public Text[] ResolutionSettings16x9, ResolutionSettings16x10;
    //public Sprite[] ControlInputTypeDisplay = new Sprite[2];
    //public string trainingRoom = "PlayerMovementTesting";
    bool ExistingOptions = false;
    // Use this for initialization
    // Use this for initialization


    Vector2 AcceptedRes720p = new Vector2(1280, 720);
    Vector2 AcceptedRes1080p = new Vector2(1920, 1080);
    Vector2 AcceptedRes768p = new Vector2(1366, 768);
    Vector2 AcceptedRes900p = new Vector2(1600, 900);

    void Start()
    {
        VolumeSliderCurrentlySelected = 0;
        bool LoadAvailable;
        GraphicsMenuResolutionOptions = 0;
        OptionsSaveLoad.LoadSettings();
        switch (OptionsSaveLoad.OptionsSaveExists)
        {
            case true:
                LoadAvailable = true;
                break;
            case false:
                LoadAvailable = false;
                break;
            default:
                LoadAvailable = false;
                break;
        }
        ExistingOptions = LoadAvailable;
        
        if (!ExistingOptions)
        {
            GameOptions.current = new GameOptions();
        }

        OptionsSaveLoad.SaveSettings();
        Controls = PlayerController.CreateWithDefaultBindings();
        //Selection2 = false;
        //ControlsListOpen = false;
        MenuIsOpen = false;
        move = 0;
        buttonPressTime = 0;
        //Debug.Log("Last Device Style: " + Controls.LastDeviceStyle);
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;

        GraphicsMenuObj.SetActive(false);
        ControllsMenuObj.SetActive(false);
        VolumeMenuObj.SetActive(false);
        ReopenRegularMenu();
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
        //selected = false;
        MenuNav = OptionsMenuNavigation.Graphics;
        OptionsOpen = OptionsMenuNavigation.None;
        buttonPressTime = buttonPressReset;
        SelectionTimer = SelectionBuffer;
        GraphicsMenuSelectionNumber = 0;
        foreach (AudioSource sourceToBeSetDeactiveHere in VolumeTestSources)
        {
            sourceToBeSetDeactiveHere.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (GameOptions.current != null)
        {
            volumeSettingsCurrentlyCheck[0] = GameOptions.current.Settings.VolumeMaster;
            volumeSettingsCurrentlyCheck[1] = GameOptions.current.Settings.VolumeMusic;
            volumeSettingsCurrentlyCheck[2] = GameOptions.current.Settings.VolumeSFX;
            volumeSettingsCurrentlyCheck[3] = GameOptions.current.Settings.VolumeVoice;
        }

        buttonPressTime -= Time.deltaTime;
        SelectionTimer -= Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        /*if (Controls.Return.WasPressed)
        {
            SceneManager.LoadScene("_TitleScreen");
        }*/

        if (!MenuIsOpen)
        {
            if (Controls.Return.WasPressed)
            {
                QuitToTitleScreen();
            }
            switch (MenuNav)
            {
                case OptionsMenuNavigation.Graphics:
                    DisplayArrowsOnSelection(0);
                    if (moveInput == 1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Title;
                    }
                    else if (moveInput == -1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Controls;
                    }

                    if (Controls.Confirm.WasPressed)
                    {
                        mySoundSource.Play();
                        OpenMenuItem(MenuTextSelections[0]);
                    }
                    break;
                case OptionsMenuNavigation.Controls:
                    DisplayArrowsOnSelection(1);
                    if (moveInput == 1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Graphics;
                    }
                    else if (moveInput == -1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Volume;
                    }

                    if (Controls.Confirm.WasPressed)
                    {
                        mySoundSource.Play();
                        OpenMenuItem(MenuTextSelections[1]);
                    }
                    break;
                case OptionsMenuNavigation.Volume:
                    DisplayArrowsOnSelection(2);
                    if (moveInput == 1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Controls;
                    }
                    else if (moveInput == -1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Title;
                    }

                    if (Controls.Confirm.WasPressed)
                    {
                        mySoundSource.Play();
                        OpenMenuItem(MenuTextSelections[2]);
                    }
                    break;
                case OptionsMenuNavigation.Title:
                    DisplayArrowsOnSelection(3);
                    if (moveInput == 1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Volume;
                    }
                    else if (moveInput == -1 && buttonPressTime <= 0)
                    {
                        mySoundSource.Play();
                        buttonPressTime = buttonPressReset;
                        MenuNav = OptionsMenuNavigation.Graphics;
                    }

                    if (Controls.Confirm.WasPressed)
                    {
                        mySoundSource.Play();
                        MenuTextSelections[3].color = SelectedColour;
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
                switch (OptionsOpen)
                {
                    case OptionsMenuNavigation.Graphics:
                        GraphicsMenuControls();
                        break;
                    case OptionsMenuNavigation.Controls:
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
                    case OptionsMenuNavigation.Volume:
                        VolumeMenuControls();
                        break;
                }
            }
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

    void DisplayArrowsOnSelection(int SelNumb)
    {
        foreach (GameObject gos in MenuSelections)
        {
            gos.SetActive(false);
        }
        MenuSelections[SelNumb].SetActive(true);

        foreach(Text myText in MenuTextSelections)
        {
            myText.color = DefaultColour;
        }
        MenuTextSelections[SelNumb].color = HighlightedColour;
    }

    void QuitToTitleScreen()
    {
        OptionsSaveLoad.SaveSettings();
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
        OptionsOpen = MenuNav;
        Invoke("OpenMenuItemInvokeFuction", SelectionBuffer);
    }

    void OpenMenuItemInvokeFuction()
    {
        GameObject MenuToOpenObj = null;
        switch (OptionsOpen)
        {
            case OptionsMenuNavigation.Graphics:
                MenuToOpenObj = GraphicsMenuObj;
                break;
            case OptionsMenuNavigation.Controls:
                MenuToOpenObj = ControllsMenuObj;
                break;
            case OptionsMenuNavigation.Volume:
                MenuToOpenObj = VolumeMenuObj;
                break;
        }
        GraphicsMenuObj.SetActive(false);
        ControllsMenuObj.SetActive(false);
        VolumeMenuObj.SetActive(false);
        MenuToOpenObj.SetActive(true);
        MenuIsOpen = true;
        CloseRegularMenu();
        OptionsOpen = MenuNav;
    }

    void CloseCurrentlyOpenMenu()
    {
        ReopenRegularMenu();
        GraphicsMenuObj.SetActive(false);
        ControllsMenuObj.SetActive(false);
        VolumeMenuObj.SetActive(false);
        OptionsOpen = OptionsMenuNavigation.None;
        MenuIsOpen = false;

    }

    void GraphicsMenuControls()
    {
        string LastDeviceUsed = Controls.LastDeviceStyle.ToString();
        string LastCheckDeviceUsed = LastDeviceUsed;
        //Debug.Log (InputDeviceStyle.PlayStation4.ToString());
        //int ButtonTypesGoEre = 0;
        switch (LastDeviceUsed)
        {
            default:

                GraphicsMenuButtonPromptsDisplayer(0);
                break;
            case "PlayStation4":

                GraphicsMenuButtonPromptsDisplayer(1);
                break;
            case "XboxOne":

                GraphicsMenuButtonPromptsDisplayer(2);
                break;
        }
        GraphicsCurrentExistingSettingsDisplaySetup();
        GraphicsMenuHighlightsCheck(GraphicsMenuSelectionNumber);
        GraphicsMenuMovememt();
    }

    void ControlsMenusControl(int ControllerInputType)
    {
        foreach (GameObject DisplayTypesForControls in InputForControls)
        {
            DisplayTypesForControls.SetActive(false);
        }
        InputForControls[ControllerInputType].SetActive(true);
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
                if(buttonPressTime <=0)
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

    void GraphicsMenuButtonPromptsDisplayer(int ControllerInputType)
    {

        foreach (GameObject DisplayTypesForControls in GraphicsMenuButtonPrompts)
        {
            DisplayTypesForControls.SetActive(false);
        }
        GraphicsMenuButtonPrompts[ControllerInputType].SetActive(true);
    }

    void GraphicsCurrentExistingSettingsDisplaySetup()
    {
        fullscreenIsActive = Screen.fullScreen;

        foreach (Text myTextsFull in FullScreenSettingsText)
        {
            myTextsFull.color = SettingIsOffColour;
        }
        FullScreenSettingsText[(fullscreenIsActive ? 0 : 1)].color = SettingIsOnColour;

        myGameWindowCurrentResolution = new Vector2((myCurrentCamera.pixelWidth), (myCurrentCamera.pixelHeight));

        if (myGameWindowCurrentResolution == AcceptedRes720p)
        {
            GraphicsMenuResolutionOptions = 0;
        }
        else if (myGameWindowCurrentResolution == AcceptedRes768p)
        {
            GraphicsMenuResolutionOptions = 1;
        }
        else if (myGameWindowCurrentResolution == AcceptedRes900p)
        {
            GraphicsMenuResolutionOptions = 2;
        }
        else if (myGameWindowCurrentResolution == AcceptedRes1080p)
        {
            GraphicsMenuResolutionOptions = 3;
        }
        else
        {
            GraphicsMenuResolutionOptions = -1;
        }

        foreach (Text myTextsFull in ResolutionOptionsToChange)
        {
            myTextsFull.color = SettingIsOffColour;
        }
        if (GraphicsMenuResolutionOptions >= 0)
        {
            ResolutionOptionsToChange[GraphicsMenuResolutionOptions].color = SettingIsOnColour;
        }

    }

    void GraphicsMenuMovememt()
    {
        GraphicsMenuHighlightsCheck(GraphicsMenuSelectionNumber);
        switch (GraphicsMenuSelectionNumber)
        {
            //Fullscreen Settings
            case 0:
                if (buttonPressTime <= 0)
                {
                    switch (Mathf.RoundToInt(move))
                    {
                        case 1:
                            mySoundSource.Play();
                            GraphicsMenuSelectionNumber = 1;
                            buttonPressTime = buttonPressReset;
                            break;
                        case -1:
                            mySoundSource.Play();
                            GraphicsMenuSelectionNumber = 1;
                            buttonPressTime = buttonPressReset;
                            break;
                    }
                    GraphicsSettingsFullScreenControlSettings();
                }
                GraphicsMenuHighlightsCheck(GraphicsMenuSelectionNumber);
                break;
            //Resolution Settings
            case 1:
                if (buttonPressTime <= 0)
                {
                    switch (Mathf.RoundToInt(move))
                    {
                        case 1:
                            mySoundSource.Play();
                            GraphicsMenuSelectionNumber = 0;
                            buttonPressTime = buttonPressReset;
                            break;
                        case -1:
                            mySoundSource.Play();
                            GraphicsMenuSelectionNumber = 0;
                            buttonPressTime = buttonPressReset;
                            break;
                    }
                    GraphicsMenuResolutionOptionSelectionControl();
                }
                GraphicsMenuHighlightsCheck(GraphicsMenuSelectionNumber);
                break;
        }
        GraphicsCurrentExistingSettingsDisplaySetup();
    }

    void GraphicsSettingsFullScreenControlSettings()
    {
        int selectionsMovementNumberForFullScreen = Mathf.RoundToInt(horiMove);

        if (buttonPressTime <= 0 && Mathf.Abs(selectionsMovementNumberForFullScreen) == 1)
        {
            mySoundSource.Play();
            bool isAcceptedRes = false;
            switch (fullscreenIsActive)
            {
                case true:
                    Vector2 currentResOfScreen = new Vector2 (myCurrentCamera.pixelWidth, myCurrentCamera.pixelHeight);
                    if(currentResOfScreen == AcceptedRes1080p || currentResOfScreen == AcceptedRes720p || currentResOfScreen == AcceptedRes768p || currentResOfScreen == AcceptedRes900p)
                    {
                        isAcceptedRes = true;
                    }
                    else
                    {
                        isAcceptedRes = false;
                        Debug.Log("Screen was not an acceptable resolution.");
                    }
                    switch (isAcceptedRes) {
                        case true:
                            Screen.SetResolution( myCurrentCamera.pixelWidth /*Screen.currentResolution.width*/, myCurrentCamera.pixelHeight /*Screen.currentResolution.height*/, false);
                            break;
                        case false:
                            Screen.SetResolution(1280, 720, false);
                            break;
                    }
                    fullscreenIsActive = false;
                    Debug.Log("Turned off Fullscreen");
                    break;
                case false:
                    Screen.SetResolution(myCurrentCamera.pixelWidth /*Screen.currentResolution.width*/, myCurrentCamera.pixelHeight /*Screen.currentResolution.height*/, true);
                    fullscreenIsActive = true;
                    Debug.Log("Turned on Fullscreen");
                    break;
                default:
                    break;
            }
            buttonPressTime = buttonPressReset;
        }

    }

    void GraphicsMenuResolutionOptionSelectionControl()
    {
        int ResolutionSelecter = Mathf.RoundToInt(horiMove);

        switch (GraphicsMenuResolutionOptions)
        {
            default:
                if (Mathf.Abs(ResolutionSelecter) == 1 && buttonPressTime <=0)
                {
                    Screen.SetResolution(1280, 720, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    //myCurrentCamera.
                    Debug.Log("right");
                buttonPressTime = buttonPressReset;
                }
                break;
            case 0:
                Screen.SetResolution(1280, 720, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));

                if (ResolutionSelecter < 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1920, 1080, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 3;
                    buttonPressTime = buttonPressReset;
                }
                else if (ResolutionSelecter > 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1366, 768, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 1;
                    Debug.Log("right");
                    buttonPressTime = buttonPressReset;
                }
                break;
            case 1:
                Screen.SetResolution(1366, 768, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));

                if (ResolutionSelecter < 0 && buttonPressTime <= 0)
                {

                    Screen.SetResolution(1280, 720, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 0;
                    buttonPressTime = buttonPressReset;
                }
                else if (ResolutionSelecter > 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1600, 900, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 2;
                    Debug.Log("right");
                    buttonPressTime = buttonPressReset;
                }
                break;
            case 2:
                Screen.SetResolution(1600, 900, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));

                if (ResolutionSelecter < 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1366, 768, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 1;
                    buttonPressTime = buttonPressReset;
                }
                else if (ResolutionSelecter > 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1920, 1080, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 3;
                    Debug.Log("right");
                    buttonPressTime = buttonPressReset;
                }
                break;
            case 3:
                Screen.SetResolution(1920, 1080, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));

                if (ResolutionSelecter < 0 && buttonPressTime <= 0)
                {
                    Screen.SetResolution(1600, 900, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 2;
                    buttonPressTime = buttonPressReset;
                }
                else if (ResolutionSelecter > 0 && buttonPressTime <= 0)
                {

                    Screen.SetResolution(1280, 720, (fullscreenIsActive ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed));
                    GraphicsMenuResolutionOptions = 0;
                    Debug.Log("right");
                    buttonPressTime = buttonPressReset;
                }
                break;
        }
    }

    void GraphicsMenuHighlightsCheck(int SelectedMenuItemForGraphics)
    {
        foreach (GameObject arrowsMan in GraphicsMenuSelectionItems)
        {
            arrowsMan.SetActive(false);

        }
        GraphicsMenuSelectionItems[SelectedMenuItemForGraphics].SetActive(true);
    }
}

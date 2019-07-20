using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    PlayerController Controls;

    public AudioSource myAudioSource, MenuAudioSource;

    public int MenuOpeningSequence;
    public float buttonPressReset = .2f;
    public float buttonPressTime = 0;
    public float fadeInIntervalDecimal = 0.001f;
    public float MenuDarkTargetAlphaColour = .75f;

    float VertInput = 0;
    public int move = 0;

    public GameObject myCanvasObj;

    [Header("Menu Stuff")]
    public Image MenuDark;
    public GameObject[] MenuObjects;
    public GameObject[] InputHelp;
    public int MenuItemSelectionNumber;
    public GameObject[] SelectionArrows;
    public Text[] SelectionTexts;
    public Color DefaultTextColour, HighlightColour, SelectedColour;

    // Use this for initialization
    void Start () {
        Controls = PlayerController.CreateWithDefaultBindings();
        MenuOpeningSequence = 0;
        buttonPressTime = 0;
        myCanvasObj.SetActive(false);
        MenuItemSelectionNumber = 0;
        move = 0;
	}

    private void Update()
    {
        buttonPressTime -= Time.unscaledDeltaTime;
    }

    // Update is called once per frame
    void  FixedUpdate () {

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
            //fade in
            case 1:
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
                    MenuOpeningSequence = 2;
                }
                break;
                //menu appeared
            case 2:
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
                        if (Mathf.Abs(move) > 0 && buttonPressTime <=0)
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
                            RestartLevelChosen();
                        }

                        break;
                    case 1:
                        if (Mathf.Abs(move) > 0 && buttonPressTime <= 0)
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
                            QuitToTitleScreen();
                        }

                        break;
                }


                break;
        }

	}



    public void GameOverReached()
    {
        myCanvasObj.SetActive(true);
        TurnOffMenuObjects();
        Color addToMenuColour = MenuDark.color;
        addToMenuColour.a = 0;
        MenuDark.color = addToMenuColour;
        MenuOpeningSequence = 1;
        MenuItemSelectionNumber = 0;
        //myAudioSource.Play();
    }

    void RestartLevelChosen()
    {
        myAudioSource.Stop();
        MenuAudioSource.Stop();
        MenuOpeningSequence = 0;
        TurnOffMenuObjects();
        Color addToMenuColour = MenuDark.color;
        addToMenuColour.a = 0;
        MenuDark.color = addToMenuColour;

        myCanvasObj.SetActive(false);

        LevelGM.Instance.PlayerRespawn();
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
        foreach(GameObject gos in MenuObjects)
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
        SceneManager.LoadScene("_TitleScreen");
    }

    void DisplayCurrentSelection(int currentSel)
    {
        foreach(GameObject gos in SelectionArrows)
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
}

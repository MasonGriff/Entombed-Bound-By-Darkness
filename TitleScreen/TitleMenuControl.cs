using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InControl;

public class TitleMenuControl : MonoBehaviour {

    PlayerController Controls;

    [SerializeField]
    public string NEwGameScene;

    public Color DefaultColour, HighlightedColour, SelectedColour, DeactivatedColour;

    public int SelectionNumber = 0;

    public GameObject[] MenuSelections = new GameObject[4];

    public Text[] MenuSelectionText = new Text[4];

    public float VertInput;
    public int MoveInput;

    public GameObject[] ClickableTitleScreenButtons = new GameObject[4];

    float move;
    float buttonPressTime;
    public float rechargeTime = 0.1f;
    public float SelectionBuffer = 0.1f;
    float SelectionTimer = 0f;

    public bool LoadAvailable = false;

    private int TimesOne = 1;

    bool CantDoAnythingWeAreLoading = false;

    public GameObject[] InputHelp;

    private void Awake()
    {

        LoadAvailable = false;
        if (GameOptions.current == null) //If there's no saved game options.
        {
            Debug.Log("New Options Started");
            OptionsSaveLoad.LoadSettings(); //Creates a new default game options.
            
        }
        else
        {
            Debug.Log("Options Exist");
        }
       
    }

    // Use this for initialization
    void Start () {
        CantDoAnythingWeAreLoading = false;
        if (Game.current != null)
        {
            Game.current = null;
        }
        Controls = PlayerController.CreateWithDefaultBindings();
        SaveAndLoad.LoadExists();
        buttonPressTime = rechargeTime;
        SelectionTimer = SelectionBuffer;
        if (SaveAndLoad.SaveExists || SaveAndLoad.Save2Exists)
        {
            LoadAvailable = true;
        }
        else
        {
            LoadAvailable = false;
        }
        /*switch (SaveAndLoad.SaveExists)
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
        }*/
        switch (LoadAvailable)
        {
            case false:
                SelectionNumber = 1;
                foreach (GameObject items in MenuSelections)
                {
                    items.SetActive(false);
                }
                foreach (Text itemText in MenuSelectionText)
                {
                    itemText.color = DefaultColour;
                }
                MenuSelectionText[0].color = DeactivatedColour;
                ClickableTitleScreenButtons[0].SetActive(false);
                MenuSelections[SelectionNumber].SetActive(true);
                MenuSelectionText[SelectionNumber].color = HighlightedColour;
                break;
            case true:
                SelectionNumber = 0;
                
                break;
        }
    }

    // Update is called once per frame

    void Update()
    {
        buttonPressTime -= Time.deltaTime;
        SelectionTimer -= Time.deltaTime;
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

        //Game.current
    }

    private void FixedUpdate()
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


        {//Input for up and down;
            /*if (Controls.Up.WasRepeated && SelectionTimer <= 0)
            {
                MoveInput = 1;
            }
            else if (Controls.Down.WasRepeated && SelectionTimer <= 0)
            {
                MoveInput = -1;
            }
            else
            {
                MoveInput = 0;
            }*/
            
            if (Controls.Return)
            {
                QuitGameGo();
            }
            
            //Prepares input for int value;
            MoveInput = Mathf.RoundToInt(VertInput);

            //Determines if up or down were pressed.
            if (SelectionTimer <= 0)
            {
                switch (MoveInput)
                {
                    case 1:
                        SelectionNumber-=1;
                        SelectionTimer = SelectionBuffer;
                        break;
                    case -1:
                        SelectionNumber+=1;
                        SelectionTimer = SelectionBuffer;
                        break;
                }
            }

            if (SelectionNumber >= 4) //Count too high.
            {
                SelectionNumber = (LoadAvailable ? 0 : 1);
            }
            else if (SelectionNumber <= 0) //Count too low.
            {
                switch (LoadAvailable)
                {
                    case true:
                        SelectionNumber = 0;
                        break;
                    case false:
                        SelectionNumber = 3;
                        break;
                }
            }

            //Highlights correct menu item.
            foreach (GameObject items in MenuSelections)
            {
                items.SetActive(false);
            }
            foreach (Text itemText in MenuSelectionText)
            {
                itemText.color = DefaultColour;
            }
            if (!LoadAvailable)
            {

                MenuSelectionText[0].color = DeactivatedColour;
            }
            MenuSelections[SelectionNumber].SetActive(true);
            MenuSelectionText[SelectionNumber].color = HighlightedColour;

            if (Controls.Confirm.WasPressed)
            {
                CantDoAnythingWeAreLoading = true;

                MenuSelectionText[SelectionNumber].color = SelectedColour;
                switch (SelectionNumber)
                {
                    case 0:
                        LoadGameGo();
                        break;
                    case 1:
                        NewGameGo();
                        break;
                    case 2:
                        MyOptionsGo();
                        break;
                    case 3:
                        QuitGameGo();
                        break;
                }
            }
        }
    }

    public void NewGameGo()
    {
        TurnOffTheButtons();
        SceneManager.LoadScene(NEwGameScene);
    }

    public void LoadGameGo()
    {
        TurnOffTheButtons();
        SceneManager.LoadScene("LoadGame");
    }
    public void MyOptionsGo()
    {
        TurnOffTheButtons();
        SceneManager.LoadScene("Options");
    }

    public void QuitGameGo()
    {
        TurnOffTheButtons();
        Application.Quit();
    }

    void TurnOffTheButtons()
    {
        foreach (GameObject Clickies in ClickableTitleScreenButtons)
        {
            Clickies.SetActive(false);
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

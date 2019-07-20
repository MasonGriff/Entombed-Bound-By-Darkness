using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CostumeSelectScreen : MonoBehaviour {

    PlayerController Controls;

    public int currentHighlightedOutfit;

    public bool[] IsItUnlocked;
    public GameObject[] CostumeSets;
    public Color UnselectedColour, HighlightedColour, SelectedColour, UnavailableColour;
    public Text[] CostumeNames;
    public GameObject[] CostumeDescription;

    public int VertMove;

    public float buttonTimer = 0;
    public float buttonReset = .2f;

    public GameObject[] SelectionArrows;
    Player_States.OutfitSettings currOutfit;

    // Use this for initialization
    void Start () {

        Controls = PlayerController.CreateWithDefaultBindings();
        VertMove = 0;
        CheckForAvailability();
        CheckUnlockProgress();
        CheckForSelected();
        buttonTimer = buttonReset;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        CheckForAvailability();
        buttonTimer -= Time.deltaTime;
        CheckForAvailability();

        CheckForSelectedGo();
        //currentHighlightedOutfit = 0;
	}

    private void FixedUpdate()
    {
        float moveInput2 = Controls.Move.Y;
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
        
        VertMove = Mathf.RoundToInt(moveInput2);

        CheckForAvailability();

        switch (StageSelect.Instance.CheckingOutfits)
        {
            case true:
                MenuIsOpen();
                break;
            case false:
                NotOpenMenu();
                break;
        }
        
    }

    void CheckUnlockProgress()
    {
        if (Game.current.Progress.EnemyParts[0] && Game.current.Progress.EnemyParts[1]&& Game.current.Progress.EnemyParts[2])
        {
            Game.current.Progress.AltEnemy = Player_States.OutfitUnlocks.Unlocked;
        }
        if (Game.current.Progress.EnlightenedParts[0] && Game.current.Progress.EnlightenedParts[1] && Game.current.Progress.EnlightenedParts[2])
        {
            Game.current.Progress.AltEnlightened = Player_States.OutfitUnlocks.Unlocked;
        }
        if (Game.current.Progress.ShadowParts[0] && Game.current.Progress.ShadowParts[1] && Game.current.Progress.ShadowParts[2])
        {
            Game.current.Progress.AltShadow = Player_States.OutfitUnlocks.Unlocked;
        }
        if (Game.current.Progress.BossPart)
        {
            Game.current.Progress.AltBoss = Player_States.OutfitUnlocks.Unlocked;
        }
        if (Game.current.Progress.BasicPart)
        {
            Game.current.Progress.AltBasic = Player_States.OutfitUnlocks.Unlocked;
        }
    }

    void CheckForAvailability()
    {
        IsItUnlocked[0] = true;
        CrossReferenceWithSave(1, Game.current.Progress.AltEnemy);
        CrossReferenceWithSave(2, Game.current.Progress.AltShadow);
        CrossReferenceWithSave(3, Game.current.Progress.AltEnlightened);
        CrossReferenceWithSave(4, Game.current.Progress.AltBoss);
        CrossReferenceWithSave(5, Game.current.Progress.AltBasic);
    }

    void CheckForSelected()
    {
        CheckForSelectedGo();
        switch (currOutfit)
        {
            case Player_States.OutfitSettings.Default:
                currentHighlightedOutfit = 0;

                break;
            case Player_States.OutfitSettings.Enemy:
                currentHighlightedOutfit = 1;

                break;
            case Player_States.OutfitSettings.Shadow:
                currentHighlightedOutfit = 2;

                break;
            case Player_States.OutfitSettings.Enlightened:
                currentHighlightedOutfit = 3;

                break;
            case Player_States.OutfitSettings.Boss:
                currentHighlightedOutfit = 4;

                break;
            case Player_States.OutfitSettings.Basic:
                currentHighlightedOutfit = 5;

                break;
        }
    }

    void CheckForSelectedGo()
    {
        currOutfit = Game.current.Progress.CurrentOutfit;
        switch (currOutfit)
        {
            case Player_States.OutfitSettings.Default:
                CostumeNames[0].color = SelectedColour;

                break;
            case Player_States.OutfitSettings.Enemy:
                CostumeNames[1].color = SelectedColour;

                break;
            case Player_States.OutfitSettings.Shadow:
                CostumeNames[2].color = SelectedColour;

                break;
            case Player_States.OutfitSettings.Enlightened:
                CostumeNames[3].color = SelectedColour;

                break;
            case Player_States.OutfitSettings.Boss:
                CostumeNames[4].color = SelectedColour;

                break;
            case Player_States.OutfitSettings.Basic:
                CostumeNames[5].color = SelectedColour;

                break;
        }
    }

    void HideCostumeNames()
    {
        CostumeNames[0].color = UnselectedColour;
        HideNameSetupGo(IsItUnlocked[1], CostumeNames[1]);
        HideNameSetupGo(IsItUnlocked[2], CostumeNames[2]);
        HideNameSetupGo(IsItUnlocked[3], CostumeNames[3]);
        HideNameSetupGo(IsItUnlocked[4], CostumeNames[4]);
        HideNameSetupGo(IsItUnlocked[5], CostumeNames[5]);
    }

    void HideNameSetupGo(bool OutfitAvailability, Text ThisCostName)
    {
        switch (OutfitAvailability)
        {
            case true:
                ThisCostName.color = UnselectedColour;

                break;
            case false:
                ThisCostName.color = UnavailableColour;
                ThisCostName.text = "???";
                break;
        }
    }

    void CrossReferenceWithSave(int OutfitAvailability, Player_States.OutfitUnlocks UnlockStatusInSave)
    {
        IsItUnlocked[OutfitAvailability] = ((UnlockStatusInSave == Player_States.OutfitUnlocks.Unlocked) ? true : false);
        
        //Debug.Log(UnlockStatusInSave.ToString());
    }

    void DisplaySelectedOutfit()
    {
        foreach (GameObject gos in CostumeSets)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in CostumeDescription)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in SelectionArrows)
        {
            gos.SetActive(false);
        }
        SelectionArrows[currentHighlightedOutfit].SetActive(true);
        CostumeSets[currentHighlightedOutfit].SetActive(true);
        CostumeDescription[currentHighlightedOutfit].SetActive(true);
        CostumeNames[currentHighlightedOutfit].color = HighlightedColour;
    }

    //Not Open
    void NotOpenMenu()
    {
        if (Input.GetKey(KeyCode.P))
        {

        }
        CheckForSelected();
        DisplaySelectedOutfit();
        buttonTimer = buttonReset;
    }
    //Open
    void MenuIsOpen()
    {

        HideCostumeNames();
        CheckForSelectedGo();
        DisplaySelectedOutfit();
        if (Controls.Return.WasPressed || Controls.ClearMind.WasPressed && buttonTimer <=0)
        {
            buttonTimer = buttonReset;
            StageSelect.Instance.buttonTimer = StageSelect.Instance.buttonReset;
            StageSelect.Instance.CheckingOutfits = false;
        }
        else
        {
            if (Mathf.Abs(VertMove) >= 1 && buttonTimer <=0)
            {
                buttonTimer = buttonReset;
                switch (VertMove)
                {
                    case 1:
                        CheckNextAvailable(false);
                        break;
                    case -1:
                        CheckNextAvailable(true);
                        break;
                }
                HideCostumeNames();
                CheckForSelectedGo();
                DisplaySelectedOutfit();

            }
            else if(Controls.Confirm.WasPressed && buttonTimer <= 0)
            {

                buttonTimer = buttonReset;
                IHavePickedOne();

            }
            HideCostumeNames();
            CheckForSelectedGo();
            DisplaySelectedOutfit();
        }
    }


    void CheckNextAvailable(bool AddIt)
    {
        if (AddIt)
        {
            currentHighlightedOutfit += 1;
        }
        else if (!AddIt)
        {
            currentHighlightedOutfit-=1;
        }
        if (currentHighlightedOutfit > 5)
        {
            Debug.Log("loop up");
            currentHighlightedOutfit = 0;
        }
        if (currentHighlightedOutfit < 0)
        {
            Debug.Log("loop down");
            currentHighlightedOutfit = 5;
        }

        if (IsItUnlocked[currentHighlightedOutfit] == false)
        {
            Debug.Log("toNext");
            CheckNextAvailable(AddIt);
        }
    }


    void IHavePickedOne()
    {
        switch (currentHighlightedOutfit)
        {
            case 0:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Default;

                break;
            case 1:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Enemy;

                break;
            case 2:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Shadow;

                break;
            case 3:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Enlightened;

                break;
            case 4:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Boss;

                break;
            case 5:
                Game.current.Progress.CurrentOutfit = Player_States.OutfitSettings.Basic;

                break;
        }
    }
}

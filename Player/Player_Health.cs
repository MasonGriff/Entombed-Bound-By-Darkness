using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player_Main))]
public class Player_Health : MonoBehaviour {
    /// <summary>
    /// This script manages the player's health values and health related things.
    /// </summary> 
  
    Player_Main Player; //Reference to head of player controller.
    //public bool DomahdAlive = true; //Domahd is still alive.

    [Header("Floats")]
    [Tooltip("Full Health's value.")]
    public int MaxHealth = 4;
    [Tooltip("Current health.")]
    public int HP;
    public float HPBarDecreaseInterval = 5f;
    public float PreClearMindUnlockedTimer = 0, ClearMindTimerInterval;

    [Header("Object Reference")]
    //public Image HealthBar;
    public GameObject[] HideableHud;
    public Slider[] HPBar, CMBar;
    public Image ClearMindDarkness;
    public GameObject GhostSpawning;
    public Image StatusAilments;
    public Image ReversedTimerBar;
    public GameObject HideHP, HideCM1, HideCM2, HideCMHalf;

    [Header("Sprites")]
    public Sprite[] HPSprite;
    public Sprite NormalState, ReversedState, CloudedState, DarknessState;

    [Header("Colours")]
    public Color defaultColour;
    public Color ReversedColour;



    private void Awake() 
    {
        Player = GetComponent<Player_Main>();
        
    }

    // Use this for initialization
    void Start () {
        //HealthBar.gameObject.SetActive(false);
        //Player.HudObj.SetActive(false);
        //DomahdAlive = true;
        HideCM1.SetActive(false);
        HideCM2.SetActive(false);
        HideCMHalf.SetActive(false);
        HideHP.SetActive(false);

        if (Game.current.Progress.CurrentDifficulty == Player_States.DifficultySetting.Hard)
        {
            MaxHealth = 1;

            HideHP.SetActive(true);
            foreach (GameObject gos in HideableHud)
            {
                gos.SetActive(false);
            }
        }

        if (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            HideCM2.SetActive(true);
            HideCMHalf.SetActive(true);
            ClearMindPreUnlockDisplayUpdate();

        }
        else if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            HideCM1.SetActive(true);
            HideCM2.SetActive(true);
        }
        else
        {
            switch (Game.current.Progress.ClearMindLevel)
            {
                case Player_States.ClearMindUpgrade.Level1:
                    HideCM2.SetActive(true);
                    HideCM1.SetActive(false);
                    break;
            }

            ClearMindUpdating();
        }
        HP = MaxHealth;
        if (CameraController.Instance.ClearMindGhostSpawn != null)
        { GhostSpawning = CameraController.Instance.ClearMindGhostSpawn; }
    }
	


	// Update is called once per frame
	void Update ()
    {
        

        if (CameraController.Instance.ClearMindGhostSpawn != null)
        {
            Vector3 newZpos = GhostSpawning.transform.position;
            newZpos.z = transform.position.z;
            GhostSpawning.transform.position = newZpos;
        }

        if (HP <= 0)
        {
            HP = 0;
        }
        HudSpriteUpdating(HPSprite); //Keeps hud up to date with displaying health.
        HealthStateUpdating(); //Keeps the PlayerHealthState updated to based on what the current health is.
        if (Player.PlayerHealthState == Player_States.PlayerHealthStates.Normal)
        {
            Player.myAnim.SetBool("LowHealth", false);
        }
        else if (Player.PlayerHealthState == Player_States.PlayerHealthStates.LowHealth)
        {
            Player.myAnim.SetBool("LowHealth", true);
        }
        StatusAilmentsUpdate();
        DebuffHudDisplay();
	}
    void FixedUpdate()
    {
        if (HP <= 0)
        {
            HP = 0;
        }
        if (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            ClearMindPreUnlockDisplayUpdate();
        }
        else if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {

            ClearMindPreUnlockDisplayUpdate();
        }
        else
        {
            ClearMindUpdating();
        }
        HudSpriteUpdating(HPSprite); //Keeps hud up to date with displaying health.
        //HudSpriteUpdating(HPSprite); //Keeps hud up to date with displaying health.
        HealthStateUpdating(); //Keeps the PlayerHealthState updated to based on what the current health is.
        StatusAilmentsUpdate();
        DebuffHudDisplay();
        if (Player.PlayerHealthState == Player_States.PlayerHealthStates.Normal)
        {
            Player.myAnim.SetBool("LowHealth", false);
        }
        else if (Player.PlayerHealthState == Player_States.PlayerHealthStates.LowHealth)
        {
            Player.myAnim.SetBool("LowHealth", true);
        }
    }

    void DebuffHudDisplay()
    {
        float invertCurrent = Player.MovementScript.invertTimer;
        float invertMax = GhostZahra.InvertTimer;

        float invertPercent = (invertCurrent / invertMax);
        //Debug.Log("Invert Percent = " + invertPercent);
        ReversedTimerBar.fillAmount = invertPercent;
    }

    void HudSpriteUpdating(Sprite[] HealthSprite)
    {
        if (!LevelGM.Instance.isPaused)
        {
            //HealthBar.gameObject.SetActive(true);
            //HealthBar.sprite = HealthSprite[HP];
            //HealthBar.color = ((Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls) ? ReversedColour : defaultColour);
            if (HP == MaxHealth)
            {
                foreach (Slider gos in HPBar)
                {
                    gos.value = 100;
                }
            }
            else if (HP == 1)
            {
                HPBar[0].value = 100;
                if (HPBar[1].value > 0)
                {
                    HPBar[1].value -= HPBarDecreaseInterval;
                }
            }
            else if (HP <= 0)
            {
                if (HPBar[1].value > 0)
                {
                    HPBar[1].value -= HPBarDecreaseInterval;
                }
                if (HPBar[0].value > 0 && HPBar[1].value == 0)
                {
                    HPBar[0].value -= HPBarDecreaseInterval;
                }
            }
        }
        else
        {

            //HealthBar.gameObject.SetActive(false);
        }
    }

    void StatusAilmentsUpdate()
    {
        if (Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            if (Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls)
            {
                StatusAilments.sprite = ReversedState;

            }
            else
            {
                StatusAilments.sprite = NormalState;
            }
        }
        else
        {
            if (Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls)
            {
                StatusAilments.sprite = ReversedState;

            }
            else
            {
                if (CMBar[0].value <= 0)
                {
                    StatusAilments.sprite = DarknessState;
                }
                else
                {
                    if (CMBar[1].value <= 0 && CMBar[0].value < 100)
                    {
                        StatusAilments.sprite = CloudedState;
                    }
                    else
                    {
                        StatusAilments.sprite = NormalState;
                    }
                }
            }
        }
    }

    void ClearMindPreUnlockDisplayUpdate()
    {
        CMBar[1].value = 0;
        CMBar[0].value = LevelGM.Instance.ClearMindValue[0];
        if (CMBar[0].value <= 0)
        {
            Player.ClearMindOverused = true;
        }
    }

    void ClearMindUpdating()
    {
        CMBar[0].value = LevelGM.Instance.ClearMindValue[0];
        CMBar[1].value = LevelGM.Instance.ClearMindValue[1];

        float darkBuffer = LevelGM.Instance.ClearMindValue[0] / 100;
        float darkValueToBe = 1 - darkBuffer;

        Color DarknessAlpha = ClearMindDarkness.color;
        float newAlphaToBe = (darkValueToBe);
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("Clear Mind Darkness Alpha is " + newAlphaToBe);
        }
        DarknessAlpha.a = newAlphaToBe;
        ClearMindDarkness.color = DarknessAlpha;
        if (LevelGM.Instance.ClearMindValue[0] <= 0)
        {
            if (!(GhostSpawning == null) && Player.PlayerState == Player_States.PlayerStates.ClearMind)
            {
                GhostSpawning.SetActive(true);
            }
        }

    }

    void HealthStateUpdating()
    {
       

        if (HP > (MaxHealth)/2) //Good Health
        {
            Player.PlayerHealthState = Player_States.PlayerHealthStates.Normal;
        }
        if(HP <= (MaxHealth/2) && HP > 0) //Low Health
        {
            Player.PlayerHealthState = Player_States.PlayerHealthStates.LowHealth;
        }
        if (HP <= 0) //Zahra Dead.
        {
            HP = 0;
            Player.PlayerHealthState = Player_States.PlayerHealthStates.Dead;
        }
    }

    public void HealthLost(int lostHealth)  //Damage has been taken. This function is called on by the main player controller's DamageTaken() function.
    {
        HP-=lostHealth;
        Debug.Log("Damage Taken");
    }

}

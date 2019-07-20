using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGM : MonoBehaviour {
 
    /// <summary>
    /// This is the main Game Master script that controls things and keeps track of them within a scene.
    /// </summary>


    public static LevelGM Instance { get; set; } //Sets the Instance of the script to this current gameobject. You can reference the script ithout needing the gameobject via LevelGM.Instance



    public bool isPaused;
    //public string ControllerInput = " Xbox"; //This is the controller input for if it's set to Dualshock4 or Xbox Controller.
    public Transform CurrentCheckpoint; //The checkpoint the player will respawn from on death. On the scene first opening, it's the starting point of the player.
    public GameObject Player; //Prefab: This is the prefab of the player that's resummoned on spawn.
    public GameObject PlayerInLevel; //This is the current instance of the player in the level.
    public GameObject MainCameraObj;

    public bool playDeath = false;

    public AudioSource BackgroundMusicSource;
    [Header("Float Values")]
    public float[] ClearMindValue = new float[2];
    public float ClearMindIncreaseInterval = 1;

    [Header("Camera Defaults")]
    //These are the minimum and maximum distances the camera will follow the player. On spawning/respawning, 
    //the camera's version of these variables will be set by these as the default.
    public Vector3 DefaultVertMinimum;
    public Vector3 DefaultVertMaximum;
    public Vector3 DefaultHoriMinimum;
    public Vector3 DefaultHoriMaximum;

    
    [Header("Hallucinations")]
    public GameObject HalicinationObjectsGroup;
    public bool ClearMind = false;
    public GameObject GhostSpawnGroup;
    public GameObject[] GhostPresets;
    public GameObject ActiveGhostSpot;
    public bool GhostIsActive = false;

    [Header("Death and Game Over")]
    public GameObject GameOverScreen;
    //public AudioSource GameOverSong;
    public GameOver GameOverScript;

    [HideInInspector]
    public ABCheckingScript PathScript;
    public int DeathCount= 0;
    public float myPlaytime = 0;
    public bool ghostCheck;
    public bool NoDamage;
    public bool RespawnSFXCanPlay;

    private void Awake()
    {
        RespawnSFXCanPlay = false;
        Instance = this; //Sets the current instance of the script to this current gameobject in the scene.
        if (Game.current == null) //If this isn't on a save file.
        {
            Game.current = new Game(); //Creates new default save file.
        }
        if (GameOptions.current == null) //If there's no saved game options.
        {
            OptionsSaveLoad.LoadSettings();
        }

        
    }
    // Use this for initialization
    void Start()
    {
        GhostIsActive = false;
        ghostCheck = false;
        NoDamage = true;
        if (GhostSpawnGroup != null && GhostPresets != null)
        {
            GhostSpawnGroup.SetActive(true);
            int randomNumberGen = Random.Range(0, GhostPresets.Length - 1);

            ActiveGhostSpot = GhostPresets[randomNumberGen];
            foreach (GameObject gos in GhostPresets)
            {
                gos.SetActive(false);
            }
            ActiveGhostSpot.SetActive(true);
            GhostSpawnGroup.SetActive(false);
        }


        DeathCount = 0;
        ClearMind = false;
        playDeath = false;
        if (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            ClearMindValue[0] = 50;
            ClearMindValue[1] = 0;
        }
        else if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {

            ClearMindValue[1] = 0;
            ClearMindValue[0] = 0;
        }
        else if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.Level1)
        {

            ClearMindValue[1] = 0;
            ClearMindValue[0] = 100;
        }
        else
        {
            ClearMindValue[0] = 100;
            ClearMindValue[1] = 100;
        }
        //These get the default camera minimum and maximum positions on fist starting the scene.
        DefaultVertMinimum = CameraController.Instance.VertMinimum;
        DefaultVertMaximum = CameraController.Instance.VertMaximum;
        DefaultHoriMinimum = CameraController.Instance.HoriMinimum;
        DefaultHoriMaximum = CameraController.Instance.HoriMaximum;
        if (PlayerInLevel == null) //Spawns player if there's no instance of the player prefab in the scene prior to first loading.
        {
            PlayerRespawn();
        }
        else //Deletes player prefab instance in the scene on loading and spawns in a new one.
        {
            Destroy(PlayerInLevel);
            PlayerRespawn();
        }
        //Debug.Log(System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + "/" + System.DateTime.Now.Year);
        RespawnSFXCanPlay = true;
    }

    private void FixedUpdate()
    {
        isPaused = Game.current.Progress.GameplayPaused; //Checks if paused.
        if (HalicinationObjectsGroup != null)
        {
            if (ClearMind == true)
            {
                if (ClearMindValue[1] > 0)
                {
                    ClearMindValue[1] -= ClearMindIncreaseInterval;
                }
                else if (ClearMindValue[1] <= 0)
                {
                    ClearMindValue[1] = 0;
                    ClearMindValue[0] -= ClearMindIncreaseInterval;
                }
                else if (ClearMindValue[0] <= 0)
                {

                    ClearMindValue[0] = 0;
                    ClearMindValue[1] = 0;
                }
                HalicinationObjectsGroup.SetActive(false);
            }
            else
            {
                HalicinationObjectsGroup.SetActive(true);
            }
        }

        if (!(Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached))
        {
            if (ClearMindValue[0] <= 0 && ClearMind)
            {
                if (GhostSpawnGroup!= null)
                {
                    GhostSpawnGroup.SetActive(true);
                    ghostCheck = false;
                }
            }
            if (!ClearMind && ActiveGhostSpot.GetComponent<GhostZahraMaster>().GhostChild == null)
            {
                if (GhostSpawnGroup != null)
                {
                    
                    
                    if (!ghostCheck)
                    {
                        int randomNumberGen = Random.Range(0, GhostPresets.Length - 1);

                        ActiveGhostSpot = GhostPresets[randomNumberGen];
                        foreach (GameObject gos in GhostPresets)
                        {
                            gos.SetActive(false);
                        }
                        ActiveGhostSpot.SetActive(true);
                        ghostCheck = true;
                    }
                    GhostSpawnGroup.SetActive(false);
                }
            }
        }
        UpdateDeath();

    }

    // Update is called once per frame
    void Update () {
        //playDeath = false;
        Game.current.Progress.TotalPlaytime += Time.unscaledDeltaTime;
        myPlaytime += Time.deltaTime;
        UpdateDeath();

        isPaused = Game.current.Progress.GameplayPaused; //Checks if paused.
        {
           /* if (HalicinationObjectsGroup != null)
            {
                if (ClearMind == true)
                {
                    if (ClearMindValue[0] < 100)
                    {
                        ClearMindValue[0] += ClearMindIncreaseInterval;
                    }
                    else if (ClearMindValue[0] >= 100)
                    {
                        ClearMindValue[0] = 100;
                        ClearMindValue[1] += ClearMindIncreaseInterval;
                    }
                    else if (ClearMindValue[1] >= 100)
                    {

                        ClearMindValue[0] = 100;
                        ClearMindValue[1] = 100;
                    }
                    HalicinationObjectsGroup.SetActive(false);
                }
                else
                {
                    HalicinationObjectsGroup.SetActive(true);
                }
            }*/
        }

        //if (Input.GetKeyDown(KeyCode.Escape)) //Tempoarary return to title screen until pause menu is set up properly again.
        //{
        //    SceneManager.LoadScene("_TitleScreen");
        //}
        //if(PlayerInLevel == null)
        //{
         //   playDeath = true;
         //   PlayerRespawn();
        //}
	}

    
    
    public void PlayerRespawn() //Respawn the player prefab.
    {
        playDeath = false;
        if (PlayerInLevel != null)
        {

            Destroy(PlayerInLevel);
        }

        BackgroundMusicSource.mute = false;
        BackgroundMusicSource.UnPause();

        CameraController.Instance.HoriMinimum = DefaultHoriMinimum;
        CameraController.Instance.HoriMaximum = DefaultHoriMaximum;
        CameraController.Instance.VertMinimum = DefaultVertMinimum;
        CameraController.Instance.VertMaximum = DefaultVertMaximum;

        CameraVectorChange.ClearCameraChange();
        PlayerInLevel = GameObject.Instantiate(Player, new Vector3(CurrentCheckpoint.position.x, CurrentCheckpoint.position.y, CurrentCheckpoint.position.z), new Quaternion(CurrentCheckpoint.rotation.x, CurrentCheckpoint.rotation.y, CurrentCheckpoint.rotation.z, CurrentCheckpoint.rotation.w));
        CameraController.Instance.Player = PlayerInLevel;
    }

    public void PlayerDeath(GameObject OldPlayer) //Delete old player that just died and call the respawn function.
    {
        SaveAndLoad.Save(Game.current.Progress.FileNumber);
        DeathCount++;
        playDeath = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Destroy(OldPlayer);
        //FallingPlatformMaster.AutoRecallPlatformSpawn();
        //PlayerRespawn();
        //GameOverScreen.SetActive(true);
        BackgroundMusicSource.mute = true;
        BackgroundMusicSource.Pause();
        GameOverScript.GameOverReached();
        //GameOverSong.Play();
        
    }

    public void PlayDeathTune()
    {
        GameOverScript.myAudioSource.Play();
    }

    void UpdateDeath()
    {
        switch (PathScript.LevelSet)
        {
            case Player_States.LevelSetNumber.Levels1:
                switch (PathScript.StageSet)
                {
                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level1Deaths[0] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level1Deaths[1] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level1Deaths[2] = DeathCount;
                        break;
                }
                break;
            case Player_States.LevelSetNumber.Levels2:
                switch (PathScript.StageSet)
                {
                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level2Deaths[0] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level2Deaths[1] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level2Deaths[2] = DeathCount;
                        break;
                }
                break;
            case Player_States.LevelSetNumber.Levels3:
                switch (PathScript.StageSet)
                {
                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level3Deaths[0] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level3Deaths[1] = DeathCount;
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level3Deaths[2] = DeathCount;
                        break;
                }
                break;
            case Player_States.LevelSetNumber.FinalBoss:

                Game.current.Progress.FinalBossDeaths = DeathCount;
                break;

        }


    }

    public void UpdateTime()
    {
        myPlaytime = Mathf.Round(myPlaytime);

        switch (PathScript.LevelSet)
        {
            case Player_States.LevelSetNumber.Levels1:
                switch (PathScript.StageSet)
                {
                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level1ClearTime[0] = ((Game.current.Progress.Level1ClearTime[0] > myPlaytime) ? myPlaytime : Game.current.Progress.Level1ClearTime[0]);
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level1ClearTime[1] = ((Game.current.Progress.Level1ClearTime[1] > myPlaytime) ? myPlaytime : Game.current.Progress.Level1ClearTime[1]);
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level1ClearTime[2] = ((Game.current.Progress.Level1ClearTime[2] > myPlaytime) ? myPlaytime : Game.current.Progress.Level1ClearTime[2]);
                        break;
                }
                break;
            case Player_States.LevelSetNumber.Levels2:
                switch (PathScript.StageSet)
                {

                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level2ClearTime[0] = ((Game.current.Progress.Level2ClearTime[0] > myPlaytime) ? myPlaytime : Game.current.Progress.Level2ClearTime[0]);
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level2ClearTime[1] = ((Game.current.Progress.Level2ClearTime[1] > myPlaytime) ? myPlaytime : Game.current.Progress.Level2ClearTime[1]);
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level2ClearTime[2] = ((Game.current.Progress.Level2ClearTime[2] > myPlaytime) ? myPlaytime : Game.current.Progress.Level2ClearTime[2]);
                        break;
                }
                break;
            case Player_States.LevelSetNumber.Levels3:
                switch (PathScript.StageSet)
                {

                    case Player_States.StageSetNumber.Stage1:
                        Game.current.Progress.Level3ClearTime[0] = ((Game.current.Progress.Level3ClearTime[0] > myPlaytime) ? myPlaytime : Game.current.Progress.Level3ClearTime[0]);
                        break;
                    case Player_States.StageSetNumber.Stage2:
                        Game.current.Progress.Level3ClearTime[1] = ((Game.current.Progress.Level3ClearTime[1] > myPlaytime) ? myPlaytime : Game.current.Progress.Level3ClearTime[1]);
                        break;
                    case Player_States.StageSetNumber.Stage3:
                        Game.current.Progress.Level3ClearTime[2] = ((Game.current.Progress.Level3ClearTime[2] > myPlaytime) ? myPlaytime : Game.current.Progress.Level3ClearTime[2]);
                        break;
                }
                break;
            case Player_States.LevelSetNumber.FinalBoss:

                Game.current.Progress.FinalBossClearTime = ((Game.current.Progress.FinalBossClearTime > myPlaytime) ? myPlaytime : Game.current.Progress.FinalBossClearTime);
                break;

        }


    }
}

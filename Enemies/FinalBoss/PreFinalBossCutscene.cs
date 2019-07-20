using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PreFinalBossCutscene : MonoBehaviour {

    public int EventsSequence = 0;
    public bool DialogueWasCompleted;
    public bool Dialogue2WasCompleted;


    public Vector3Int[] PreFightDialogueList;

    public int PreDialogueLevels;
    public bool PreDialogueStarted;
    public int PreDialogueLevelsMax = 1;

    float zoomTimer = 0;
    float zoomTimerReset = .01f;

    public GameObject BackWall;


    public Vector3Int[] PostFightDialogueList;

    public int PostDialogueLevels;
    public bool PostDialogueStarted;
    public int PostDialogueLevelsMax = 1;

    public Vector3Int[] PostFightDialogueList2;

    public int PostDialogueLevels2;
    public bool PostDialogueStarted2;
    public int PostDialogueLevelsMax2 = 1;



    public Transform GlyphExitLocation;
    public float waitTimer;
    public GameObject GlyphModel;

    public Transform PlayerPresetPosition;

    public GameObject BlackOverlayObj;

    Player_Main plyr;
    // Use this for initialization
    void Start () {
        EventsSequence = 0;
        BackWall.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!(LevelGM.Instance.playDeath))
        {
            switch (EventsSequence)
            {
                case -1:

                    BackWall.SetActive(false);
                    break;
                case 0:

                    BackWall.SetActive(false);
                    break;
                case 1:

                    Game.current.Progress.GameplayPaused = true;
                    zoomTimer -= Time.deltaTime;
                    Camera activeCamera = CameraController.Instance.GetComponent<Camera>();

                    if (LevelGM.Instance.ActiveGhostSpot.GetComponent<GhostZahraMaster>().GhostChild != null)
                    {
                        Destroy(LevelGM.Instance.ActiveGhostSpot.GetComponent<GhostZahraMaster>().GhostChild);
                    }

                    if (zoomTimer <= 0 && CameraController.Instance.CameraZPos > -40)
                    {
                        zoomTimer = zoomTimerReset;
                        CameraController.Instance.CameraZPos += (-1f);
                        if (activeCamera.fieldOfView > 15)
                        { activeCamera.fieldOfView -= 1; }
                        /*float f = .83f;
                        float f1 = activeCamera.fieldOfView;
                        Debug.Log(f1.ToString());
                        f1 = (f1 - f);
                        Debug.Log(f1.ToString());
                        activeCamera.fieldOfView = f;*/

                    }
                    else if (CameraController.Instance.CameraZPos <= -40)
                    {
                        activeCamera.fieldOfView = 15;
                        CameraController.Instance.CameraZPos = -40;
                        EventsSequence = 2;
                    }
                    break;
                case 2:

                    BackWall.SetActive(true);
                    switch (DialogueWasCompleted)
                    {
                        case false:

                            EventsSequence = 3;
                            BackWall.SetActive(true);
                            PreDialogueLevels = 1;
                            DialogueGo(PreFightDialogueList);
                            PreDialogueStarted = true;
                            break;
                        case true:

                            EventsSequence = 4;
                            break;
                    }
                    break;
                case 3:
                    if (DialogueSystem.Instance.dialogueBegin && !PreDialogueStarted && PreDialogueLevels < PreDialogueLevelsMax)
                    {
                        PreDialogueStarted = true;
                    }
                    if (PreDialogueLevels < PreDialogueLevelsMax && PreDialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PreDialogueLevels++;
                        //if (dialogueLevels == 2)
                        //{ 
                        //    //DialogueSystem.Instance.AddNewDialogue(dialogue2, charName2);
                        //}
                    }
                    else if (PreDialogueLevels >= PreDialogueLevelsMax && PreDialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PreDialogueStarted = false;
                        DialogueWasCompleted = true;
                        //Game.current.Progress.GameplayPaused = false;
                        EventsSequence = 4;
                    }
                    break;
                case 4:
                    SetUpTheFight();
                    break;

                case 5:

                    break;
                case 6:
                    BlackOverlayObj.SetActive(true);
                    BackWall.SetActive(false);
                    LevelGM.Instance.BackgroundMusicSource.pitch = 1;
                    plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    plyr.myAnim.SetInteger("AnimState", 0);
                    plyr.myAnim.SetBool("Dashing", false);
                    plyr.myAnim.SetBool("Jump", false);
                    plyr.myAnim.SetBool("Airborne", false);
                    plyr.myAnim.SetBool("Dashing", false);
                    plyr.MeleeScript.ResetMeleeState();
                    plyr.myAnim.SetBool("ClearMind", false);
                    break;
                case 7:
                    plyr.transform.position = PlayerPresetPosition.position;
                    FinalBoss_Main.Instance.gameObject.transform.position = FinalBoss_Main.Instance.TouchingFloorClone.position;

                    BlackOverlayObj.SetActive(false);

                    PostDialogueLevels = 1;
                    DialogueGo(PostFightDialogueList);
                    PostDialogueStarted = true;
                    EventsSequence = 8;
                    break;
                case 8:

                    if (DialogueSystem.Instance.dialogueBegin && !PostDialogueStarted && PostDialogueLevels < PostDialogueLevelsMax)
                    {
                        PostDialogueStarted = true;
                    }
                    if (PostDialogueLevels < PostDialogueLevelsMax && PostDialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PostDialogueLevels++;
                        //if (dialogueLevels == 2)
                        //{ 
                        //    //DialogueSystem.Instance.AddNewDialogue(dialogue2, charName2);
                        //}
                    }
                    else if (PostDialogueLevels >= PostDialogueLevelsMax && PostDialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PostDialogueStarted = false;
                        Dialogue2WasCompleted = true;
                        //Game.current.Progress.GameplayPaused = false;
                        //EventsSequence = 4;
                        //smokeScreen.Stop();
                        EventsSequence = 9;
                    }
                    break;
                case 9:
                    
                    FinalBoss_Main.Instance.gameObject.transform.DOScale(new Vector3(0, 0,0), .5f);
                    GlyphModel.transform.DOMove(new Vector3(GlyphExitLocation.position.x, GlyphModel.transform.position.y, GlyphModel.transform.position.z), 5);
                    waitTimer = 5;
                    EventsSequence = 10;

                    break;
                case 10:
                    waitTimer -= Time.deltaTime;
                    if (GlyphModel.transform.position.x == GlyphExitLocation.transform.position.x || waitTimer <= 0)
                    {

                        PostDialogueLevels2 = 1;
                        DialogueGo(PostFightDialogueList2);
                        PostDialogueStarted2 = true;
                        EventsSequence = 11;
                    }
                    break;
                case 11:
                    if (DialogueSystem.Instance.dialogueBegin && !PostDialogueStarted2 && PostDialogueLevels2 < PostDialogueLevelsMax2)
                    {
                        PostDialogueStarted2 = true;
                    }
                    if (PostDialogueLevels2 < PostDialogueLevelsMax2 && PostDialogueStarted2 && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PostDialogueLevels2++;
                        //if (dialogueLevels == 2)
                        //{ 
                        //    //DialogueSystem.Instance.AddNewDialogue(dialogue2, charName2);
                        //}
                    }
                    else if (PostDialogueLevels2 >= PostDialogueLevelsMax2 && PostDialogueStarted2 && !(DialogueSystem.Instance.dialogueBegin))
                    {
                        PostDialogueStarted2 = false;
                        //Dialogue2WasCompleted = true;
                        //Game.current.Progress.GameplayPaused = false;
                        //EventsSequence = 4;
                        //smokeScreen.Stop();
                        EventsSequence = 12;
                    }
                    break;
                case 12:
                    Game.current.Progress.FinalBoss = Player_States.LevelCompletion.Cleared;
                    Game.current.Progress.NewGamePlus = true;
                    Game.current.Progress.PreBossCutscene = true;
                    Game.current.Progress.FinalBossDeaths = LevelGM.Instance.DeathCount;
                    
                    LevelGM.Instance.UpdateTime();

                    int myRating = 0;
                    if (LevelGM.Instance.myPlaytime < 550)
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
                    Game.current.Progress.BossPart = true;
                    Game.current.Progress.FinalBossRating = myRating;

                    Game.current.Progress.GameplayPaused = false;
                    SaveAndLoad.Save(Game.current.Progress.FileNumber);
                    EventsSequence = 13;
                    break;
                case 13:
                    SceneManager.LoadScene("Win");
                    break;
            }
        }
        else
        {
            OnPlayerDeathDuringBoss();
        }
	}

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            if (EventsSequence == 0)
            {
                Game.current.Progress.GameplayPaused = true;
                plyr = CameraController.Instance.Player.GetComponent<Player_Main>();
                plyr.myRB.velocity = new Vector2(0, 0);
                plyr.myAnim.SetInteger("AnimState", 0);
                plyr.myAnim.SetBool("Dashing", false);
                plyr.myAnim.SetBool("Jump", false);
                plyr.myAnim.SetBool("Airborne", false);
                plyr.myAnim.SetBool("Dashing", false);
                plyr.MeleeScript.ResetMeleeState();
                plyr.myAnim.SetBool("ClearMind", false);
                EventsSequence = 1;
            }
        }
    }

    public void OnPlayerDeathDuringBoss()
    {
        FinalBoss_Main.Instance.ResetArena();
        EventsSequence = -1;
        CameraController.Instance.CameraZPos = -10;
        Camera activeCamera = CameraController.Instance.GetComponent<Camera>();
        activeCamera.fieldOfView = 40;
    }

    public void SetUpTheFight()
    {
        FinalBoss_Main.Instance.SetupBoss();
        EventsSequence = 5;
        Game.current.Progress.GameplayPaused = false;
    }

    public void DialogueGo(Vector3Int[] LinesToGo)
    {
        Game.current.Progress.GameplayPaused = true;
        DialogueSystem.Instance.AddNewDialogue(LinesToGo);
    }
}

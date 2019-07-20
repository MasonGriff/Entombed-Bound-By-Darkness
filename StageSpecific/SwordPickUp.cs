using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickUp : MonoBehaviour {

    PlayerController Controls;

    public enum SwordPickupNumberGeneral { Sword1, Sword2}
    public SwordPickupNumberGeneral SwordNumber;
    public Player_Main plyr;
    public ParticleSystem smokeScreen;

    public int EventSequence = 0;
    public bool SwordPickedUp = false;
    public GameObject SwordModel, EventTrigger;

    public GameObject SelectionChoices;
    public GameObject[] SelectionChoice;

    public float buttonTimer = 0;
    public float ButtonReset = 1;


    public bool DialogueWasCompleted;
    public bool Dialogue2WasCompleted;

    public Vector3Int[] PreFightDialogueList;

    public int PreDialogueLevels;
    public bool PreDialogueStarted;
    public int PreDialogueLevelsMax = 1;

    public Vector3Int[] PostFightDialogueList;

    public int PostDialogueLevels;
    public bool PostDialogueStarted;
    public int PostDialogueLevelsMax = 1;
      
    // Use this for initialization
    void Start () {
        smokeScreen.Stop();

        SelectionChoices.SetActive(false);

        if ((Game.current.Progress.Sword1Cutscene && SwordNumber == SwordPickupNumberGeneral.Sword1) || (Game.current.Progress.Sword2Cutscene && SwordNumber == SwordPickupNumberGeneral.Sword2))
        {
            Destroy(EventTrigger);
        }

        Controls = PlayerController.CreateWithDefaultBindings();
        SaveAndLoad.Save(Game.current.Progress.FileNumber);

        if (Game.current != null)
        {
            if (SwordNumber == SwordPickupNumberGeneral.Sword1 && Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.Obtained ||
                SwordNumber == SwordPickupNumberGeneral.Sword2 && Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.Obtained)
            {
                SwordPickedUp = true;
            }
        }
        if (SwordPickedUp)
        {
            SwordModel.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        buttonTimer -= Time.deltaTime;
        switch (EventSequence)
        {
            case 1:


                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                PreDialogueLevels = 1;
                smokeScreen.Play();
                DialogueGo(PreFightDialogueList);
                PreDialogueStarted = true;
                EventSequence = 2;
                break;
            case 2:

                Game.current.Progress.GameplayPaused = true;

                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

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
                    //EventsSequence = 4;
                    EventSequence = 3;
                }
                break;
            case 3:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                SelectionChoices.SetActive(true);
                EventSequence = 4;

                break;
            case 4:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if (Controls.Confirm.WasPressed)
                {
                    switch (SwordNumber)
                    {
                        case SwordPickupNumberGeneral.Sword1:
                            Game.current.Progress.Sword1 = Player_States.SwordsObtainedStatus.Obtained;
                            
                            break;
                        case SwordPickupNumberGeneral.Sword2:
                            Game.current.Progress.Sword2 = Player_States.SwordsObtainedStatus.Obtained;
                            break;
                    }
                    switch (Game.current.Progress.SwordLevel)
                    {
                        case Player_States.SwordUpgrade.NotReached:
                            Game.current.Progress.SwordLevel = Player_States.SwordUpgrade.DealsDamage;
                            break;
                        case Player_States.SwordUpgrade.DealsDamage:
                            Game.current.Progress.SwordLevel = Player_States.SwordUpgrade.DispellsIllusions;
                            break;
                    }
                    plyr.MeleeScript.SwordsCurrentlyOwned();
                    SwordPickedUp = true;
                    SwordModel.SetActive(false);
                    EventSequence = 5;
                    SelectionChoice[1].SetActive(false);
                }
                else if (Controls.Return.WasPressed)
                {

                    switch (SwordNumber)
                    {
                        case SwordPickupNumberGeneral.Sword1:
                            Game.current.Progress.Sword1 = Player_States.SwordsObtainedStatus.Ignored;
                            break;
                        case SwordPickupNumberGeneral.Sword2:
                            Game.current.Progress.Sword2 = Player_States.SwordsObtainedStatus.Ignored;
                            break;
                    }
                    switch (Game.current.Progress.ClearMindLevel)
                    {
                        case Player_States.ClearMindUpgrade.NotReached:
                            Game.current.Progress.ClearMindLevel = Player_States.ClearMindUpgrade.Level1;
                            LevelGM.Instance.ClearMindValue[0] = 100;
                            break;
                        case Player_States.ClearMindUpgrade.Level1:

                            Game.current.Progress.ClearMindLevel = Player_States.ClearMindUpgrade.Level2;
                            LevelGM.Instance.ClearMindValue[0] = 100;
                            LevelGM.Instance.ClearMindValue[1] = 100;
                            break;
                    }

                    SelectionChoice[0].SetActive(false);
                    EventSequence = 5;
                }
                break;
            case 5:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                buttonTimer = ButtonReset;
                EventSequence = 6;
                break;

            case 6:

                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if (buttonTimer <= 0)
                {
                    SelectionChoices.SetActive(false);

                    EventSequence = 7;
                }
                break;
            case 7:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                PostDialogueLevels = 1;
                DialogueGo(PostFightDialogueList);
                PostDialogueStarted = true;
                EventSequence = 8;
                break;
            case 8:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
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
                    smokeScreen.Stop();
                    EventSequence = 9;
                }
                break;
            case 9:
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                switch (SwordNumber)
                {
                    case SwordPickupNumberGeneral.Sword1:
                        Game.current.Progress.Sword1Cutscene = true;
                        Game.current.Progress.Level2[0] = (Game.current.Progress.Level2[0] != Player_States.LevelCompletion.Cleared ? Player_States.LevelCompletion.Incomplete : Player_States.LevelCompletion.Cleared);
                        break;
                    case SwordPickupNumberGeneral.Sword2:
                        Game.current.Progress.Sword2Cutscene = true;
                        Game.current.Progress.Level3[0] = (Game.current.Progress.Level3[0] != Player_States.LevelCompletion.Cleared ? Player_States.LevelCompletion.Incomplete : Player_States.LevelCompletion.Cleared);
                        break;
                }
                EventSequence = 10;
                break;
            case 10:

                Game.current.Progress.GameplayPaused = false;

                plyr.myRB.constraints = RigidbodyConstraints2D.FreezeRotation;


                SaveAndLoad.Save(Game.current.Progress.FileNumber);

                Destroy(EventTrigger);
                break;
        }

	}

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (!(Game.current.Progress.Sword1Cutscene && SwordNumber == SwordPickupNumberGeneral.Sword1) || !(Game.current.Progress.Sword2Cutscene && SwordNumber == SwordPickupNumberGeneral.Sword2))
        {
            if (myTrigg.tag == "Player")
            {
                plyr = myTrigg.gameObject.GetComponent<Player_Main>();
                //EventSequence = 1;
                plyr = CameraController.Instance.Player.GetComponent<Player_Main>();
                plyr.PlayerState = Player_States.PlayerStates.Normal;
                plyr.myRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                plyr.myRB.velocity = new Vector2(0, 0);
                plyr.myAnim.SetInteger("AnimState", 0);
                plyr.myAnim.SetBool("Dashing", false);
                plyr.myAnim.SetBool("Jump", false);
                plyr.myAnim.SetBool("Airborne", false);
                plyr.myAnim.SetBool("Dashing", false);
                plyr.MeleeScript.ResetMeleeState();
                plyr.myAnim.SetBool("ClearMind", false);
                plyr.MovementScript.moveInput = 0;
                EventSequence = 1;
            }
        }
    }

    public void DialogueGo(Vector3Int[] LinesToGo)
    {
        Game.current.Progress.GameplayPaused = true;
        DialogueSystem.Instance.AddNewDialogue(LinesToGo);
    }
}

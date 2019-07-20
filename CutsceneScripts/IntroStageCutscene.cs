using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntroStageCutscene : MonoBehaviour {

    public int EventsSequence = 0;

    public bool DialogueWasCompleted;


    public Vector3Int[] PreFightDialogueList;

    public int PreDialogueLevels;
    public bool PreDialogueStarted;
    public int PreDialogueLevelsMax = 1;

    public GameObject GlyphModel;
    public Animator GlyphAnim;

    public float waitTimer;

    public Transform GlyphExitLocation;

    public AnimationClip FlipAnim;

    public GameObject MoveTutorialPrompt;

    // Use this for initialization
    void Start () {
        MoveTutorialPrompt.SetActive(false);
        EventsSequence = 1;
        //BackWall.SetActive(true);
        PreDialogueLevels = 1;
        DialogueGo(PreFightDialogueList);
        PreDialogueStarted = true;
    }
	
	// Update is called once per frame
	void Update () {
        waitTimer -= Time.deltaTime;
        switch (EventsSequence)
        {
            case 1:
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
                    EventsSequence = 2;
                }

                break;
            case 2:
                GlyphAnim.SetInteger("Sequence", 1);
                GlyphModel.transform.DOLocalRotate(new Vector3(GlyphModel.transform.rotation.eulerAngles.x, 180, GlyphModel.transform.rotation.eulerAngles.z), FlipAnim.length);
                waitTimer = FlipAnim.length;
                EventsSequence = 3;
                break;
            case 3:
                if (waitTimer <= 0)
                {
                    GlyphModel.transform.DOMove(new Vector3(GlyphExitLocation.position.x, GlyphModel.transform.position.y, GlyphModel.transform.position.z), 3);
                    waitTimer = 3;
                    EventsSequence = 4;
                }
                break;
            case 4:
                if (waitTimer <= 0)
                {
                    GlyphModel.SetActive(false);
                    Game.current.Progress.GameplayPaused = false;
                    MoveTutorialPrompt.SetActive(true);
                    Game.current.Progress.OpeningCutscene = true;
                    SaveAndLoad.Save(Game.current.Progress.FileNumber);
                    Destroy(this.gameObject);
                }
                break;
        }
    }


    public void DialogueGo(Vector3Int[] LinesToGo)
    {
        Game.current.Progress.GameplayPaused = true;
        DialogueSystem.Instance.AddNewDialogue(LinesToGo);
    }
}


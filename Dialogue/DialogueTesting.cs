using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTesting : MonoBehaviour {

    public Vector3Int[] DialogueLists;

    //public string[] dialogue1;
    //public string charName1;

    //public string[] dialogue2;
    //public string charName2;

    public int dialogueLevels;
    public bool dialogueStarted;
    public int dialogueLevelsMax = 1;

    // Use this for initialization
    void Start () {
        dialogueLevels = 1;
        DialogueGo(DialogueLists);
        dialogueStarted = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (DialogueSystem.Instance.dialogueBegin && !dialogueStarted && dialogueLevels < dialogueLevelsMax)
        {
            dialogueStarted = true;
        }
        if (dialogueLevels < dialogueLevelsMax && dialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
        {
            dialogueLevels++;
            //if (dialogueLevels == 2)
            //{
            //    //DialogueSystem.Instance.AddNewDialogue(dialogue2, charName2);
            //}
        }
        else if(dialogueLevels >= dialogueLevelsMax && dialogueStarted && !(DialogueSystem.Instance.dialogueBegin))
        {
            dialogueStarted = false;
            Game.current.Progress.GameplayPaused = false;
        }
	}
    public void DialogueGo(Vector3Int[] LinesToGo)
    {
        Game.current.Progress.GameplayPaused = true;
        DialogueSystem.Instance.AddNewDialogue(LinesToGo);
    }
}

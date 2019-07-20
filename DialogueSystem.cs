using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class DialogueSystem : MonoBehaviour {
    /// <summary>
    /// This script controls the Dialogue Manager.
    /// When a new set of dialogue is started, it'll run through here and this script with control displaying and advancing dialogue.
    /// </summary>
    PlayerController Controls;


    public static DialogueSystem Instance { get; set; } //This sets the current instance of this script to this game object. To reference anything in this script, just do DialogueSystem.Instance


    [Tooltip("The canvas object that houses dialogue displays.")]
    public GameObject dialogueCanvas;
    [Tooltip("The panel in the canvas that displays the dialogue.")]
    public GameObject dialoguePanel;


    [Tooltip("Character name currently displayed in Dialogue box")]
    string CharName = "";
    [Tooltip("Each item referenced per line for dialogue." + "\n" + "\n" + "X for Character Name." + "\n" + "Y for line of dialogue." + "\n" + "Z for Character Face Portrait.")]
    public List<Vector3Int> dialogueLines = new List<Vector3Int>();


    [Tooltip("Dialogue has begun. Tells the script to tell the save file to pause.")]
    public bool dialogueBegin = false;
    [Tooltip("There is another line of dialogue after the current one and you are able to continue on to it.")]
    public bool ContinueAvailable = false;

    public Text dialogueText, nameText; //The text boxes for dialogue and name display lovated on the dialogue panel.
    public Image facePortraitBox;
    int dialogueIndex; //The current line in the list of added dialogue line.
    public AudioSource textAudio; 
    public AudioClip textClip;

    float buttonRecast = 1;
    public float buttonRecastReset =1;

    // Use this for initialization
    void Awake () {
        /*
        dialogueCanvas = GameObject.Find("DialogueCanvas").gameObject;
        dialoguePanel = dialogueCanvas.transform.Find("TextBox").gameObject;
        dialogueText = dialoguePanel.transform.Find("Dialogue").gameObject.GetComponent<Text>();
        nameText = dialoguePanel.transform.Find("Name").gameObject.GetComponent<Text>();
        textAudio = dialoguePanel.GetComponent<AudioSource>();
        */
        dialoguePanel.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
	}

    private void Start()
    {
        Controls = PlayerController.CreateWithDefaultBindings();
        buttonRecastReset = 0;
    }

    void Update()
    {
        buttonRecast -= Time.fixedDeltaTime;
        if (dialogueBegin == true){
            if (dialogueText.text == (DialogueDatabase.Instance.DialogueLines[(dialogueLines[dialogueIndex].y)]) || buttonRecast <=0)
            {
                ContinueAvailable = true;
            }

            if (dialogueBegin && ContinueAvailable)
            {
                if (Controls.Confirm.WasPressed || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    ContinueAvailable = false;
                    ContinueDialogue();
                }
            }
            if (!dialogueBegin)
            {
                //textAudio.Stop();
            }
        }
    }

    public void AddNewDialogue(Vector3Int[] lines) //Public function that all events that use dialogue call to tell the dialogue manager to start doing its thing. Plus in an array of dialogue strings and a character name to go with them.
    {
        dialogueIndex = -1;
        dialogueLines = new List<Vector3Int>();
        foreach(Vector3Int line in lines)
        {
            dialogueLines.Add(line);
        }
        //this.CharName = CharName;
       // Debug.Log(dialogueLines.Count - 1);
        
        CreateDialogue();
    }

    public void CreateDialogue() //Begins the dialogue added from Add New Dialogue.
    {
        //dialogueText.text = dialogueLines[dialogueIndex];
        nameText.text = CharName;
        dialoguePanel.SetActive(true);
        dialogueBegin = true;
        ContinueDialogue();
    }

    public void ContinueDialogue() //Move to next line in the dialogue list.
    {
        dialogueText.text = "";
        buttonRecast = buttonRecastReset;
        if (dialogueIndex < dialogueLines.Count - 1) //Not the last line of dialogue in the list.
        {
            dialogueIndex++;
            nameText.text = (DialogueDatabase.Instance.CharacterName[(dialogueLines[dialogueIndex].x)]);
            facePortraitBox.sprite = (DialogueDatabase.Instance.FacePortrait[(dialogueLines[dialogueIndex].z)]);
            dialogueText.text = "";
            StopAllCoroutines();
            StartCoroutine(TypeSentence(DialogueDatabase.Instance.DialogueLines[(dialogueLines[dialogueIndex].y)]));
            
            
        }
        else //The last line of dialogue in the list has been reached.
        {
            dialoguePanel.SetActive(false);
            dialogueBegin = false;
        }
    }

    IEnumerator TypeSentence(string sentence) //Types out the line of dialogue letter by letter.
    {
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            /*if (textAudio != null)
            {
                textAudio.clip = textClip;

                textAudio.Play();
            }*/
            
            yield return null;
        }
    }

}

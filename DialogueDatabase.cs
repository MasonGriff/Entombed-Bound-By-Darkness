using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDatabase : MonoBehaviour {
    /// <summary>
    /// This is a script for holding all dialogue and dialogue window related items. 
    /// Each part of the dialogue calls are given their own category and code=number. 
    /// The code number will be called in a Vector 3 function for easier categorization. Remember where you store your dialogue lines.
    /// </summary>
    public static DialogueDatabase Instance { get; set; }

    public string[] CharacterName;
    public string[] DialogueLines;
    public Sprite[] FacePortrait;

    private void Awake()
    {
        Instance = this;
    }

}

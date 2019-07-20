using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroStageGM : MonoBehaviour {
    public static IntroStageGM GMInstance;

    public bool LevelCleared = false;

    public bool Dialogue01 = false;
    public bool Dialogue02 = false;
    public bool Dialogue03 = false;
    public bool DialogueBoss = false;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (GMInstance == null)
        {
            GMInstance = this;
            Debug.Log("Gm instance set");
        }
        else
        {
            Debug.Log("destroyed");
            Destroy(this.gameObject);
            return;
        }

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (LevelCleared)
        {
            //Game.current.Alyzara.IntroStageCleared = "true";
            Destroy(this.gameObject);
        }
	}
}

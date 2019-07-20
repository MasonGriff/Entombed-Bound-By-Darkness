using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ready : MonoBehaviour {

    public Animator myAnim;
    public AudioClip readySound;
    public AudioSource mySource;

    public AnimationClip readyOutTransitionAnim;

    bool readyDone;
    public float soundLength;
    public float playbackFloat =0;

    public float readytimer;

	// Use this for initialization
	void Start () {
        Game.current.Alyzara.GameplayPaused = true;
        myAnim = this.gameObject.GetComponent<Animator>();
        mySource = this.gameObject.GetComponent<AudioSource>();
        soundLength = readySound.length;
        mySource.clip = readySound;
        mySource.Play();

        readytimer = readyOutTransitionAnim.length;
    }
	
	// Update is called once per frame
	void Update () {
        
        playbackFloat += Time.fixedDeltaTime;
        
        if(playbackFloat >= soundLength && myAnim.GetBool("Ready") == false)
        {
            //Game.current.Alyzara.GameplayPaused = false;
            //mySource.Stop();
            myAnim.SetBool("Ready", true);
            playbackFloat = 0;
        }
        else if (playbackFloat >= readytimer && myAnim.GetBool("Ready") == true)
        {
            Game.current.Alyzara.GameplayPaused = false;
            this.gameObject.SetActive(false);
        }

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PreTitleIntroVideo : MonoBehaviour {

    public string toNextScene = "_TitleScreen";

    public bool PressStartIsABool = true;

    public AudioClip videoSoundClip;
    public VideoClip videoVisualClip;
    public VideoPlayer myPlayer;
    public AudioSource mySoundSource;

    public float videoTimer;
    public float waitTimer = 2f;

    public GameObject PressStart;

	// Use this for initialization
	void Start () {

        myPlayer.Stop();
        mySoundSource.Stop();
        myPlayer.clip = videoVisualClip;
        myPlayer.time = 0;
        mySoundSource.clip = videoSoundClip;
        mySoundSource.time = 0;
        videoTimer = (float)videoVisualClip.length;
        myPlayer.Play();
        mySoundSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
        waitTimer -= Time.deltaTime;
        videoTimer -= Time.deltaTime;
        if (videoTimer <= 0)
        {
            if (PressStartIsABool)
            { PressStart.SetActive(true); }
            else
            {

                SceneManager.LoadScene(toNextScene);
            }
        }

        if (/*videoTimer <=0 ||*/ waitTimer <=0 && Input.anyKeyDown /*|| myPlayer.time == videoVisualClip.length*/)
        {
            SceneManager.LoadScene(toNextScene);
        }
	}
}

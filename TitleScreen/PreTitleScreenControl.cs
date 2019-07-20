using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreTitleScreenControl : MonoBehaviour {

    public string NextSceneNameHere = "_TitleScreen";
    public AudioSource myAudioSource;

    public int SequenceNumber = 0;

    public float waitTime = 1;
    public float waitingHappened = 0;

    public GameObject Message1, Message2;

    public string NewMessage1, NewMessage2;
    public string Message1Error;

    public GameObject MessageBackdrop;

    public string Message1ReturnToOld;

	// Use this for initialization
	void Start () {
        SequenceNumber = 0;
        Message1.SetActive(false);
        Message2.SetActive(false);
        MessageBackdrop.SetActive(false);
        waitTime = Random.Range(0.1f, 0.3f);
        waitingHappened = waitTime;
	}
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = 1;
        waitingHappened -= Time.deltaTime;

        if (waitingHappened <=0)
        {
            switch (SequenceNumber)
            {
                case 0:
                    myAudioSource.Play();
                    Message1ReturnToOld = Message1.GetComponent<Text>().text;
                    SequenceNumber = 1;
                    waitingHappened = waitTime;
                    MessageBackdrop.SetActive(true);
                    Message1.SetActive(true);
                    break;
                case 1:
                    myAudioSource.Stop();
                    SequenceNumber = 2;
                    waitingHappened = waitTime * 2;
                    Message1.GetComponent<Text>().text += ".";
                    break;
                case 2:
                    SequenceNumber = 3;
                    waitingHappened = waitTime;
                    Message1.GetComponent<Text>().text += ".";
                    OptionsSaveLoad.LoadSettings(true);
                    break;
                case 3:
                    if (OptionsSaveLoad.OptionsSaveExists)
                    {
                        SequenceNumber = 5;
                        waitingHappened = waitTime;
                        Message1.GetComponent<Text>().text += ".";
                        //OptionsSaveLoad.LoadSettings(true);
                    }
                    else
                    {

                        SequenceNumber = 4;
                        waitingHappened = waitTime * 2;
                        Message1.GetComponent<Text>().text = Message1Error;
                        myAudioSource.Play();
                        GameOptions.current = new GameOptions();
                    }
                    break;
                case 4:
                    //myAudioSource.Stop();
                    SequenceNumber = 5;
                    waitingHappened = waitTime;
                    Message1.GetComponent<Text>().text = Message1ReturnToOld + "..";
                    OptionsSaveLoad.SaveSettings();
                    break;
                case 5:
                    //myAudioSource.Stop();
                    SequenceNumber = 6;
                    waitingHappened = waitTime * 2;
                    Message1.GetComponent<Text>().text = Message1ReturnToOld + "...";
                    break;
                case 6:
                    SequenceNumber = 7;
                    waitingHappened = waitTime;
                    Message1.SetActive(false);
                    MessageBackdrop.SetActive(false);
                    break;
                case 7:
                    myAudioSource.Play();
                    SequenceNumber = 8;
                    waitingHappened = waitTime;
                    MessageBackdrop.SetActive(true);
                    Message2.SetActive(true);
                    break;

                case 8:
                    //myAudioSource.Stop();
                    SequenceNumber = 9;
                    waitingHappened = waitTime;
                    Message2.SetActive(false);
                    MessageBackdrop.SetActive(false);
                    break;
                case 9:
                    SceneManager.LoadScene(NextSceneNameHere);
                    break;
            }
        }


	}
}

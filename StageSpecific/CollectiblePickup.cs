using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiblePickup : MonoBehaviour {

    public enum whichLvl { lvl1s1, lvl1s2, lvl1s3,lvl2s1, lvl2s2, lvl2s3, lvl3s1, lvl3s2, lvl3s3 }

    public whichLvl CurrentStage;
    public bool isCollected;
    bool beginDestructionSequence = false;

    public GameObject sfxOBJ;
    public AudioSource sfxSource;

    public GameObject PopupMessage;
    public Text PopUpText;
     Collider2D myColl;
    public GameObject mySpinner;

	// Use this for initialization
	void Start () {
        beginDestructionSequence = false;
        myColl = GetComponent<Collider2D>();
        //mySpinner = GetComponent<ParticleSystem>();
        PopupMessage.SetActive(false);
        CheckForParts();
        if (isCollected)
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (isCollected)
        {
            if (!beginDestructionSequence)
            {
                Invoke("PreppedToDestroy", 2);
                beginDestructionSequence = true;
            }
        }
    }


    void CheckForParts()
    {
        switch (CurrentStage)
        {
            case whichLvl.lvl1s1:
                if (Game.current.Progress.EnemyParts[0])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl1s2:
                if (Game.current.Progress.EnemyParts[1])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl1s3:
                if (Game.current.Progress.EnemyParts[2])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl2s1:
                if (Game.current.Progress.ShadowParts[0])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl2s2:
                if (Game.current.Progress.ShadowParts[1])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl2s3:
                if (Game.current.Progress.ShadowParts[2])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl3s1:
                if (Game.current.Progress.EnlightenedParts[0])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl3s2:
                if (Game.current.Progress.EnlightenedParts[1])
                {
                    isCollected = true;
                }
                break;
            case whichLvl.lvl3s3:
                if (Game.current.Progress.EnlightenedParts[2])
                {
                    isCollected = true;
                }
                break;
        }
    }

    void PlayerHasCollected()
    {
        int AmountCheck = 0;
        switch (CurrentStage)
        {

            case whichLvl.lvl1s1:
                Game.current.Progress.EnemyParts[0] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enemy" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl1s2:
                Game.current.Progress.EnemyParts[1] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enemy" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl1s3:
                Game.current.Progress.EnemyParts[2] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enemy" + " Costume Parts "   + AmountCheck + " of 3.");
                break;

            case whichLvl.lvl2s1:
                Game.current.Progress.ShadowParts[0] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Shadow" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl2s2:
                Game.current.Progress.ShadowParts[1] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Shadow" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl2s3:
                Game.current.Progress.ShadowParts[2] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Shadow" + " Costume Parts "   + AmountCheck + " of 3.");
                break;

            case whichLvl.lvl3s1:
                Game.current.Progress.EnlightenedParts[0] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enlightened" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl3s2:
                Game.current.Progress.EnlightenedParts[1] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enlightened" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
            case whichLvl.lvl3s3:
                Game.current.Progress.EnlightenedParts[2] = true;
                foreach (bool gos in Game.current.Progress.EnemyParts)
                {
                    if (gos == true)
                    {
                        AmountCheck++;
                    }
                }
                PopUpText.text = ("Enlightened" + " Costume Parts "   + AmountCheck + " of 3.");
                break;
        }

        sfxSource.Play();
       // sfxOBJ.transform.parent = null;

       // mySpinner.Stop();
        PopupMessage.SetActive(true);

        isCollected = true;

    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if(myTrigg.tag == "Player")
        {
            PlayerHasCollected();
            myColl.enabled = false;
            mySpinner.SetActive(false);
            //mySpinner.Stop();
        }
    }

    void PreppedToDestroy()
    {
        Destroy(this.gameObject);
    }
}

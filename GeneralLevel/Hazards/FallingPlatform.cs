using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FallingPlatform : MonoBehaviour {

    FallingPlatformMaster ParentScript;

    public float OffscreenCheckDistance = 30;

    public float FallGravity= 3;
    public float FallWarningDuration = 3f;
    public float WarningShakePower = 2;
    float fallWarningCheck = 0;

    public bool fallWarning;
    public bool FallTriggered;
    public bool FallStart;

    public AudioSource creakSFX;

    [HideInInspector]
    public Collider2D myColl;
    [HideInInspector]
    public Rigidbody2D myRB;
    public Sequence myDoMoveSequence;
    public bool creaks;
    [SerializeField]
    public GameObject PlayerObj;
    

    // Use this for initialization
    void Start () {
        myRB = GetComponent<Rigidbody2D>();
        myColl = GetComponent<Collider2D>();
        ParentScript = transform.parent.GetComponent<FallingPlatformMaster>();
        ResetTheStuff();
	}
	
	// Update is called once per frame
	void Update () {
        fallWarningCheck -= Time.deltaTime;
	}

    private void FixedUpdate()
    {
        FindThePlayer();
        if (!fallWarning)
        {
            creakSFX.Stop();
            creaks = false;
            myRB.gravityScale = 0;
            myRB.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
        if (fallWarning && !FallTriggered)
        {

            myRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            FallTriggered = true;
            //transform.DOShakePosition(FallWarningDuration, new Vector3(0, WarningShakePower, 0));
            myDoMoveSequence = DOTween.Sequence();
            myDoMoveSequence.Append(transform.DOShakePosition(FallWarningDuration, new Vector3(0, WarningShakePower, 0)));
            myDoMoveSequence.Play();

            if (!creaks)
            {
                creaks = true;
                creakSFX.Play();
            }

            fallWarningCheck = FallWarningDuration;
        }
        if (FallTriggered && fallWarningCheck <= 0 && !FallStart)
        {
            //myDoMoveSequence.Kill();
            FallStart = true;
            ParentScript.FallHasBegun = true;
            myRB.gravityScale = FallGravity;
            myColl.enabled = false;
        }

        if (FallStart && PlayerObj == null)
        {
            myRB.gravityScale = 0;
            myDoMoveSequence.Kill();
            Debug.Log("Offscreen now.");
            gameObject.SetActive(false);

        }

    }

    public GameObject FindThePlayer()
    {
        GameObject[] Players;
        Players = GameObject.FindGameObjectsWithTag("Player");
        PlayerObj = null;
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach(GameObject player in Players)
        {
            Vector3 diff = player.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if ((player.transform.position.x <= (transform.position.x + OffscreenCheckDistance)) && (player.transform.position.x >= (transform.position.x - OffscreenCheckDistance))) //Checks if the nearest perch point is in range.
            {
                if ((player.transform.position.y <= (transform.position.y + OffscreenCheckDistance)) && (player.transform.position.y >= (transform.position.y - OffscreenCheckDistance)))
                {
                    PlayerObj = player;
                    distance = curDist;
                }
            }
        }

        

        return PlayerObj;
    }

    public void ResetTheStuff()
    {
        myDoMoveSequence.Kill();
        ParentScript.FallHasBegun = false;
        fallWarningCheck = 0;
        fallWarning = false;
        FallTriggered = false;
        FallStart = false;
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            fallWarning = true;
        }
    }
}

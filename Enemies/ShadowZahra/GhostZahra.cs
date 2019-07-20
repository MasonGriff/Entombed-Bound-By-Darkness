using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostZahra : MonoBehaviour {
    public int DamageOutput;

    Collider2D myCollider;

    GhostZahraMaster myMaster;
    EnemyHealth myHealth;
    public static float InvertTimer = 5f;

    Transform newTarget = null;
    public bool TargetEngage;
    public bool ChasingTarget;
    public Animator myAnim;
    public GameObject myModel;

    public float moveTimeToTarget = 2;

    public AnimationClip myLockOnClip;

    public int FreeFloatSequence = 0;

    public float FloatTimer;
    public float FloatTimerReset = 2;
    public float floatUpDownInterval = .01f;

    float disappearTimer = 0;
    float disappearReset = .5f;

    public bool ChaseHasBegun;

    public float RotationFacingLeft = 0;
    public float RotationFacingRight = 180;

    public Vector3 TargetLocationReference;
    public bool PlayerIsHit;
    bool DeadGhost;
	// Use this for initialization
	void Start () {
        myCollider = GetComponent<Collider2D>();
        DeadGhost = false;
        GameObject[] WallsObj = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject gos in WallsObj)
        {
            Collider2D wallColl = gos.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myCollider, wallColl);
        }
        GameObject[] GroundsObj = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject gos in GroundsObj)
        {
            Collider2D groundColl = gos.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myCollider, groundColl);
        }

        myMaster = transform.parent.GetComponent<GhostZahraMaster>();
        if (myMaster.OverrideGhostZahraSpeed)
        {
            moveTimeToTarget = myMaster.GhostZahraSpeed;
        }
        myHealth = GetComponent<EnemyHealth>();
        FloatTimer = 0;
        PlayerIsHit = false;
        ChaseHasBegun = false;
        ChasingTarget = false;
        TargetEngage = false;
        FreeFloatSequence = 0;
	}
	
	// Update is called once per frame
	void Update () {
        FloatTimer -= Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (!TargetEngage)
        {
            Vector3 newPos;
            switch (FreeFloatSequence)
            {
                case 0:
                    FloatTimer = FloatTimerReset;
                    FreeFloatSequence = 1;
                    break;
                case 1:
                    switch(FloatTimer <= 0)
                    {
                        case false:
                            newPos = transform.localPosition;
                            newPos.y += floatUpDownInterval;
                            transform.localPosition = newPos;
                            break;
                        case true:
                            FreeFloatSequence = 2;
                            FloatTimer = FloatTimerReset*2;
                            break;
                    }
                    break;
                case 2:
                    switch (FloatTimer <= 0)
                    {
                        case false:
                            newPos = transform.localPosition;
                            newPos.y -= floatUpDownInterval;
                            transform.localPosition = newPos;
                            break;
                        case true:
                            FreeFloatSequence = 3;
                            FloatTimer = FloatTimerReset;
                            break;
                    }
                    break;
                case 3:
                    switch (FloatTimer <= 0)
                    {
                        case false:
                            newPos = transform.localPosition;
                            newPos.y += floatUpDownInterval;
                            transform.localPosition = newPos;
                            break;
                        case true:
                            FreeFloatSequence = 1;
                            FloatTimer = FloatTimerReset;
                            break;
                    }
                    break;
            }
        }
	}


    private void FixedUpdate()
    { 
        if (myHealth.Damaged)
        {
            myHealth.damageEnabled = false;
            myHealth.Health -= myHealth.damageTaken;
        }
        if (myHealth.Health <= 0)
        {
            GhostVanishes();
        }
        if (ChasingTarget && !ChaseHasBegun)
        {
            ChaseHasBegun = true;
            FloatTimer = moveTimeToTarget;
            myAnim.SetTrigger("Dash");
            transform.DOMove(TargetLocationReference, moveTimeToTarget);
        }
        else if (ChaseHasBegun && transform.position != TargetLocationReference && FloatTimer >= 0 && Game.current.Progress.CurrentDifficulty != Player_States.DifficultySetting.Easy)
        {
            TargetLocationReference = newTarget.position;
            Vector3 TargetsPosition = TargetLocationReference;
            if (TargetsPosition.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, RotationFacingLeft, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -RotationFacingRight, 0);
            }
            if (Game.current.Progress.CurrentDifficulty != Player_States.DifficultySetting.Hard)
            { transform.DOMove(TargetLocationReference, (moveTimeToTarget)); }
            else
            {
                float newDodge = moveTimeToTarget;
                newDodge = newDodge * 0.65f;
                transform.DOMove(TargetLocationReference, newDodge);
            }

        }
        else if (ChaseHasBegun && transform.position == TargetLocationReference || FloatTimer <=0 && ChaseHasBegun)
        {
            if (!DeadGhost)
            {
                disappearTimer = 0;
                disappearTimer = disappearReset;
                DeadGhost = true;
                transform.DOScale(new Vector3(0, 0, 0), disappearReset);
            }
            if (DeadGhost)
            {
                if (disappearTimer <= 0)
                {
                    disappearTimer = 0;
                    GhostVanishes();
                }
            }
        }
    }

    public void TargetIsSighted(Vector3 TargetsPosition, Transform MyTarget)
    {
        TargetEngage = true;
        TargetLocationReference = TargetsPosition;
        newTarget = MyTarget;
        myAnim.SetTrigger("LockOn");
        if (TargetsPosition.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, RotationFacingLeft, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -RotationFacingRight, 0);
        }
        transform.DOLocalMove(new Vector3(0, 0, 0), myLockOnClip.length);
        Invoke("LockOnToTarget", myLockOnClip.length);
    }
    void LockOnToTarget()
    {
        ChasingTarget = true;
    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            Debug.Log("PlayerTouch");
            if (ChasingTarget)
            {
                PlayerIsHit = true;
                Player_Main plyr = myTrigg.GetComponent<Player_Main>();
                plyr.DamageTaken(DamageOutput);Debug.Log("PlayerTakesDamage");
                plyr.MovementScript.invertTimer = InvertTimer;
                plyr.PlayerDebuffState = Player_States.PlayerDebuffState.InvertControls;
                GhostVanishes();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D myTrigg)
    {
        if (myTrigg.gameObject.tag == "Player")
        {
            Debug.Log("PlayerTouch");
            if (ChasingTarget)
            {
                PlayerIsHit = true;
                Player_Main plyr = myTrigg.gameObject.GetComponent<Player_Main>();
                plyr.DamageTaken(DamageOutput); Debug.Log("PlayerTakesDamage");
                plyr.MovementScript.invertTimer = InvertTimer;
                plyr.PlayerDebuffState = Player_States.PlayerDebuffState.InvertControls;
                GhostVanishes();
            }
        }
    }

    public void GhostVanishes()
    {
        myMaster.RespawnTimer = myMaster.RespawnTimerReset;
        Destroy(gameObject);
    }
}

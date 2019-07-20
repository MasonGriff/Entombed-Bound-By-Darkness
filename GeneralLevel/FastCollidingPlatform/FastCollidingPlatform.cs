using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastCollidingPlatform : MonoBehaviour {

    public bool HitWall = false;
    public bool PlatformDying = false;

    public Player_States.PlatformDirection MyDirection;

    SwitchCheck CollisionSwitch;
    Collider2D myColl;

    Rigidbody2D myrb;
    public float MoveSpeed;
    public float GravityDropSpeed = 3;

    float LastXPos;

    [HideInInspector]
    public Collider2D WallIgnore, FloorIgnore;

    public GameObject[] ChildObjects;

    public float DeathTime;
    float DeadTimer;
    bool TimerStart;
	// Use this for initialization
	void Start () {
        myrb = GetComponent<Rigidbody2D>();
        myColl = GetComponent<Collider2D>();
        CollisionSwitch = GetComponent<SwitchCheck>();
        PlatformDying = false;
        foreach(GameObject childs in ChildObjects)
        {
            if (childs.GetComponent<Collider2D>() != null)
            {
                Physics2D.IgnoreCollision(myColl, childs.GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(WallIgnore, childs.GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(FloorIgnore, childs.GetComponent<Collider2D>());
                    
}
        }
        HitWall = false;
        TimerStart = false;
	}
	
	// Update is called once per frame
	void Update () {

        DeadTimer -= Time.deltaTime;
        if (HitWall)
        {
            PlatformDying = true;
        }
        if(PlatformDying && !TimerStart)
        {
            PlatformDeath();
        }
        else if (PlatformDying && TimerStart)
        {
            DeadTimer -= Time.deltaTime;
            if (DeadTimer <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            PlatformMovement();
            
            LastXPos = transform.position.x;
        }
	}

    void PlatformMovement()
    {
        switch (MyDirection)
        {
            case Player_States.PlatformDirection.left:
                myrb.velocity = new Vector2(-MoveSpeed, 0);
                break;
            case Player_States.PlatformDirection.right:
                myrb.velocity = new Vector2(MoveSpeed, 0);
                break;
            default:
                MyDirection = Player_States.PlatformDirection.left;
                break;
        }
    }

    void PlatformDeath()
    {
        myrb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        myrb.gravityScale = GravityDropSpeed;
        DeadTimer = DeathTime;
        TimerStart = true;
    }

    private void OnTriggerStay2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Wall")
        {
            if (CollisionSwitch.SwitchOn)
            {
                Player_Main PlayCon = LevelGM.Instance.PlayerInLevel.GetComponent<Player_Main>();
                PlayCon.HealthScript.HP = 0;
                PlayCon.PlayerHealthState = Player_States.PlayerHealthStates.Dead;
                //PlatformDeath();
                Physics2D.IgnoreCollision(myColl, PlayCon.gameObject.GetComponent<Collider2D>());
                foreach (GameObject otherObj in ChildObjects)
                {
                    Collider2D otherColl = otherObj.GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(otherColl, PlayCon.gameObject.GetComponent<Collider2D>());
                }
            }
        }
        if (myTrigg.tag == "Pitfall")
        {
            Destroy(this.gameObject);
        }
    }


}

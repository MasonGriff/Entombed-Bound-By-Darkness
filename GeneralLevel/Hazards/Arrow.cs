using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    //public Player_States.PlatformDirection ShotDirection;
    public float ArrowSpeed = 2.5f;
    public bool BeginMySequence = false;

    public int Damage = 1;
    public GameObject OriginPoint;

    public float lifeMaxTime = 5f;
    float lifeTimer = 0;

    //public float OffscreenCheckDistance = 30;
    //public float offscreenCheckHeight = 10;
    public GameObject PlayerObj;
    //public Vector2 MyVelocity;
    
    Rigidbody2D myRB;
    [HideInInspector]
    public Collider2D myColl;

    bool beginDeath=false;
    // Use this for initialization
    void Start ()
    {
        myRB = GetComponent<Rigidbody2D>();
        myColl = GetComponent<Collider2D>();
        GameObject[] ignoresColl = GameObject.FindGameObjectsWithTag("Wall");
        foreach(GameObject gos in ignoresColl)
        {
            Collider2D ignores = gos.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myColl, ignores);
        }
        GameObject[] ignoresGround = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject gos in ignoresGround)
        {
            Collider2D ignores = gos.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myColl, ignores);
        }
        //DeterminArrowVelocity(ShotDirection);
        lifeTimer = lifeMaxTime;
	}
	
	// Update is called once per frame
	void Update () {
        lifeTimer -= Time.deltaTime;
        /*if (BeginMySequence && beginDeath == false)
        {
            myRB.velocity = MyVelocity;

        }
        else
        {
            DeterminArrowVelocity(ShotDirection);
        }
        */
        myRB.AddForce(transform.up * ArrowSpeed);
        /*switch (ShotDirection)
        {
            case Player_States.PlatformDirection.up:

                break;
            case Player_States.PlatformDirection.down:

                myRB.AddForce(transform.up * -ArrowSpeed);
                break;
            case Player_States.PlatformDirection.right:

                myRB.AddForce(transform.right * ArrowSpeed);
                break;
            case Player_States.PlatformDirection.left:

                myRB.AddForce(transform.right * -ArrowSpeed);
                break;
        }*/

        if(lifeTimer <= 0)
        {
            BulletDeath();
        }
	}

    private void FixedUpdate()
    {
        
        /*if (BeginMySequence && !beginDeath)
        {
            myRB.velocity = MyVelocity;
            FindThePlayer();
            if (PlayerObj == null)
            {
                BulletDeath();
            }
        }*/
    }

    /*public GameObject FindThePlayer()
    {
        GameObject[] Players;
        Players = GameObject.FindGameObjectsWithTag("Player");
        PlayerObj = null;
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject player in Players)
        {
            Vector3 diff = player.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if ((player.transform.position.x <= (transform.position.x + OffscreenCheckDistance)) && (player.transform.position.x >= (transform.position.x - OffscreenCheckDistance))) //Checks if the nearest perch point is in range.
            {
                if ((player.transform.position.y <= (transform.position.y + offscreenCheckHeight)) && (player.transform.position.y >= (transform.position.y - offscreenCheckHeight)))
                {
                    PlayerObj = player;
                    distance = curDist;
                }
            }
        }
        return PlayerObj;
    }*/

    void BulletDeath()
    {
        beginDeath = true;
        Destroy(this.gameObject);

    }

    
    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            Player_Main PlayerScpt = myTrigg.GetComponent<Player_Main>();
            PlayerScpt.DamageTaken(Damage);
            BulletDeath();
        }
    }

}

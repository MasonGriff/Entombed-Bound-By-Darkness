using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CaveBat : MonoBehaviour {

    EnemyHealth HealthScript;
    //public int MaxHP;
    //public int HP;
    public GameObject BatParent;
    public CaveBatSpawning ParentScript;
    public Transform LeftPos;
    public Transform RightPos;
    public Transform DropPos;
    public bool facingRight = false;
    public Animator myAnim;
    public Collider2D myCollider;

    public GameObject player;
    //public PlayerController playerControl;

    public bool dying = false;
    public bool dyingBegun = false;

    public float MaxTimeFlying;
    public float MaxTimeRest;
    public float FlightTimePassed;
    public float DieAnimTime;
    public float dieTimeGo;

    public bool BatActive = false;

    public bool BatAtRest = false;
    public bool batLeavingRest = false;
    public bool BatChangeAltitude = false;

    public AnimationClip deathClip;

    public bool AttackedPlayer = false;

    private void Awake()
    {
        HealthScript = GetComponent<EnemyHealth>();
        myAnim = this.gameObject.GetComponent<Animator>();
        myCollider = this.gameObject.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {

        HealthScript.Health = HealthScript.FullHealth;
        facingRight = false;
        myCollider.enabled = true;

        Vector3 tempScale = transform.localScale;
        tempScale.x = 1f;
        transform.localScale = tempScale;

        myAnim.SetInteger("AnimState", 0);
        myAnim.SetBool("Die", false);
        dying = false;
        dyingBegun = false;
        BatChangeAltitude = false;
        BatAtRest = false;
        FlightTimePassed = 0;
    }

    // Use this for initialization
    void Start () {
        BatParent = transform.parent.gameObject;
        ParentScript = BatParent.GetComponent<CaveBatSpawning>();
        LeftPos = ParentScript.LeftPos;
        RightPos = ParentScript.RightPos;
        DropPos = ParentScript.DropPos;
        myAnim = this.gameObject.GetComponent<Animator>();
        myCollider = this.gameObject.GetComponent<Collider2D>();
        HealthScript.Health = HealthScript.FullHealth;
        DieAnimTime = deathClip.length;
        BatAtRest = false;
        FlightTimePassed = 0;
        dyingBegun = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!(LevelGM.Instance.isPaused))

        {
            if (!(HealthScript.Damaged))
            { FlightTimePassed += Time.deltaTime; }
            //float distanceFloat = Vector3.Distance(LeftPos.position.x, RightPos.position.x)
            if (player != null)
            {
                //if (playerControl.TakingDamage == false && AttackedPlayer)
                //{
                //    AttackedPlayer = false;
                //    Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), myCollider, false);
                //}
            }


            if (HealthScript.Health <= 0 && !dying|| HealthScript.dead) 
            {
                dying = true;
                dieTimeGo = DieAnimTime;
                //Debug.Log("dying switched to true");
                DyingBegunFunction();
                //Debug.Log(dying);
            }
            if(HealthScript.Damaged && HealthScript.MeleeImmunity)
            {
                DamageTaken();
            }

            else if (!dying && !(HealthScript.Damaged))
            {
                if (BatAtRest)
                {
                    if (FlightTimePassed >= (MaxTimeRest / 2) && !batLeavingRest && FlightTimePassed < MaxTimeFlying)
                    {
                        Flip();
                        batLeavingRest = true;
                        //Debug.Log("flipped");
                    }
                    else if (FlightTimePassed >= MaxTimeRest)
                    {
                        BatAtRest = false;
                        batLeavingRest = false;
                        FlightTimePassed = 0;
                        myAnim.SetInteger("AnimState", 1);
                    }
                }
                else if (!BatAtRest)
                {
                    myAnim.SetInteger("AnimState", 1);
                    if (!facingRight && FlightTimePassed < MaxTimeFlying && !BatActive)
                    {

                        transform.DOMoveX(LeftPos.transform.position.x, MaxTimeFlying);
                        BatActive = true;

                    }
                    else if (!facingRight && FlightTimePassed >= MaxTimeFlying)
                    {
                        transform.position = LeftPos.position;
                        BatActive = false;
                    }
                    else if (facingRight && FlightTimePassed < MaxTimeFlying && !BatActive)
                    {
                        transform.DOMoveX(RightPos.transform.position.x, MaxTimeFlying);
                        BatActive = true;
                    }
                    else if (facingRight && FlightTimePassed >= MaxTimeFlying)
                    {
                        transform.position = RightPos.position;
                        BatActive = false;
                    }


                    if (FlightTimePassed <= (MaxTimeFlying / 4) && !BatChangeAltitude)
                    {
                        transform.DOMoveY(DropPos.position.y, (MaxTimeFlying /4));
                        BatChangeAltitude = true;
                    }
                    else if (FlightTimePassed > (MaxTimeFlying / 2) && FlightTimePassed <= MaxTimeFlying)
                    {
                        transform.DOMoveY(LeftPos.position.y, (MaxTimeFlying / 2));
                        BatChangeAltitude = false;
                    }


                    if (FlightTimePassed >= MaxTimeFlying)
                    {
                        FlightTimePassed = 0;
                        BatAtRest = true;
                        myAnim.SetInteger("AnimState", 0);
                    }
                }
            }

            

        }

        if (dying && !dyingBegun)
        {
            dieTimeGo -= Time.deltaTime;
            if (dieTimeGo <= 0)
            {
                dyingBegun = true;
                Debug.Log(dyingBegun);
            }
        }
        if (dyingBegun)
        {
            ParentScript.BatAlive = false;
            //Debug.Log("bat alive is false");
            this.gameObject.SetActive(false);
        }
        
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1f;
        transform.localScale = tempScale;
    }


    void DyingBegunFunction()
    {
        Debug.Log("dying began");
        FlightTimePassed = 0;
        myAnim.SetBool("Die", true);
        myCollider.enabled = false;
        Debug.Log("dying disabled collider");
    }

    void DamageTaken()
    {
        HealthScript.MeleeImmunity = false;
        HealthScript.Health -= HealthScript.damageTaken;
        Debug.Log("Bat is hit");
    }

    private void OnCollisionEnter2D(Collision2D myColl)
    {
        /*if(myColl.gameObject.tag == "PlayerProjectile")
        {
            GameObject collideison = myColl.gameObject;
            BusterShot buster = collideison.GetComponent<BusterShot>();
            Physics2D.IgnoreCollision(buster.bulletColl, myCollider);
            buster.bulletHit = true;
            HealthScript.Health--;
            Debug.Log("Bat Enemy Hit");
        }*/

        if(myColl.gameObject.tag == "Player")
        {
            GameObject Zahra = myColl.gameObject;
            Player_Main Plyr = Zahra.GetComponent<Player_Main>();
            Plyr.DamageTaken(1);
            Debug.Log("Player Hit");
        } 
        
       

    }
}

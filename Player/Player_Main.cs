using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player_Main : MonoBehaviour {

    /// <summary>
    /// This is the main script for the player controller. When any other script is referencing the player 
    /// controller it'll reference from here, and if another script needs to access the subscripts of the 
    /// player controller, it'll reference from here, for example: 
    ///                                                           Player_Main Player = /*Player's game object being referenced*/GetComponent<Player_Main>();
    ///                                                           Player.MovementScript./*Variable You're accessing*/ = /*Whatever you're doing with it*/
    /// 
    /// </summary>
    public enum DimensionsOfModel { ThreeD, TwoD}
    
    public DimensionsOfModel MyDimensions = DimensionsOfModel.TwoD;

    [Header("Sub Scripts")]//These are the scripts that control inputs and management of the player character. They're separated into other scripts for the sake of organization.
    public Player_Main PlayerScript;
    public Player_Movement MovementScript;
    public Player_Health HealthScript;
    //public Player_DomahdWeaponManager DomahdScript;
    public Player_Melee MeleeScript;
    public Player_PauseMenu PauseScript;
    public Player_WallTouchCollisionCheck WallCheckSCript; //Script detecting if player is touching wall. It isn't on the main player's gameobject but on one of its child objects.
    
    [Header("Player States")]
    public Player_States.PlayerStates PlayerState = Player_States.PlayerStates.Normal; //The state that the player is currently in.
    public Player_States.PlayerHealthStates PlayerHealthState = Player_States.PlayerHealthStates.Normal; //State of player's health. If they're normal, low, or dead.
    public Player_States.PlayerDebuffState PlayerDebuffState = Player_States.PlayerDebuffState.Normal;
    public bool Grounded = true; //player is touching ground.
    public bool WallTouch = false; //Player is touching wall.
    public bool DamageEnabled = true; //Player isn't immune to damage;

    [Header("Object Reference")]
    public GameObject PlayerObj; //the gameobject this script and the subscripts are attached to.
    public GameObject myModel, AttackBoxObj; //MyModel is the physical representation of the player [sprite/3d model]. Attack Box Object is the trigger collider that is used for melee attacks.
    public GameObject ModelFlippingParent; //This is a parent to the attack box and model that is mainly used for flipping the player when clinging to a wall.
    //public GameObject[] DomahdObj; //Collection of all the Domahd gameobjects to be referenced. May only be [1] if we're not adding more Domahd models
    public GameObject /*HudObj,*/ PauseMenuObj; //Canvas objects that house the UI elements. Should be self-explainatory.
    public GameObject NearestEnemy; //Nearest enemy to the player.
    public GameObject ClearMindOverlay;
    public GameObject DashEffect;
    public Transform SmokeTrailPosition;
    public Transform HeadBonePosition;
    //public ParticleSystem SmokeTrail;
    //public GameObject NearestPerch; //Nearest perch point to the player that's within range.
    //public GameObject CurrentPerch;
    //public GameObject NextPerch; //The perch point the player will pounce to next.
    //public GameObject FindNearestEnemy;

    [Header("Animation")]
    public Animator myAnim; //Main animator for the player model. 
    public Animator myHitBox, myAttackBox; //Hit box is placed on the top parent of the player's prefab, the same object this script is on. It is the collider representing the player's physical interaction with the world. Attack Box animates the attack box collider for melee attacks.
   // public Animator DomahdsFacingAnim; //This animator changes the direction the Domahd/s are facing. Mainly used for when clinging to the wall.

    [Header("Components")]
    public Collider2D myColl; //Main collider. Same one affected by myHitBox animator controller.
    public Rigidbody2D myRB; //Rigidbody. Self explainatory.
    public AudioSource myVoiceBox;
    public AudioSource JumpFall;
    public AudioSource WallClingSFX;
    // public AudioSource DeadAudioSource;
    public AudioSource damageSound;
    public AudioSource ReviveSFX;
    public AudioClip JumpClip, LandClip, doubleClip, dashSfxClip;

    [Header("Check References")]
    public Transform GroundCheck; //Object that raycasts out to detect if player is touching the ground.
    public Transform WallCheck; //Child object with collider for detecting if player is touching wall.
    public bool pauseCheck; //Is the game paused.
    public bool pauseMenuCheck = false;
    public bool FacingRight = true; //Is the player facing right.
    public float groundRadius = 0.2f; //Radius of the raycast casted by Ground Check.
    public LayerMask whatisground; //Layers that are checked for overlapping by Ground Check.
    public bool touchingWall = false; //Is the player touching a wall.
    public bool ClingToWall = false; //Is the player currently clinging to a wall.
    public float wallRadius = 0.3f; //Outdated variable not commented out yet.
    public float DamageTime; //Counting down time from taking a hit.
    public float DamageInvuln = 0.5f; //Time that DamageTime gets set to when the player takes a hit. Outdated as time is now taken from animation clip below.
    public float IFramesTimer;
    public AnimationClip DamageAnimation; //Animation clip of player getting hurt. This clip's length is taken as a float for damage time.
    public float damageKnockbackForce = 10; //How far player is knocked back after taking damage.
    public bool SlideHappening = false; //Bool for player state remembering that it's sliding. Outdated as there is now a proper sliding Player State
    bool deathWaiting = false; //Player is dead and this bool tells the controller that they're waiting on animation to finish.
    public AnimationClip DeathAnimation; //Animation clip of player dying. The clip.length is used to tell when death animation is over.
    bool deathSetupHasBEgun = false;
    public float[] ClearMindMeter = new float[2];
    public bool ClearMindOverused;

    [Header("Outfits")]
    public GameObject[] OutfitModels = new GameObject[5];
    public Animator[] OutfitAnim = new Animator[5];
    public GameObject BackSword1, LeftSword, RightSword;
    public GameObject BackspotMain, LeftSpotMain, RightSpotMain;
    public GameObject[] Backspot, LeftSpot, RightSpot;
    public Transform ParentToModel;
    public Transform[] HeadBones;

    [Header("Voice Clips")]
    public AudioClip DamageGrunt;
    public AudioClip DeathGrunt;
   // public audio

    private void Awake()
    {
        deathWaiting = false; //Player isn't dying on awake.
        
    }

    // Use this for initialization
    void Start () {
        PlayerObj = this.gameObject; //sets the player object
        PlayerState = Player_States.PlayerStates.Normal; //sets state to default
        PlayerHealthState = Player_States.PlayerHealthStates.Normal; //sets health state to default.
        // DeadAudioSource.Stop();
        //AudioListener.pause = true;
       // JumpFall.Stop();
       // myVoiceBox.Stop();
        foreach(GameObject gos in OutfitModels)
        {
            gos.SetActive(true);
        }

        switch (Game.current.Progress.CurrentOutfit)
        {
            case Player_States.OutfitSettings.Default:
                myModel = OutfitModels[0];
                myAnim = OutfitAnim[0];
                BackspotMain = Backspot[0];
                LeftSpotMain = LeftSpot[0];
                RightSpotMain = RightSpot[0];
                HeadBonePosition = HeadBones[0];

                Destroy(OutfitModels[1]);
                Destroy(OutfitModels[2]);
                Destroy(OutfitModels[3]);
                Destroy(OutfitModels[4]);
                Destroy(OutfitModels[5]);
                break;
            case Player_States.OutfitSettings.Enemy:
                myModel = OutfitModels[1];
                myAnim = OutfitAnim[1];
                
                BackspotMain = Backspot[1];
                LeftSpotMain = LeftSpot[1];
                RightSpotMain = RightSpot[1];
                HeadBonePosition = HeadBones[1];

                Destroy(OutfitModels[0]);
                Destroy(OutfitModels[2]);
                Destroy(OutfitModels[3]);
                Destroy(OutfitModels[4]);
                Destroy(OutfitModels[5]);
                break;
            case Player_States.OutfitSettings.Shadow:
                myModel = OutfitModels[2];
                myAnim = OutfitAnim[2];
                
                BackspotMain = Backspot[2];
                LeftSpotMain = LeftSpot[2];
                RightSpotMain = RightSpot[2];
                HeadBonePosition = HeadBones[2];

                Destroy(OutfitModels[1]);
                Destroy(OutfitModels[0]);
                Destroy(OutfitModels[3]);
                Destroy(OutfitModels[4]);
                Destroy(OutfitModels[5]);
                break;
            case Player_States.OutfitSettings.Enlightened:
                myModel = OutfitModels[3];
                myAnim = OutfitAnim[3];
                
                BackspotMain = Backspot[3];
                LeftSpotMain = LeftSpot[3];
                RightSpotMain = RightSpot[3];
                HeadBonePosition = HeadBones[3];

                Destroy(OutfitModels[1]);
                Destroy(OutfitModels[2]);
                Destroy(OutfitModels[0]);
                Destroy(OutfitModels[4]);
                Destroy(OutfitModels[5]);
                break;
            case Player_States.OutfitSettings.Boss:
                myModel = OutfitModels[4];
                myAnim = OutfitAnim[4];
                
                BackspotMain = Backspot[4];
                LeftSpotMain = LeftSpot[4];
                RightSpotMain = RightSpot[4];
                HeadBonePosition = HeadBones[4];

                Destroy(OutfitModels[1]);
                Destroy(OutfitModels[2]);
                Destroy(OutfitModels[3]);
                Destroy(OutfitModels[0]);
                Destroy(OutfitModels[5]);
                break;
            case Player_States.OutfitSettings.Basic:
                myModel = OutfitModels[5];
                myAnim = OutfitAnim[5];

                BackspotMain = Backspot[5];
                LeftSpotMain = LeftSpot[5];
                RightSpotMain = RightSpot[5];
                HeadBonePosition = HeadBones[5];

                Destroy(OutfitModels[1]);
                Destroy(OutfitModels[2]);
                Destroy(OutfitModels[3]);
                Destroy(OutfitModels[0]);
                Destroy(OutfitModels[4]);
                break;
        }

        myModel.name = "Model";
        BackSword1.transform.parent = BackspotMain.transform;
        //Backsword2.transform.parent = BackspotMain.transform;
        LeftSword.transform.parent = LeftSpotMain.transform;
        RightSword.transform.parent = RightSpotMain.transform;
        LeftSword.transform.position = LeftSpotMain.transform.position;

        RightSword.transform.position = RightSpotMain.transform.position;

        myModel.transform.parent = ParentToModel;

        ClearMindOverlay.SetActive(false);
        DashEffect.SetActive(false);
        ClearMindOverused = false;
        if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.NotReached && Game.current.Progress.ClearMindLevel == Player_States.ClearMindUpgrade.NotReached)
        {
            ClearMindOverused = true;
        }
        if (LevelGM.Instance.RespawnSFXCanPlay)
        {
            ReviveSFX.Play();
        }
        //SmokeTrail.Stop();
        //if (SmokeTrail != null)
        //{
        //    if (ClingToWall)
        //    {
        //        SmokeTrail.Play();
        //    }
        //    else
        //   {
        //       SmokeTrail.Stop();
        //   }
        // }
        // DamageInvuln = DamageAnimation.length/4; //Sets the DamageInvuln float to the length of the referenced animation clip. Overrides whatever float was put in in inspector
    }
	
	// Update is called once per frame
	void Update () {
        myAnim.SetBool("Hurt",false);
        pauseCheck = LevelGM.Instance.isPaused; //Checks if paused. The Level GM will keep track and this just sets to whatever it says.
       // if (Input.GetKeyDown(KeyCode.L))
       // {
       //     AudioListener.pause = !AudioListener.pause;
       // }
        IFramesTimer -= Time.deltaTime;
        if (IFramesTimer <= 0 && !DamageEnabled)
        {
            DamageEnabled = true;
        }

        if (PlayerState== Player_States.PlayerStates.ClearMind)
        {
            ClearMindOverlay.SetActive(true);
        }
        else
        {
            ClearMindOverlay.SetActive(false);
        }

        if (!pauseMenuCheck)
        {
            if (PlayerHealthState == Player_States.PlayerHealthStates.Dead && deathWaiting)
            {
                myAnim.SetBool("Death", false);
            }
            if (PlayerState == Player_States.PlayerStates.Damaged) //Taking Damage currently.
            {
                DamageState();
            }
            //if (DomahdScript.DomahdState == Player_States.DomahdDebuffState.Damaged)
            //{
            //    DamageStateDomahd();
            //}
            if (PlayerHealthState == Player_States.PlayerHealthStates.Dead && !deathWaiting) //Is dead.
            {
                DeathSetup();
            }
            if (PlayerHealthState == Player_States.PlayerHealthStates.Dead)
            {

                RigidbodyConstraints2D DeathConstraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                myRB.constraints = DeathConstraints;
            }
        }

        if (ClingToWall)
        {
            WallClingSFX.gameObject.SetActive(true);
        }
        else
        {

            WallClingSFX.gameObject.SetActive(false);
        }


        //if (SmokeTrail != null)
        //{
        //    if (ClingToWall)
        //    {
        //        SmokeTrail.Play();
        //    }
        //    else
        //    {
        //        SmokeTrail.Stop();
        //    }
        //}
        //FindNearestEnemy(); //Does function for finding nearest enemy at all times.
        /*if (PlayerState != Player_States.PlayerStates.Perched)
        {
            FindNearestPerch(); //Does function for finding nearest perch point in range.
        }
        if (PlayerState == Player_States.PlayerStates.PounceReady)
        {
            FindNearestPerch();
        }*/
        //if (DomahdScript.DomahdState == Player_States.DomahdDebuffState.Normal) 
        //{ DomahdsFacingAnim.SetBool("FacingDifferent", ClingToWall); } //flips the side the Domahd is facing if player is clinging to the wall or not.
        if (!(PlayerHealthState == Player_States.PlayerHealthStates.Dead))
        {
            if (PlayerState == Player_States.PlayerStates.Crouching || PlayerState == Player_States.PlayerStates.Sliding || PlayerState == Player_States.PlayerStates.MeleeAttacking && MeleeScript.MeleeState == Player_States.PlayerMeleeStates.CrouchAttack)
            {
                myHitBox.SetBool("Crouching", true); //Switches player's collider to crouching size.
            }
            else
            {
                myHitBox.SetBool("Crouching", false); //Switches player's collider to default standing size.
            }
        }
        if (!(PlayerHealthState == Player_States.PlayerHealthStates.Dead))
        {
            if (MyDimensions == DimensionsOfModel.ThreeD)
            {
                if (PlayerState == Player_States.PlayerStates.Dashing)
                {
                    myHitBox.SetBool("Dash", true);
                }
                else
                {
                    myHitBox.SetBool("Dash", false);
                }
            }
        }
	}

    private void FixedUpdate()
    {
        pauseCheck = LevelGM.Instance.isPaused; //Checks if paused. The Level GM will keep track and this just sets to whatever it says.

        
        if (PlayerState == Player_States.PlayerStates.ClearMind)
        {
            ClearMindOverlay.SetActive(true);
            
        }
        else
        {
            ClearMindOverlay.SetActive(false);
        }


        if (PlayerState == Player_States.PlayerStates.Dashing || PlayerState == Player_States.PlayerStates.Sliding)
        {
            DashEffect.SetActive(true);
        }
        else
        {
            DashEffect.SetActive(false);
        }

        /*if (ClingToWall)
        {
            SmokeTrail.Play();
        }
        else
        {
            SmokeTrail.Stop();
        }*/
    }

    public void DeathSetup() //Setting up the player for dying.
    {
        if (!deathSetupHasBEgun)
        {
            deathSetupHasBEgun = true;
            Game.current.Progress.PlayerDeathCount++;
            LevelGM.Instance.PlayDeathTune();
            LevelGM.Instance.PlayerDeath(PlayerObj); //Calls the GM's function for player death to take care of the rest.
                                                     //myVoiceBox.clip = DeathGrunt;
                                                     //myVoiceBox.Play();

          //  DeadAudioSource.Play();
            Debug.Log("Death Setup");
            HealthScript.HP = 0;
            PlayerHealthState = Player_States.PlayerHealthStates.Dead;
            //myColl.enabled = false;
            myRB.gravityScale = 5;
            //myRB.isKinematic = true;
            myRB.velocity = new Vector2(0, 0);
            RigidbodyConstraints2D DeathConstraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            myRB.constraints = DeathConstraints;
            MeleeScript.ResetMeleeState();
            LevelGM.Instance.ClearMind = false;
            ClearMindOverlay.SetActive(false);
            myAnim.SetBool("Dashing", false);
            myAnim.SetBool("ClearMind", false);
            myAnim.SetBool("WallSlash", false);
            myAnim.SetBool("DashEnder", false);
            myAnim.SetBool("AerialSlash", false);
            myAnim.SetBool("CrouchSlash", false);
            myAnim.SetBool("WallCling", false);
            myAnim.SetBool("Crouching", false);
            myAnim.SetBool("Jump", false);
            myAnim.SetBool("Airborne", false);
            myAnim.SetBool("Death", true);
            //SmokeTrail.Stop();
            DashEffect.SetActive(false);
            myAnim.SetTrigger("DeathCall");
            deathWaiting = true;
            Invoke("PlayerDeath", DeathAnimation.length/2);
        }
    }

    public void PlayerDeath() //When death Animation is finished, it does this function now.
    {
        Debug.Log("Dead");

        //LevelGM.Instance.PlayerDeath(PlayerObj); //Calls the GM's function for player death to take care of the rest.

    }

    public void FlipPlayer() //Flip the side the player is facing.
    {
        if (MyDimensions == DimensionsOfModel.TwoD)
        {
            Vector3 tempScale = PlayerObj.transform.localScale;
            tempScale.x *= -1f;
            PlayerObj.transform.localScale = tempScale;
        }
        else if (MyDimensions == DimensionsOfModel.ThreeD)
        {
            switch (FacingRight)
            {
                case true:
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                    break;
                case false:
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                    break;
            }
        }
    }

   /*public GameObject FindNearestEnemy() //Searches all enemies in the scene to find the nearest to the player.
    {
        GameObject[] Gos;
        Gos = GameObject.FindGameObjectsWithTag("Enemy"); //Finds all gameobjects tagged "Enemy" in the scene and adds them to the Gos array.
        NearestEnemy = null; //Resets the current Nearest enemy in the event that no enemy is detected at all.
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position; //Player's position at this frame.
        foreach (GameObject Enemy in Gos)
        {
            Vector3 diff = Enemy.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance) //Checks if enemy is closer than previous enemies gone through in the array.
            {
                NearestEnemy = Enemy;
                distance = curDist;
            }
        }

        return NearestEnemy;
    }*/
    /*public GameObject FindNearestPerch() //Searches for perch point nearest that's in range.
    {
        GameObject[] PerchPoints; 
        PerchPoints = GameObject.FindGameObjectsWithTag("Perch"); //finds every gameobject tagged as "Perch" in the scene.
        NearestPerch = null; //Resets the current nearest in case none are in range at all.
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position; //Player's position this frame.
        foreach (GameObject PerchPoint in PerchPoints)
        {
            Vector3 diff = PerchPoint.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance) //Narrows down every perch point in the array to the nearest one to the player.
            {
                //NearestPerch = PerchPoint;
                //distance = curDist;
                if ((PerchPoint.transform.position.x <= (transform.position.x + PerchDistanceCheck)) && (PerchPoint.transform.position.x >= (transform.position.x - PerchDistanceCheck))) //Checks if the nearest perch point is in range.
                {
                    if ((PerchPoint.transform.position.y <= (transform.position.y + PerchDistanceCheck)) && (PerchPoint.transform.position.y >= (transform.position.y - PerchDistanceCheck)))
                    {
                        NearestPerch = PerchPoint;
                        distance = curDist;
                    }
                }
            }
        }

        return NearestPerch;
    }*/

    public void DamageTaken(int HealthLost) //Public script that is called by anything doing damage when it's currently going to hit the player.
    {
        if (PlayerHealthState != Player_States.PlayerHealthStates.Dead)
        {
            if (DamageEnabled) //Checks if player's invulnerable or not.
            {
                HealthScript.HealthLost(HealthLost); //Takes away from current HP in the Health subscript.
                IFramesTimer = DamageInvuln;
                LevelGM.Instance.NoDamage = false;
                DamageTime = DamageInvuln / 4;
                DamageEnabled = false; //Give player temporary i-frames
                                       //bool HitAnimGo = (HealthScript.DomahdAlive ? true : false); //Checks if Domahd is still out and alive. 
                                       //if (HitAnimGo == true) //Plays damage animation for Domahd if it's not dead.
                                       //{
                                       //    DomahdScript.DomahdAnim.SetTrigger("Damaged");
                                       //}
                                       //myAnim.SetTrigger("Hurt");
                                       //myVoiceBox.clip = DamageGrunt;
                                       //myVoiceBox.Play();
                myAnim.SetBool("Hurt", true);
                LevelGM.Instance.ClearMind = false;
                PlayerDebuffState = Player_States.PlayerDebuffState.Normal;
                //Everything below resets player's states and resets animation booleans and values to cancel out of anything the player is doing while the damage is happening.
                MeleeScript.ResetMeleeState();
                PlayerState = Player_States.PlayerStates.Damaged;
                ClingToWall = false;
                myRB.velocity = new Vector2(0, 0);
                myHitBox.SetBool("Crouching", false);
                myAnim.SetBool("Dashing", false);
                myAnim.SetBool("ClearMind", false);
                //myAnim.SetBool("PounceReady", false);
                //myAnim.SetBool("Pounce", false);
                //myAnim.SetBool("Perched", false);
                myRB.constraints = MovementScript.YContraints[0];
                MovementScript.dashTime = 0;
                MovementScript.WallJumpTime = 0;
                //CurrentPerch = null;
                //MovementScript.PerchScript = null;
                //PerchToIgnore = null;
                damageSound.Play();
                DamageState(); //Calls the damage state function for the first time.
            }
        }
    }

    void DamageState() //The countdown for the damage invulnerability.
    {
        DamageTime -= Time.deltaTime;
        if (DamageTime <= 0 && PlayerState == Player_States.PlayerStates.Damaged)
        {
            PlayerState = Player_States.PlayerStates.Normal; //Resets player to default state after damage animation is done.
            //DamageEnabled = true; //Player's invulnerability is over.
        }
    }

    //void DamageStateDomahd()
    //{
    //    DamageTime -= Time.deltaTime;
    //    if (DamageTime <= 0 && DomahdScript.DomahdState == Player_States.DomahdDebuffState.Damaged)
    //    {
    //        DomahdScript.DomahdState = Player_States.DomahdDebuffState.Normal; //Resets player to default state after damage animation is done.
    //        DamageEnabled = true; //Player's invulnerability is over.
    //    }
    //}
}

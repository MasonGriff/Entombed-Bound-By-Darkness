using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Main))]
public class Player_Movement : MonoBehaviour {

    /// <summary>
    /// PLayer controller script that manages movement abilities.
    /// </summary>
    PlayerController Controls;
     
    Player_Main Player; //Reference to the main player controller.
    

    [Header("Float Variables")]
    [Tooltip("The default gravity.")]
    public float MyGravity;
    public float FallGravity = 3;
    [Tooltip("The input from the controller for horizontal movement.")]
    public float moveInput;
    [Tooltip("The input from the controller for vertical movement.")]
    public float VertInput;
    [Tooltip("Player's current walk/run speed.")]
    public float PlayerSpeed = 1.5f;
    public float PlayerSpeedGrounded = 5;
    public float PlayerSpeedAirborne = 4;
    [Tooltip("Player crouch walk speed.")]
    public float CrawlSpeed = .75f;
    //[Tooltip("Player's punce speed.")]
    //public float PounceSpeed = 2f;
    [Tooltip("Force that controls the height of player's jump during set in stone jump arcs.")]
    public float jumpForce = 6;
    public float jumpVelocity = 6;
    //[Tooltip("Force that controls the height of player's jump when jump has been held down a little.")]
    //public float superJumpForce = 400;
    [Tooltip("Invisible cooldown after jumping before you can double jump in the air.")]
    public float DoubleJumpWait = .0f;
    float DoubleJumpCooldown; //The countdown of the cooldown above.
    [Tooltip("Checking if player has double jumped already.")]
    public bool DoubleJumpCheck = false; //Checking if player has double jumped already.
    public bool DoubleJumpTrue = false;
    [Tooltip("The countdown of dashing.")]
    public float dashTime; //The countdown for dashing
    public float dashJumpForce = 400f;
    public bool dashJumping = false;
    [Tooltip("The duration of a dash (in seconds)")]
    public float dashRecharge = 0.5f;
    //float dashCooldown;
    [Tooltip("The duration of an air-dash (in seconds).")]
    public float airDashRecharge = .25f;
    [Tooltip("Checking if player has air-dashed already.")]
    public bool AirDashCheck = false;
    [Tooltip("The duration of a slide (in seconds)")]
    public float slideRecharge = 0.5f;
    [Tooltip("Speed that player moves whilst dashing.")]
    public float dashingSpeed = 4;
    [Tooltip("Speed that player moves whilst sliding.")]
    public float slideSpeed = 3;
    [Tooltip("Checking if player is mid-wall jump")]
    public bool WallJump = false;
    public bool WallDashJump = false;
    [Tooltip("Speed of player's decent whilst clinging to a wall.")]
    public float WallSlideSpeed = .5f;
    [Tooltip("Horizontal force of player's wall jump.")]
    public float WallJumpPush = 2f;
    [Tooltip("Vertical force of player's wall jump.")]
    public float wallJumpHeight = 10f;
    public float FastMovingWallJumpMultiplier = 2;
    public bool FastMovingWallCling = false;
    public bool DoingFastWallJump;
    public float WallDashJumpPush = 5f;
    public float WallDashJumpHeight = 6f;
    //public float wallJumpXVelocity = 0;
    [Tooltip("Period of time after wall jump before player can input another action.")]
    public float WallJumpCooldown = .2f; //Part of the fix that made wall jumping finally work.
    [Tooltip("The countdown of the wall jump cooldown.")]
    public float WallJumpTime = 0;
    public AnimationClip WallJumpAnim;

    public bool JumpGo = false;
    [Tooltip("Current level of charge for your jump.")]
    public float JumpChargeup = 0;
    [Tooltip("Time(in seconds) for holding down jump to perform the Super Jump. The higher the value of this, the higher/farther the possible jump")]
    public float JumpChargeupMax = .2f;
    bool JumpChargeMaxedOut = false;
    //float DoubleJumpCooldownTimer
    float jumpGoInsurance = 0;
    float jumpGoInsuranceMax = .05f;

    public float invertTimer = 0;

    [Header("Outside of PLayer Scripts")]
    //public PerchPoint PerchScript;

    [Header("Constraints")]
    public RigidbodyConstraints2D[] YContraints = new RigidbodyConstraints2D[2];

    public float SceneBeginWaitTimer=  .3f;
    public float SceneBeginTimerGo = 0;
    float groundedTimerCheck = 0;
    public float GroundedTimerReset = .5f;
    [HideInInspector]
    public bool newGrounded;

    private void Awake()
    {
        //MovementScript
    }

    // Use this for initialization
    void Start()
    {
        SceneBeginTimerGo = SceneBeginWaitTimer;
        Controls = PlayerController.CreateWithDefaultBindings();
        moveInput = 0;
        Player = GetComponent<Player_Main>();
        YContraints[0] = Player.myRB.constraints;
        YContraints[1] = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        JumpChargeup = 0;
        DoubleJumpCooldown = 0;
        invertTimer = 0;
        JumpChargeMaxedOut = false;
        if (MyGravity <=0)
        { MyGravity = Player.myRB.gravityScale; }
    }

    private void Update()
    {
        if (SceneBeginTimerGo >= 0)
        {
            SceneBeginTimerGo -= Time.deltaTime;
;        }
        if (SceneBeginTimerGo < 0)
        {
            if (!(Player.PlayerState == Player_States.PlayerStates.Damaged) && !(Player.PlayerHealthState == Player_States.PlayerHealthStates.Dead))
            {
                //Player.myRB.angularVelocity = 0;
                Player.myRB.inertia = 0;
                Player.myRB.rotation = 0;
                dashTime -= Time.deltaTime;
                invertTimer -= Time.deltaTime;
                if (invertTimer <= 0 && Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls)
                {
                    Player.PlayerDebuffState = Player_States.PlayerDebuffState.Normal;
                    invertTimer = 0;
                }
                if (invertTimer <= 0)
                {

                    invertTimer = 0;
                }
                //DoubleJumpCooldown -= Time.deltaTime;
                if (WallJumpTime <= 0) //Turns off wall jump waiting bool, enabling player to perform actions again.
                {
                    if (!Player.ClingToWall)
                    { FlipModelParent(1); }
                    WallJump = false;
                    if (DoingFastWallJump)
                    {
                        DoingFastWallJump = false;
                        Player.transform.parent = null;
                    }
                }
                WallJumpTime -= Time.deltaTime;
                DoubleJumpCooldown -= Time.deltaTime;
                groundedTimerCheck -= Time.deltaTime;
                GroundTouchingCheck(); //check if grounded.
                {/*if (!Player.pauseCheck && Player.PlayerHealthState != Player_States.PlayerHealthStates.Dead) //Player isn't dead and game isn't paused.
            {
                if (!(Player.PlayerState == Player_States.PlayerStates.Damaged)) //Player isn't currently taking damage
                {
                    if (Player.PlayerState == Player_States.PlayerStates.PounceReady) //Player is hodling down R2, preparing to punce.
                    {
                        if (Controls.Pounce.WasRepeated > .3 || Input.GetButton("Pounce" + LevelGM.Instance.ControllerInput)) //Player is still holding down R2.
                        {
                            PounceReadyState();
                        }
                        else //Player let go of R2.
                        {
                            Player.PlayerState = Player_States.PlayerStates.Normal;
                            Player.myAnim.SetBool("PounceReady", false);
                        }
                    }
                    else if (Player.PlayerState == Player_States.PlayerStates.Pounce) //Player is mid-pounce.
                    {
                        Pounce();
                    }
                    else if (Player.PlayerState == Player_States.PlayerStates.Perched) //Player is currently on perch point.
                    {
                        PerchMovementChecking();
                    }
                    else //Basic Movement outside of pounce and perch.
                    { MovementChecking(); }
                }
            }*/
                } //commented out stuff, moved to Fixed Update.
            }
        }
    }

    private void FixedUpdate()
    {
        //Testing how to tell input types.
        /*PlayerController playerActions = Controls;

        Debug.Log("Last Input Type: " + Controls.LastInputType);
        Debug.Log("Last Device Class: " + playerActions.LastDeviceClass);
        Debug.Log("Last Device Style: " + playerActions.LastDeviceStyle);*/

        //

        if (SceneBeginTimerGo < 0)
        {

            if (invertTimer <= 0 && Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls)
            {
                Player.PlayerDebuffState = Player_States.PlayerDebuffState.Normal;
                
            }

            if (!(Player.PlayerState == Player_States.PlayerStates.Damaged) && !(Player.PlayerHealthState == Player_States.PlayerHealthStates.Dead))
            {
                switch (Player.Grounded) //change speed based on if grounded or airborne.
                {
                    case true:
                        PlayerSpeed = PlayerSpeedGrounded;
                        break;
                    case false:
                        PlayerSpeed = PlayerSpeedAirborne;
                        break;
                    default:
                        PlayerSpeed = PlayerSpeedGrounded;
                        break;
                }
                DoubleJumpCooldown -= Time.deltaTime; //brings down cooldown period for double jump. Required to have double jump function in Fixed Update, not needed if moved back to regular Update.
                JumpChargeup -= Time.deltaTime;
                if (!Player.pauseCheck && Player.PlayerHealthState != Player_States.PlayerHealthStates.Dead) //Player isn't dead and game isn't paused.
                {
                    ClearMindChecking();
                    if (!(Player.PlayerState == Player_States.PlayerStates.Damaged) && !(Player.PlayerState == Player_States.PlayerStates.ClearMind)) //Player isn't currently taking damage
                    {
                        { MovementChecking(); }
                    }
                }
                else if (Player.pauseCheck && Player.PlayerHealthState != Player_States.PlayerHealthStates.Dead && !(Player.PlayerState == Player_States.PlayerStates.Damaged))
                {
                    Player_Main plyr = Player;
                    plyr.myAnim.SetInteger("AnimState", 0);
                    plyr.myAnim.SetBool("Dashing", false);
                    plyr.myAnim.SetBool("Jump", false);
                    plyr.myAnim.SetBool("Airborne", false);
                    plyr.myAnim.SetBool("Dashing", false);
                    plyr.MeleeScript.ResetMeleeState();
                    plyr.myAnim.SetBool("ClearMind", false);
                    plyr.MovementScript.moveInput = 0;
                }
            }
        }
    }

    void ClearMindChecking()
    {
        if (Player.ClearMindOverused && Player.PlayerState == Player_States.PlayerStates.ClearMind)
        {
            Player.PlayerState = Player_States.PlayerStates.Normal;
            LevelGM.Instance.ClearMind = false;
            Player.myAnim.SetBool("ClearMind", false);
        }
        if (Controls.ClearMind.WasPressed)
        {
            if (!(Player.PlayerState == Player_States.PlayerStates.ClearMind) && Player.Grounded && !Player.ClearMindOverused)
            {
                Player.PlayerState = Player_States.PlayerStates.ClearMind;
                Player.myRB.velocity = new Vector2(0, 0);
                dashTime = 0;
                LevelGM.Instance.ClearMind = true;
                Player.myAnim.SetBool("ClearMind", true);
            }
            else
            {
                Player.PlayerState = Player_States.PlayerStates.Normal;
                LevelGM.Instance.ClearMind = false;
                Player.myAnim.SetBool("ClearMind", false);
            }
        }
    }



    void MovementChecking() //Regular movement.
    {
        if (!(Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.MeleeChain) && !(Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.DashEnder) && !(Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.CrouchAttack) && !(Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.WallAttack)) //If not doing melee attacks.
        {
            moveInput = Controls.Move.X; //Horizontal movement input.
            moveInput = ((Player.PlayerDebuffState == Player_States.PlayerDebuffState.InvertControls) ? moveInput * -1 : moveInput);
            { if (moveInput > .2)
                {
                    moveInput = 1;
                }
                else if (moveInput <= .2 && moveInput >= -.2)
                {
                    moveInput = 0;
                }
                else if (moveInput < -.2)
                {
                    moveInput = -1;
                }
            } //Normalize Move Input
            VertInput = Controls.Move.Y; //Vertical input.
            {
                if (VertInput > .45)
                {
                    VertInput = 1;
                }
                else if (VertInput <= .45 && VertInput >= -.45)
                {
                    VertInput = 0;
                }
                else if (VertInput < -.45)
                {
                    VertInput = -1;
                }
            } //Normalize Vert Input
        }

        //if (!(Player.PlayerState == Player_States.PlayerStates.Perched) && !(Player.PlayerState == Player_States.PlayerStates.Pounce) && !(Player.PlayerState == Player_States.PlayerStates.PounceReady)) //Player is doing nothing pounce or perch related.
        {
            CrouchChecking(); //Check if crouching.
            WallTouchingCheck(); //Check if clinging to wall.
            DashingCheck(); //Check if input is pressed for dashing or sliding.
            switch (DoubleJumpCheck)
            {
                case false:
                    JumpChecking();
                    break;
                case true:
                    DoubleJumpChecking();
                    break;
            }

            //JumpChecking(); //Check is input is pressed for jumping.
            if (!WallJump && WallJumpTime <= 0) //Not wall jumping.
            {
                //=== Standing, Not Dashing, and not damaged/dead/crouching/attacking ===
                if (Player.PlayerState == Player_States.PlayerStates.Normal)
                {
                    Player.myRB.velocity = new Vector2(moveInput * PlayerSpeed, Player.myRB.velocity.y);
                }
                else if (Player.PlayerState == Player_States.PlayerStates.Crouching)
                {
                    Player.myRB.velocity = new Vector2(moveInput * CrawlSpeed, Player.myRB.velocity.y);
                }
                //===Flip Direction Facing ===
                if (moveInput >= .01 && !Player.FacingRight && !(Player.PlayerState == Player_States.PlayerStates.Dashing))
                {
                    Player.FacingRight = true;
                    Player.FlipPlayer();
                }
                else if (moveInput <= -.01 && Player.FacingRight && !(Player.PlayerState == Player_States.PlayerStates.Dashing))
                {
                    Player.FacingRight = false;
                    Player.FlipPlayer();
                }

                //=== Animate ===
                if (Mathf.Abs(moveInput) > .1f && Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.None)
                {
                    Player.myAnim.SetInteger("AnimState", 1);
                }
                else
                {
                    Player.myAnim.SetInteger("AnimState", 0);
                }
                

                //=== Jumping Additions ===
                if (JumpGo)
                {
                    if (JumpChargeup <=0 || Controls.Jump.WasReleased)
                    {
                        //<animation change to be added>
                        Player.myRB.gravityScale = FallGravity;
                        JumpGo = false;

                    }
                    else
                    {
                        if (JumpChargeup > 0)
                        {
                            Player.myRB.gravityScale = MyGravity;
                            Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, jumpVelocity);
                        }
                    }
                }
                //=== Jump Animate ===
                switch (newGrounded)
                {
                    case true:
                        Player.myAnim.SetBool("Jump", false);
                        Player.myAnim.SetBool("Airborne", false);
                        break;
                    case false:
                        switch (JumpGo)
                        {
                            case true:
                                Player.myAnim.SetBool("Jump", true);
                                Player.myAnim.SetBool("Airborne", false);
                                break;
                            case false:
                                Player.myAnim.SetBool("Airborne", true);
                                Player.myAnim.SetBool("Jump", false);
                                break;
                        }
                        break;
                }

                //=== If Dashing ===
                if (Player.PlayerState == Player_States.PlayerStates.Dashing)
                {
                    if (!newGrounded) //Lock vertical position for air-dash.
                    {
                        Player.myRB.constraints = YContraints[1];
                    }

                    if (dashTime <= 0 || Controls.Dash.WasReleased) //Not dashing anymore or let go of dashing button, returns player to normal state.
                    {
                        Player.myAnim.SetBool("Dashing", false);
                        Player.myRB.constraints = YContraints[0];
                        Player.PlayerState = Player_States.PlayerStates.Normal;
                        Player.SlideHappening = false;
                    }
                    else
                    {
                        Player.myAnim.SetBool("Dashing", true);
                        if (dashTime > 0)
                        {
                            if (Player.FacingRight)
                            {
                                Player.myRB.velocity = new Vector2(dashingSpeed, Player.myRB.velocity.y);
                            }
                            else if (!Player.FacingRight)
                            {
                                Player.myRB.velocity = new Vector2(-dashingSpeed, Player.myRB.velocity.y);
                            }
                        }
                    }
                }
                //=== If Sliding ===
                else if (Player.PlayerState == Player_States.PlayerStates.Sliding)
                {
                    if (dashTime <= 0)
                    {
                        Player.myAnim.SetBool("Dashing", false);
                        Player.myRB.constraints = YContraints[0];
                        Player.PlayerState = Player_States.PlayerStates.Normal;
                        Player.SlideHappening = false;
                    }
                    else
                    {
                        Player.myAnim.SetBool("Dashing", true);
                        if (dashTime > 0)
                        {
                            if (Player.FacingRight)
                            {
                                Player.myRB.velocity = new Vector2(slideSpeed, Player.myRB.velocity.y);
                            }
                            else if (!Player.FacingRight)
                            {
                                Player.myRB.velocity = new Vector2(-slideSpeed, Player.myRB.velocity.y);
                            }
                        }
                    }
                }
                if (Player.PlayerState != Player_States.PlayerStates.Dashing && Player.PlayerState != Player_States.PlayerStates.Sliding) //Disable animation for sliding/dashing.
                {
                    Player.myAnim.SetBool("Dashing", false);
                }
            }
            if (WallJumpTime > 0) //Player is still locked into wall jump.
            {
                FlipModelParent(-1);
                Player.myAnim.SetBool("Jump", true);
                Player.myAnim.SetBool("Airborne", false);
                Player.myRB.velocity = new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1), wallJumpHeight); //Keeps a consistent velocity for wall jumping until player can input again.
            }
            if (WallJumpTime <= 0 && WallJump && !Player.Grounded) //No longer wall jumping.
            {
                //Player.myRB.AddForce(new Vector2(Player.FacingRight ? (PlayerSpeed *2) : (PlayerSpeed*-2), 0f));
                WallJump = false;
                Player.myAnim.SetBool("Airborne", true);
                Player.myAnim.SetBool("Jump", false);
                Player.myRB.gravityScale = FallGravity;
            }
        }

    }
    

    void GroundTouchingCheck() //Checking if grounded.
    {
        newGrounded = Physics2D.OverlapCircle(Player.GroundCheck.position, Player.groundRadius, Player.whatisground);

        if (newGrounded)
        {
            if (!Player.Grounded)
            {
                Player.JumpFall.Stop();
                Player.JumpFall.clip = Player.LandClip;
                Player.JumpFall.Play();
            }
            Player.Grounded = true;
            groundedTimerCheck = GroundedTimerReset;
        }
        else
        {
            if (groundedTimerCheck <= 0)
            {
                Player.Grounded = false;
            }

        }

        if (Player.Grounded) //Resets airdash and double jump when grounded.
        {
            DoubleJumpCheck = false;
            DoubleJumpTrue = false;
            AirDashCheck = false;
            //JumpGo = false;
            Player.myRB.gravityScale = MyGravity;
        }

        //Animation Checking
        if (!newGrounded && !(Controls.Jump.WasPressed)) //Player is airborne, setting animation.
        {
            if (!JumpGo)
            { Player.myAnim.SetBool("Airborne", true); }
        }
        else if (newGrounded && !(Controls.Jump.WasPressed)) //Player is grounded, setting animation.
        {
            Player.myAnim.SetBool("Airborne", false);
            Player.myAnim.SetBool("Jump", false);
        }
        if (Player.ClingToWall) //PLayer is clinging to wall and no longer grounded as a result.
        {
            //Player.Grounded = false;
        } 
    }
    


    void WallTouchingCheck() //Check if clinging to wall.
    {
        if (Player.WallCheckSCript.TouchedWall != null) //The wall touch script has detected a wall.
        {
            if (Player.touchingWall && !Player.Grounded) //Player_Main has accepted that player is touching wall. Additional failsafe if something goes wrong with above if statement.
            {
                if (Player.WallCheckSCript.TouchedWall.transform.position.x < Player.PlayerObj.transform.position.x && moveInput < -.1 || Player.WallCheckSCript.TouchedWall.transform.position.x > Player.PlayerObj.transform.position.x && moveInput > .1) //if moving towards wall
                {
                    if (!Player.ClingToWall) //Resets the melee attack states.
                    {
                        Player.MeleeScript.ResetMeleeState();
                    }
                    if (Player.PlayerState == Player_States.PlayerStates.Dashing) //Cancels out of dash.
                    {
                        Player.PlayerState = Player_States.PlayerStates.Normal;
                        dashTime = 0;
                    }
                    Player.ClingToWall = true;
                    //Player.transform.parent = Player.WallCheckSCript.TouchedWall.transform;
                }
                else //Not clinging to wall anymore.
                {
                    //Player.myRB.gravityScale = 1;
                    Player.myRB.constraints = YContraints[0];
                    Player.ClingToWall = false;
                }
            }
            else //Not clinging to wall anymore.
            {
                //Player.myRB.gravityScale = 1;
                Player.myRB.constraints = YContraints[0];
                Player.ClingToWall = false;
            }
        }
        else //Not clinging to wall anymore.
        {
            //Player.myRB.gravityScale = 1;
            Player.myRB.constraints = YContraints[0];
            Player.ClingToWall = false;
            Player.myAnim.SetBool("WallCling", false);

        }

        if (Player.ClingToWall || Player.MeleeScript.MeleeState == Player_States.PlayerMeleeStates.WallAttack) //If clinging to wall or performing melee while on wall.
        {
            //Resetting Double jump and air dash.
            DoubleJumpCheck = false;
            DoubleJumpTrue = false;
            AirDashCheck = false;
            JumpGo = false;
            //Decending slowly down wall
            Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, -WallSlideSpeed);

            Player.myRB.constraints = YContraints[0];
            //Player.myRB.gravityScale = MyGravity;
            Player.myRB.gravityScale = 0.25f*(MyGravity);
            Player.myAnim.SetBool("WallCling", true);
            FlipModelParent(-1); //Has player model facing the way opposite of their input when clinging to wall. As player's input is toward the wall, you don't want player facing into the wall.
        }
        else //Reset animations and wall cling related thing when no longer clinging to wall.
        {
            if (!DoingFastWallJump)
            {
                Player.transform.parent = null;
            }
            Player.myRB.gravityScale = MyGravity;
            Player.myRB.constraints = YContraints[0];
            Player.myAnim.SetBool("WallCling", false);
            FlipModelParent(1);
        }
        if (Player.Grounded)
        {
            Player.ClingToWall = false;
            Player.myRB.gravityScale = MyGravity;
            Player.myRB.constraints = YContraints[0];
            Player.myAnim.SetBool("WallCling", false);
            FlipModelParent(1);
        }
    }

    void DashingCheck() //Check for input for dashing and perform the dash when pressed.
    {
        if (dashTime <= 0 && Player.PlayerState != Player_States.PlayerStates.Dashing) //Not already dashing.
        {
            if (Controls.Dash.WasPressed && !AirDashCheck && !Player.ClingToWall) //Dash is pressed.
            {

                Player.JumpFall.Stop();
                Player.JumpFall.clip = Player.dashSfxClip;
                Player.JumpFall.Play();
                if (!(Player.PlayerState == Player_States.PlayerStates.Crouching)) //Performing dash and not slide.
                {

                    dashTime = (newGrounded ? dashRecharge : airDashRecharge);
                    Player.MeleeScript.ResetMeleeState();
                    JumpGo = false;
                    Player.myAnim.SetBool("Dashing", true);
                    Player.myAnim.SetBool("Jump", false);
                    Player.PlayerState = Player_States.PlayerStates.Dashing;
                    if (!newGrounded)
                    {
                        AirDashCheck = true;
                    }
                    Debug.Log("Dash");
                    Player.myRB.velocity = new Vector2(0, 0);
                    if (WallJump)
                    {
                        WallJump = false;
                        WallJumpTime = 0;
                    }
                    if (!WallJump)
                    {
                        if (moveInput >= .01 && !Player.FacingRight)
                        {
                            Player.FacingRight = true;
                            Player.FlipPlayer();
                        }
                        else if (moveInput <= -.01 && Player.FacingRight)
                        {
                            Player.FacingRight = false;
                            Player.FlipPlayer();
                        }
                    }
                }
                else if (Player.PlayerState == Player_States.PlayerStates.Crouching) //Performing slide and not dash.
                {
                    Player.PlayerState = Player_States.PlayerStates.Sliding;
                    Player.MeleeScript.ResetMeleeState();
                    dashTime = slideRecharge;
                    Player.SlideHappening = true;
                    Debug.Log("Dash");
                    Player.myRB.velocity = new Vector2(0, 0);
                    if (WallJump)
                    {
                        WallJump = false;
                        WallJumpTime = 0;
                    }
                    if (!WallJump)
                    {
                        if (moveInput >= .01 && !Player.FacingRight)
                        {
                            Player.FacingRight = true;
                            Player.FlipPlayer();
                        }
                        else if (moveInput <= -.01 && Player.FacingRight)
                        {
                            Player.FacingRight = false;
                            Player.FlipPlayer();
                        }
                    }
                }
            }
        }
    }

    void DoubleJumpChecking() //Made separate from Jumping Script to function while in Fixed Update.
    {
        //<insert if statement for double jump unlocked>
        {
            if (!Player.Grounded && DoubleJumpCheck && !DoubleJumpTrue && !Player.ClingToWall && DoubleJumpCooldown <= 0) //Haven't double jumped yet and cooldown before you can is up.
            {

                if (Controls.Jump.WasPressed)
                {
                    Player.JumpFall.Stop();
                    Player.JumpFall.clip = Player.doubleClip;
                    Player.JumpFall.Play();
                    JumpGo = false;
                    Player.myRB.gravityScale = MyGravity;
                    Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, 0);
                    Player.myRB.gravityScale = MyGravity;
                    Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, jumpVelocity);
                    Player.MeleeScript.ResetMeleeState();
                    DoubleJumpTrue = true;
                    Debug.Log("Double Jump");
                    Player.myRB.constraints = YContraints[0];
                    if (Player.PlayerState == Player_States.PlayerStates.Normal || Player.PlayerState == Player_States.PlayerStates.MeleeAttacking)
                    {
                        Player.myRB.AddForce(new Vector2(0f, jumpForce));

                        Player.myAnim.SetBool("Jump", true);
                    }
                    else if (Player.PlayerState == Player_States.PlayerStates.Dashing)
                    {
                        Player.PlayerState = Player_States.PlayerStates.Normal;
                        Player.MeleeScript.ResetMeleeState();
                        Player.myRB.AddForce(new Vector2(0f, jumpForce * 1.25f));
                        Player.myAnim.SetBool("Jump", true);
                        Player.myAnim.SetBool("Dashing", false);
                    }
                    Player.myAnim.SetTrigger("Double");
                    Player.myAnim.SetBool("Jump", false);
                    Player.myRB.gravityScale = FallGravity;
                }
            }
        }
    }

    void JumpChecking() //Check if jumping.
    {
        if (!Player.Grounded && !Player.ClingToWall) //Check if completely airborne.
        {
            DoubleJumpCheck = true;
        }
        if (!Player.Grounded && DoubleJumpCheck && !DoubleJumpTrue && !Player.ClingToWall) //Artifact from when double jump was under JumpChecking(), remains here so as not to break anything.
        { }
        else if (Player.Grounded && !Player.ClingToWall) //Jump off ground.
        {

            if (Controls.Jump.WasPressed && !JumpGo)
            {
                Player.JumpFall.Stop();
                Player.JumpFall.clip = Player.doubleClip;
                Player.JumpFall.Play();
                JumpGo = true;
                JumpChargeup = JumpChargeupMax;
                jumpGoInsurance = jumpGoInsuranceMax;
                Player.MeleeScript.ResetMeleeState();
                Player.myRB.velocity = new Vector2(0, 0);
                DoubleJumpCheck = true;
                groundedTimerCheck = 0;
                if (Player.PlayerState == Player_States.PlayerStates.Dashing)
                {
                    JumpGo = false;
                    Player.PlayerState = Player_States.PlayerStates.Normal;
                    Player.myRB.constraints = YContraints[0];
                    Player.myRB.AddForce(new Vector2(0f, (dashJumpForce)));
                    //Player.myAnim.SetTrigger("Jump");
                    Player.myAnim.SetBool("Dashing", false);
                    Debug.Log("Dash Jump");
                }
            }

            {//if (Input.GetButtonDown("Jump" + LevelGM.Instance.ControllerInput))
             /*if (JumpGo && Input.GetButtonUp("Jump" + LevelGM.Instance.ControllerInput) || JumpGo && JumpChargeMaxedOut)
             {
                 JumpGo = false;
                 JumpChargeup = 0;
                 DoubleJumpCooldown = DoubleJumpWait;
                 Player.MeleeScript .ResetMeleeState();
                 Player.myRB.velocity = new Vector2(0, 0);
                 DoubleJumpCheck = true;
                 Player.myRB.constraints = YContraints[0];
                 if(!JumpChargeMaxedOut)
                 { Debug.Log("Jump"); }
                 if (Player.PlayerState == Player_States.PlayerStates.Normal || Player.PlayerState == Player_States.PlayerStates.MeleeAttacking)
                 {
                     Player.myRB.AddForce(new Vector2(0f, (JumpChargeMaxedOut? superJumpForce:jumpForce)));

                     Player.myAnim.SetTrigger("Jump");
                 }
                 else if (Player.PlayerState == Player_States.PlayerStates.Dashing)
                 {
                     Player.PlayerState = Player_States.PlayerStates.Normal;
                     Player.myRB.AddForce(new Vector2(0f, (JumpChargeMaxedOut ? superJumpForce : jumpForce) * 1.25f));
                     Player.myAnim.SetTrigger("Jump");
                     Player.myAnim.SetBool("Dashing", false);
                 }
                 JumpChargeMaxedOut = false;


             }*/
            } //old jump funtion. [commented out]
        }
        else if (Player.ClingToWall) //Wall Jump
        {
            if (Controls.Jump.WasPressed)
            {
                DoubleJumpCooldown = DoubleJumpWait;
                Player.myRB.constraints = YContraints[0];
                Player.myRB.gravityScale = MyGravity;
                DoubleJumpCheck = false;
                DoubleJumpTrue = false;
                AirDashCheck = false;
                Debug.Log("Jump");
                //JumpGo = true;
                //Player.myAnim.SetBool("Jump", true);
                Player.myAnim.SetTrigger("WallKick");

                Player.JumpFall.Stop();
                Player.JumpFall.clip = Player.doubleClip;
                Player.JumpFall.Play();
                if (Player.PlayerState == Player_States.PlayerStates.Normal || Player.PlayerState == Player_States.PlayerStates.MeleeAttacking)
                {
                    
                    if (!FastMovingWallCling)
                    {
                        Player.MeleeScript.ResetMeleeState();
                        WallJump = true;
                        //WallJumpTime = WallJumpCooldown * 1.5f;
                        WallJumpTime = WallJumpAnim.length;
                        FlipModelParent(-1);
                        //Player.myRB.velocity = new Vector2(0, 0);
                        Player.myRB.gravityScale = MyGravity;
                        Player.myRB.constraints = YContraints[0];
                        //Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, 0);
                        Player.myRB.velocity = new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier : 1), wallJumpHeight);
                    }
                    else
                    {
                        Player.MeleeScript.ResetMeleeState();
                        WallJump = true;
                        //WallJumpTime = WallJumpCooldown;
                        WallJumpTime = WallJumpAnim.length;
                        FlipModelParent(-1);
                        DoingFastWallJump = true;
                        Player.myRB.gravityScale = (.5f * MyGravity);
                        Player.myRB.constraints = YContraints[0];
                        Player.myRB.velocity = new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier: 1), wallJumpHeight);
                        Player.myRB.AddForce(new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier: 1), 0));
                        Player.myRB.gravityScale = MyGravity;
                    }
                    
                
                    // Player.myRB.AddForce(Vector3.up* WallJumpPush, ForceMode2D.VelocityChange));
                }
                /*else if (Player.PlayerState == Player_States.PlayerStates.Dashing)
                {
                    Player.PlayerState = Player_States.PlayerStates.Normal;
                    WallJump = true;
                    WallJumpTime = WallJumpCooldown;
                    Player.myRB.constraints = YContraints[0];
                    Player.myRB.velocity = new Vector2(WallJumpPush * 1.5f *(Player.FacingRight ? -1 : 1), 1.5f*wallJumpHeight);
                    Player.myAnim.SetBool("Dashing", false);
                }*/


            }
        }

    }
    /*
    void WallJumpAGo()
    {
        DoubleJumpCooldown = DoubleJumpWait;
        Player.myRB.constraints = YContraints[0];
        Player.myRB.gravityScale = MyGravity;
        DoubleJumpCheck = false;
        DoubleJumpTrue = false;
        AirDashCheck = false;
        Debug.Log("Jump");
        //JumpGo = true;
        //Player.myAnim.SetBool("Jump", true);
        Player.myAnim.SetTrigger("WallKick");
        if (Player.PlayerState == Player_States.PlayerStates.Normal || Player.PlayerState == Player_States.PlayerStates.MeleeAttacking)
        {

            if (!FastMovingWallCling)
            {
                Player.MeleeScript.ResetMeleeState();
                WallJump = true;
                WallJumpTime = WallJumpCooldown * 1.5f;
                //Player.myRB.velocity = new Vector2(0, 0);
                Player.myRB.gravityScale = MyGravity;
                Player.myRB.constraints = YContraints[0];
                //Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, 0);
                Player.myRB.velocity = new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier : 1), wallJumpHeight);
            }
            else
            {
                Player.MeleeScript.ResetMeleeState();
                WallJump = true;
                WallJumpTime = WallJumpCooldown;
                DoingFastWallJump = true;
                Player.myRB.gravityScale = (.5f * MyGravity);
                Player.myRB.constraints = YContraints[0];
                Player.myRB.velocity = new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier : 1), wallJumpHeight);
                Player.myRB.AddForce(new Vector2(WallJumpPush * (Player.FacingRight ? -1 : 1) * (FastMovingWallCling ? FastMovingWallJumpMultiplier : 1), 0));
                Player.myRB.gravityScale = MyGravity;
            }


            // Player.myRB.AddForce(Vector3.up* WallJumpPush, ForceMode2D.VelocityChange));
        }
        /*else if (Player.PlayerState == Player_States.PlayerStates.Dashing)
        {
            Player.PlayerState = Player_States.PlayerStates.Normal;
            WallJump = true;
            WallJumpTime = WallJumpCooldown;
            Player.myRB.constraints = YContraints[0];
            Player.myRB.velocity = new Vector2(WallJumpPush * 1.5f *(Player.FacingRight ? -1 : 1), 1.5f*wallJumpHeight);
            Player.myAnim.SetBool("Dashing", false);
        }*/
    //}

    void CrouchChecking() //Check if crouching.
    {
        if (!(Player.PlayerState == Player_States.PlayerStates.Crouching) && VertInput < -.01 && Player.Grounded && !(Player.PlayerState == Player_States.PlayerStates.Sliding)) //Player is holding down.
        {
            Player.PlayerState = Player_States.PlayerStates.Crouching;
            //Player.myRB.velocity = new Vector2(0, 0);

        }
        if (Player.PlayerState == Player_States.PlayerStates.Crouching || Player.PlayerState == Player_States.PlayerStates.Sliding) //Player's state is crouching or sliding.
        {
            Player.myAnim.SetBool("Crouching", true); //Set animator to crouch.
            if (VertInput >= 0) //Player not holding down anymore.

            {
                Player.PlayerState = Player_States.PlayerStates.Normal;
                Player.myAnim.SetBool("Crouching", false);
            }
        }

        else //Not crouching.
        {
            Player.myAnim.SetBool("Crouching", false);

        }
    }

    public void FlipModelParent(float FlipValue) //Change if facing left or right when touching wall.
    {
        Vector3 tempScale = Player.ModelFlippingParent.transform.localScale;
        tempScale.x = FlipValue;
        Player.ModelFlippingParent.transform.localScale = tempScale;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
    }

    private void OnCollisionStay2D(Collision2D myTrigg)
    {
        if (myTrigg.transform.tag == "FastWall")
        {
            if (Player.ClingToWall)
            {
                FastMovingWallCling = true;
            }
        }
        if(myTrigg.transform.tag == "MovingPlatform")
        {
            transform.parent = myTrigg.transform;
        }
    }

    void OnCollisionExit2D(Collision2D myTrigg)
    {
        if (myTrigg.transform.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
        if (myTrigg.transform.tag == "FastWall")
        {
            FastMovingWallCling = false;
        }
    }
}

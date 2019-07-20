using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(Player_Main))]
public class Player_Melee : MonoBehaviour {

    PlayerController Controls;

    public int SwordsOwned = 0;



    [Tooltip("The Head script of the plater controller.")]
    public Player_Main Player; //Reference to the PLayer main script.
    [Tooltip("The current melee action being undertaken by the player.")]
    public Player_States.PlayerMeleeStates MeleeState = Player_States.PlayerMeleeStates.None;
    [Tooltip("Countdown of current melee attack animation playing.")]
    public float AnimationTimer;
    [Tooltip("The damage output detected by enemies. It is changed by each attack done.")]
    public int DamageDealt = 0;
    public int DamageOutput= 0;

    [Header("Object Reference")]
    public GameObject Sword1;
    public GameObject Sword2;
    public GameObject Back1,Back2;

    [Header("Standing Melee")]
    [Tooltip("Damage done by the Standing Melee Attack.")]
    public int StandingMeleeDamage = 1;
    [Tooltip("For detecting which arm is performing the next melee attack.")]
    public bool StandingArmAlternate = false;
    [Tooltip("The attack in the standing melle combo being done currently.")]
    public int MeleeChainNumber = 0; //The attack in the combo for standing melee.
    [Tooltip("Animation of standing melee attack. Used to get the length for timing.")]
    public AnimationClip StandingAnimReference;
    public AnimationClip StandingAnimReferenceSword;
    public AnimationClip StandingAnimReference2Sword;

    [Header("Dash Attack")]
    [Tooltip("Damage performed by dashing attack.")]
    public int DashEnderDamage = 2;
    [Tooltip("Bool for seeing if dash attack is done.")]
    public bool DashEndBoolChanger = false;
    [Tooltip("Animation of dashing melee attack. Used to get the length for timing.")]
    public AnimationClip DashAttackAnimReference;
    public AnimationClip DashAttackAnimReferenceSword;
    public AnimationClip DashAttackAnimReference2Sword;

    [Header("Aerial Attack")]
    [Tooltip("Damage done by aerial attack.")]
    public int AerialDamage = 1;
    [Tooltip("Animation of aerial melee attack. Used to get the length for timing.")]
    public AnimationClip AerialAnimReference;
    public AnimationClip AerialAnimReferenceSword;
    public AnimationClip AerialAnimReference2Sword;

    [Header("Crawl/Crouch Attack")]
    [Tooltip("Damage delt by melee while crouched.")]
    public int CrouchDamage = 1;
    [Tooltip("Animation of crouching melee attack. Used to get the length for timing.")]
    public AnimationClip CrouchAnimReference;
    public AnimationClip CrouchAnimReferenceSword;
    public AnimationClip CrouchAnimReference2Sword;

    [Header("Wall Clinging Attack")]
    [Tooltip("Damage done by melee while wall clinging.")]
    public int WallClingDamage = 1;
    [Tooltip("Animation of melee attack while wall clinging. Used to get the length for timing.")]
    public AnimationClip WallAnimReference;
    public AnimationClip WallAnimReferenceSword;
    public AnimationClip WallAnimReference2Sword;

    private void Awake()
    {
        DamageOutput = 0;
    }

    // Use this for initialization
    void Start () {
        StandingArmAlternate = false;
        Controls = PlayerController.CreateWithDefaultBindings();
        SwordsCurrentlyOwned();
        BackSwords();
	}
    private void Update()
    {
        AnimationTimer = (AnimationTimer <= 0 ? AnimationTimer = 0 : AnimationTimer -= Time.deltaTime);
    }

    // Update is called once per frame
    void  FixedUpdate () {
        
        
        if (DashEndBoolChanger) //Dash is being eased out of for the dash attack.
        {
            Player.myAnim.SetBool("Dashing", false);
            Player.myRB.velocity = new Vector2(Player.myRB.velocity.x /1.1f, Player.myRB.velocity.y);
        }
        

        if (!(Player.PlayerState == Player_States.PlayerStates.Damaged) && !(Player.PlayerHealthState == Player_States.PlayerHealthStates.Dead)) //not dead.
        {
            if (AnimationTimer <= 0 && !(MeleeState == Player_States.PlayerMeleeStates.None) && !StandingArmAlternate)
            {
                ResetMeleeState(); //Resets the melee states once attack animation is over.
            }
            /*else if (AnimationTimer > 0 && (MeleeState == Player_States.PlayerMeleeStates.MeleeChain) && !StandingArmAlternate && MeleeChainNumber == 1 && (AnimationTimer <= ((StandingAnimReference.length * .75f)-.1f)))
            {
                if (Controls.Melee.WasPressed)
                {
                    StandingArmAlternate = true;
                }
            }
            else if ((AnimationTimer <= 0 && !(MeleeState == Player_States.PlayerMeleeStates.None) && StandingArmAlternate))
            {
                StandingMeleeCombo2();
            }*/
            if (Controls.Melee.WasPressed && SwordsOwned > 0) //Check for melee inputs.
            {
                MeleeInputChecking();
            }
        }
	}

    public void ResetMeleeState() //Resets all values for melee attacks. Used when animation for attack is over or player is taking damage.
    {
        BackSwords();
        DamageDealt = 0;
        Player.myAttackBox.SetInteger("Standing", 0);
        Player.myAttackBox.SetTrigger("Reset");
        StandingArmAlternate = false;
        //Player.myAnim.SetTrigger("MeleeReset");
        Player.myAnim.SetBool("DashEnder", false);
        Player.myAnim.SetBool("AerialSlash", false);
        Player.myAnim.SetBool("CrouchSlash", false);
        Player.myAnim.SetBool("WallSlash", false);
        Player.myAnim.SetInteger("Standing", 0);
        MeleeChainNumber = 0;
        DashEndBoolChanger = false;
        MeleeState = Player_States.PlayerMeleeStates.None;
        if (Player.PlayerState == Player_States.PlayerStates.MeleeAttacking)
        { Player.PlayerState = Player_States.PlayerStates.Normal; }
        //Debug.Log("Melee Reset");

    }

    void MeleeInputChecking() //Call other functions that checks for melee inputs.
    {
        HandSwords();
        StandingMeleeCombo();
        DashEnder();
        AerialSlash();
        CrouchSlash();
        WallClingSlash();
    }

    void StandingMeleeCombo()
    {
        if (Player.Grounded)
        {
            if (MeleeChainNumber <= 0 && MeleeState == Player_States.PlayerMeleeStates.None && Player.PlayerState == Player_States.PlayerStates.Normal && !StandingArmAlternate)
            {
                Player.myVoiceBox.Play();
                Player.myRB.velocity = new Vector2(0, 0);
                Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
                //StandingArmAlternate = true;
                MeleeState = Player_States.PlayerMeleeStates.MeleeChain;
                MeleeChainNumber = 1;
                AnimationTimer = (StandingAnimReference.length * .5f);
                Player.myAnim.SetInteger("Standing", 1);
                Player.myAttackBox.SetInteger("Standing", 1);
                DamageDealt = DamageOutput;
                Debug.Log("Claw Standing Slash");
            }
            /*else if (MeleeChainNumber <= 0 && MeleeState == Player_States.PlayerMeleeStates.None && Player.PlayerState == Player_States.PlayerStates.Normal && StandingArmAlternate
                || MeleeChainNumber <= 0 && MeleeState == Player_States.PlayerMeleeStates.MeleeChain && Player.PlayerState == Player_States.PlayerStates.Normal && StandingArmAlternate)
            {
                Player.myRB.velocity = new Vector2(0, 0);
                Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
                StandingArmAlternate = false;
                MeleeState = Player_States.PlayerMeleeStates.MeleeChain;
                MeleeChainNumber = 2;
                AnimationTimer = StandingAnimReference2.length;
                Player.myAnim.SetInteger("Standing", 2);
                Player.myAttackBox.SetInteger("Standing", 2);
                DamageDealt = StandingMeleeDamage;
                Debug.Log("Claw Standing Slash, other hand");
            }*/
        }
    }

    /*void StandingMeleeCombo2()
    {
        Player.myRB.velocity = new Vector2(0, 0);
        Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
        StandingArmAlternate = false;
        MeleeState = Player_States.PlayerMeleeStates.MeleeChain;
        MeleeChainNumber = 2;
        AnimationTimer = (StandingAnimReference2.length *.25f);
        Player.myAnim.SetInteger("Standing", 2);
        Player.myAttackBox.SetInteger("Standing", 2);
        DamageDealt = StandingMeleeDamage;
        Debug.Log("Claw Standing Slash combo");
    }*/

    void DashEnder()
    {
        if (Player.PlayerState == Player_States.PlayerStates.Dashing && !(Player.PlayerState==Player_States.PlayerStates.MeleeAttacking) /*&& Player.Grounded*/ && !(Player.ClingToWall))
        {
            Player.myVoiceBox.Play();
            Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
            MeleeState = Player_States.PlayerMeleeStates.DashEnder;
            DamageDealt = DamageOutput;
            AnimationTimer = DashAttackAnimReference.length;
            DashEndBoolChanger = true;
            Player.MovementScript.dashTime = 0;
            Player.myRB.constraints = Player.MovementScript.YContraints[0];
            Player.myAttackBox.SetTrigger("Dash");
            Player.myAnim.SetBool("DashEnder", true);
            //Player.myAnim.SetBool("Dashing", false);
            //Player.myRB.velocity = new Vector2(0, 0);
            Debug.Log("DashAttack");
        }
    }

    void AerialSlash()
    {
        if (Player.PlayerState == Player_States.PlayerStates.Normal && MeleeState == Player_States.PlayerMeleeStates.None && !Player.MovementScript.newGrounded && !Player.ClingToWall)
        {
            Player.myVoiceBox.Play();
            Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
            MeleeState = Player_States.PlayerMeleeStates.AirAttack;
            DamageDealt = DamageOutput;
            AnimationTimer = (AerialAnimReference.length * .5f);
            Player.myRB.velocity = new Vector2(Player.myRB.velocity.x, 0);
            Player.myAttackBox.SetTrigger("Aerial");
            Player.myAnim.SetBool("AerialSlash", true);
            Debug.Log("Aerial Slash");
        }
    }

    void CrouchSlash()
    {
        if (Player.PlayerState == Player_States.PlayerStates.Crouching && MeleeState == Player_States.PlayerMeleeStates.None && Player.Grounded && !Player.ClingToWall)
        {
            Player.myVoiceBox.Play();
            Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
            MeleeState = Player_States.PlayerMeleeStates.CrouchAttack;
            DamageDealt = DamageOutput;
            AnimationTimer = (CrouchAnimReference.length * .5f);
            Player.myAttackBox.SetTrigger("Crouch");
            Player.myAnim.SetBool("CrouchSlash", true);
            Debug.Log("Crouch Attack");
        }
    }
    
    void WallClingSlash()
    {
        if (Player.PlayerState == Player_States.PlayerStates.Normal && !(Player.PlayerState == Player_States.PlayerStates.MeleeAttacking) && Player.ClingToWall)
        {
            Player.myVoiceBox.Play();
            Player.PlayerState = Player_States.PlayerStates.MeleeAttacking;
            MeleeState = Player_States.PlayerMeleeStates.WallAttack;
            DamageDealt = DamageOutput;
            Player.ClingToWall = true;
            Player.myRB.constraints = Player.MovementScript.YContraints[1];
            AnimationTimer = (WallAnimReference.length * .5f);
            Player.myAttackBox.SetTrigger("Wall");
            Player.myAnim.SetBool("WallSlash", true);
            //Player.myAnim.SetBool("Dashing", false);
            //Player.myRB.velocity = new Vector2(0, 0);
            Debug.Log("Wall Attack");
        }
    }



    public void SwordsCurrentlyOwned()
    {
        if (!(Game.current.Progress.NewGamePlus))
        {
            if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.Obtained && Game.current.Progress.Sword2 != Player_States.SwordsObtainedStatus.Obtained)
            {
                SwordsOwned = 0;
                Debug.Log("Swords owned = " + SwordsOwned);
            }
            else if (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.Obtained && Game.current.Progress.Sword2 != Player_States.SwordsObtainedStatus.Obtained)
            {
                SwordsOwned = 1;
                Debug.Log("Swords owned = " + SwordsOwned);
            }
            else if (Game.current.Progress.Sword1 != Player_States.SwordsObtainedStatus.Obtained && Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.Obtained)
            {
                SwordsOwned = 1;
                Debug.Log("Swords owned = " + SwordsOwned);
            }
            else if (Game.current.Progress.Sword1 == Player_States.SwordsObtainedStatus.Obtained && Game.current.Progress.Sword2 == Player_States.SwordsObtainedStatus.Obtained)
            {
                SwordsOwned = 2;
                Debug.Log("Swords owned = " + SwordsOwned);
            }
        }
        else
        {
            switch (Game.current.Progress.SwordLevel)
            {
                case Player_States.SwordUpgrade.NotReached:
                    SwordsOwned = 0;
                    Debug.Log("Swords owned = " + SwordsOwned);
                    break;
                case Player_States.SwordUpgrade.DealsDamage:
                    SwordsOwned = 1;
                    Debug.Log("Swords owned = " + SwordsOwned);
                    break;
                case Player_States.SwordUpgrade.DispellsIllusions:
                    SwordsOwned = 2;
                    Debug.Log("Swords owned = " + SwordsOwned);
                    break;

            }
        }
        Debug.Log("Swords owned = " + SwordsOwned);

        switch (SwordsOwned)
        {
            case 0:
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                DamageOutput = 0;
                break;
            case 1:
                Sword1.SetActive(true);
                Sword2.SetActive(false);
                DamageOutput = 1;
                break;
            case 2:
                Sword1.SetActive(true);
                Sword2.SetActive(true);
                DamageOutput = 2;
                break;
        }

        Player.myAnim.SetInteger("Swords", SwordsOwned);
    }
    
    void HandSwords()
    {

        Back1.SetActive(false);
        Back2.SetActive(false);
        switch (SwordsOwned)
        {
            case 0:
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                DamageOutput = 0;
                break;
            case 1:
                Sword1.SetActive(true);
                Sword2.SetActive(false);
                DamageOutput = 1;
                break;
            case 2:
                Sword1.SetActive(true);
                Sword2.SetActive(true);
                DamageOutput = 2;
                break;
        }
    }
    void BackSwords()
    {

        Sword1.SetActive(false);
        Sword2.SetActive(false);
        switch (SwordsOwned)
        {
            case 0:
                Back1.SetActive(false);
                Back2.SetActive(false);
                DamageOutput = 0;
                break;
            case 1:
                Back1.SetActive(true);
                Back2.SetActive(false);
                DamageOutput = 1;
                break;
            case 2:
                Back1.SetActive(true);
                Back2.SetActive(true);
                DamageOutput = 2;
                break;
        }
    }
}

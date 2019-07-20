using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Player_States {

    //Script for referencing all enum values that may be used or referenced in other scripts.

    //Player States
	public enum PlayerStates { Normal, Damaged, MeleeAttacking, Dashing, Crouching, Sliding, ClearMind}
    public enum PlayerHealthStates { Normal, LowHealth, Dead}
    public enum PlayerMeleeStates { None, MeleeChain, TailSwipe, CrouchAttack, DashEnder, AirAttack, WallAttack}
    public enum PlayerDebuffState { Normal, Poisoned, InvertControls}
    public enum DomahdDebuffState { Normal, Incapacitated, Damaged, Dead, Reviving}
    public enum DomahdEnemyLocationIndicator { Behind, Front}
    public enum SwordsObtainedStatus { NotReached,Obtained,Ignored}


    //Universal Events
    public enum SwitchCheckFor { OnTriggerEnter, OnTriggerStay, OnTriggerExit, OnCollisionEnter, OnCollisonStay, OnCollisionExit}
    public enum PerchPointNavigation { NotApplicable, Available}
    public enum PlatformDirection { left,right,up,down}
    public enum ProximityMineMasterTypes { Floating, Tracking }
    public enum LaserTripTypes { Proximity, Pattern}

    public enum LevelSetNumber { Levels1, Levels2, Levels3, FinalBoss, SwordRoom }
    public enum StageSetNumber { Stage1, Stage2, Stage3 }

    //Save File Entries States
    public enum LevelCompletion { NotStarted, Incomplete, Cleared}
    public enum AbilityUnlocks { NotUnlocked, Unlocked, Upgraded}
    public enum ArmourUpgrades { NotUnlocked, Unlocked}
    public enum DifficultySetting { Easy, Normal, Hard}
    public enum OutfitUnlocks { Locked, Unlocked}
    public enum ClearMindUpgrade { NotReached, Level1, Level2}
    public enum SwordUpgrade { NotReached, DealsDamage, DispellsIllusions }
    public enum CharacterSettings { Zahra, ShadowZahra}
    public enum OutfitSettings { Default, Enemy, Shadow, Enlightened, Boss, Basic}

    //Options 
    public enum ControllerInput { Dualshock4, Xbox}
    public enum PlayerLanguageSetting { English}
}

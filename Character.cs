using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character{
    /// <summary>
    /// This script is what goes into the save file for the game.
    /// As the game plays and unlocks are obtained, the information for the unlocks is kept in the current save file of this script.
    /// </summary>
    /// 
    public int FileNumber;

    //public bool Stage1Cleared, Stage2Cleared, Stage3Cleared, FinalBossCleared;

    public Player_States.LevelCompletion[] Level1 = new Player_States.LevelCompletion[3], Level2 = new Player_States.LevelCompletion[3], Level3 = new Player_States.LevelCompletion[3];
    public Player_States.LevelCompletion FinalBoss;

    public int[] Level1Deaths = new int[3], Level2Deaths = new int[3], Level3Deaths = new int[3];
    public int FinalBossDeaths;

    public float[] Level1ClearTime = new float[3], Level2ClearTime = new float[3], Level3ClearTime = new float[3];
    public float FinalBossClearTime, TotalPlaytime;

    public int[] Level1Rating = new int[3], Level2Rating = new int[3], Level3Rating = new int[3];
    public int FinalBossRating, OverallRating;

    public Player_States.SwordsObtainedStatus Sword1, Sword2;
    public bool NewGamePlus = false;
    public bool NewGamePlusStarted = false;

    public string SaveDate;
    public System.DateTime CurrentDate;
    //Public systems
    public bool GameplayPaused;

    public int PlayerDeathCount;

    //cuscenes
    public bool OpeningCutscene, Sword1Cutscene, Sword2Cutscene, PreBossCutscene;

    public Player_States.DifficultySetting CurrentDifficulty;

    public Player_States.OutfitUnlocks AltEnemy, AltShadow, AltEnlightened, AltBoss, AltBasic;
    public bool[] EnemyParts = new bool[3], ShadowParts = new bool[3], EnlightenedParts = new bool[3];
    public bool BossPart, BasicPart;

    public Player_States.ClearMindUpgrade ClearMindLevel;
    public Player_States.SwordUpgrade SwordLevel;

    public Player_States.CharacterSettings SelectedCharacter;
    public Player_States.OutfitSettings CurrentOutfit;

    public Character()
    {
        FileNumber = 1;
        NewGamePlus = false;

        TotalPlaytime = 0;

        //===reset ratings===
        Level1Rating = new int[3];
        Level2Rating = new int[3];
        Level3Rating = new int[3];

        Level1Rating[0] = 0;
        Level1Rating[1] = 0;
        Level1Rating[2] = 0;

        Level2Rating[0] = 0;
        Level2Rating[1] = 0;
        Level2Rating[2] = 0;

        Level3Rating[0] = 0;
        Level3Rating[1] = 0;
        Level3Rating[2] = 0;

        FinalBossRating = 0;
        OverallRating = 0;
        

        //===reset deaths===
        Level1Deaths = new int[3];
        Level2Deaths = new int[3];
        Level3Deaths = new int[3];

        Level1Deaths[0] = 0;
        Level1Deaths[1] = 0;
        Level1Deaths[2] = 0;

        Level2Deaths[0] = 0;
        Level2Deaths[1] = 0;
        Level2Deaths[2] = 0;

        Level3Deaths[0] = 0;
        Level3Deaths[1] = 0;
        Level3Deaths[2] = 0;

        FinalBossDeaths = 0;

        //===reset clear times===
        Level1ClearTime = new float[3];
        Level2ClearTime = new float[3];
        Level3ClearTime = new float[3];

        Level1ClearTime[0] = 9999;
        Level1ClearTime[1] = 9999;
        Level1ClearTime[2] = 9999;

        Level2ClearTime[0] = 9999;
        Level2ClearTime[1] = 9999;
        Level2ClearTime[2] = 9999;

        Level3ClearTime[0] = 9999;
        Level3ClearTime[1] = 9999;
        Level3ClearTime[2] = 9999;

        FinalBossClearTime = 0;

        //===reset level completion===
        Level1 = new Player_States.LevelCompletion[3];
        Level2 = new Player_States.LevelCompletion[3];
        Level3 = new Player_States.LevelCompletion[3];

        Level1[0] = Player_States.LevelCompletion.Incomplete;
        Level1[1] = Player_States.LevelCompletion.NotStarted;
        Level1[2] = Player_States.LevelCompletion.NotStarted;


        Level2[0] = Player_States.LevelCompletion.NotStarted;
        Level2[1] = Player_States.LevelCompletion.NotStarted;
        Level2[2] = Player_States.LevelCompletion.NotStarted;

        Level3[0] = Player_States.LevelCompletion.NotStarted;
        Level3[1] = Player_States.LevelCompletion.NotStarted;
        Level3[2] = Player_States.LevelCompletion.NotStarted;

        FinalBoss = Player_States.LevelCompletion.NotStarted;

        //===reset swords===
        Sword1 = Player_States.SwordsObtainedStatus.NotReached;
        Sword2 = Player_States.SwordsObtainedStatus.NotReached;

        OpeningCutscene = false;
        Sword1Cutscene = false;
        Sword2Cutscene = false;
        PreBossCutscene = false;

        SwordLevel = Player_States.SwordUpgrade.NotReached;
        ClearMindLevel = Player_States.ClearMindUpgrade.NotReached;

        CurrentDifficulty = Player_States.DifficultySetting.Normal;

        //===Costume Settings===
        CurrentOutfit = Player_States.OutfitSettings.Default;
        AltBoss = Player_States.OutfitUnlocks.Locked;
        AltEnemy = Player_States.OutfitUnlocks.Locked;
        AltEnlightened = Player_States.OutfitUnlocks.Locked;
        AltShadow = Player_States.OutfitUnlocks.Locked;
        AltBasic = Player_States.OutfitUnlocks.Locked;

        EnemyParts = new bool[3];
        EnemyParts[0] = false;
        EnemyParts[1] = false;
        EnemyParts[2] = false;

        EnlightenedParts = new bool[3];
        EnlightenedParts[0] = false;
        EnlightenedParts[1] = false;
        EnlightenedParts[2] = false;

        ShadowParts= new bool[3];
        ShadowParts[0] = false;
        ShadowParts[1] = false;
        ShadowParts[2] = false;

        BossPart = false;
        BasicPart = false;

        NewGamePlusStarted = false;

        SaveDate = System.DateTime.Now.ToString();
        //SaveExists = false;
        //CurrentDate = System.DateTime.Now;
        PlayerDeathCount = 0;
        OverallRating = 0;
        SelectedCharacter = Player_States.CharacterSettings.Zahra;
        GameplayPaused = false;
    }

}

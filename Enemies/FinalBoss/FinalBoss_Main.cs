using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalBoss_Main : MonoBehaviour {
    public static FinalBoss_Main Instance { get; set; }
    public enum BossStates { Docile, Waking, Idle, DashAttack, Summonning, ScreenNuke, Damaged, Victory, Defeat, Dead}
    public enum SummonTypes { None, SpikeFloor, SpikeBlock, ProxyLaser, ProxyMine, ChaserMine, Ghost, FakeBlock, FakeMine}
    public enum BossMechType { NoSword, WithSwords}

    EnemyHealth myHealth;
    Rigidbody2D myRB;
    public Collider2D myCollider;

    public int BeginningSequence = 0;
    Vector3 startingPos;

    public int Phase2StartHealth;
    public int Phase3StartHealth;

    public float Phase2Timer = 120;
    public float Phase3Timer = 60;

    [Header("Main")]
    public BossMechType BossMechanicsType = BossMechType.NoSword;
    public BossStates BossState = BossStates.Docile;
    public SummonTypes SummonType = SummonTypes.None;
    public int CurrentPhase = 0;
    public int PhaseTransition = 0;
    public Animator myAnim;
    public float BossSurviveTime = 180;
    public float bossSurvivalTimer;
    public float Speed = 6f;
    public float BossStatePhaseTimer = 0;
    public int DamageOutput = 1;
    public float RotationFacingLeft = 0;
    public float RotationFacingRight = 180;
    public GameObject HealthbarObj;
    public Image bossHealthBar;
    public Slider bossHealthSlider;
    //public Sprite[] HealthbarSprites;
    public Transform TouchingFloorClone;
    public GameObject bossHourglass;

    [Header("Object Rererence")]
    public ParticleSystem BossSmoke;
    public Animator MeleeAnim;
    public GameObject PlayerObj;
    public Player_Main Plyr;
    public GameObject ModelParent;

    [Header("Script Reference")]
    public FinalBoss_Melee MeleeScript;
    public PreFinalBossCutscene CutsceneScript;
    

    [Header("Idle")]
    public int IdleSequence = 0;
    public float IdleSequenceTimer;
    public Transform[] ResetLocations;
    public Transform NearestIdlePoint;

    [Header("Dash Attack")]
    public int DashAttackSequence = 0;
    public float DashAttackDuration = 1;
    public float DashAttackAnimTimer;
    public Vector3 TargetLocationReference;

    [Header("Damage")]
    public float damageTimer;
    public float iFramesMax = 3;
    public float damageInvulnTimer;

    [Header("Summoning")]
    public GameObject[] SummonWarning;
    public int SummoningSequence = 0;
    public float SummoningWaitTimer = 0;
    public bool ProduceFakeTrap;
    public GameObject[] SpikeFloor;
    public bool[] SpikeFloorActive;
    public GameObject[] SpikeBlock;
    public bool[] SpikeBlockActive;
    public GameObject[] ProxyLaser;
    public bool[] LaserActive;
    public GameObject[] ProxyMine;
    public GameObject[] ProxyActive;
    public GameObject[] ChaserMine;
    public GameObject[] ChaserActive;
    public FinalBossGhostMaster[] GhostScripts;
    public GameObject[] FakeBlock;
    public GameObject[] FakeMine;
    public Transform NextNearestPoint;

    [Header("Spike Summoning")]
    int SpikeSequence = 0;
    public Transform[] SpikeLocationCheck;
    public Transform NearestSpikeSpawn;
    GameObject SpikeToSpawn;

    [Header("Ghosts Summon")]
    public float GhostSummonExistTimer = 15;

    [Header("Summoning Prefabs")]
    public GameObject ProxyMinePrefab;
    public GameObject ChaserMinePrefab;
    public GameObject GhostSpawnPrefab;

    [Header("ScreenNuke")]
    public int ScreenNukeSequence = 0;

    [Header("Animation Timing References")]
    public AnimationClip DocileAnim;
    public AnimationClip WakingClip, IdleClip, DashAttackClip, SummonClip, Summon2Clip, DamageClip;

    Sequence myDoMoveSequence;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        foreach (GameObject gos in SummonWarning)
        {
            gos.SetActive(false);
        }
        startingPos = transform.position;
        myHealth = GetComponent<EnemyHealth>();
        //Phase2StartHealth = (myHealth.FullHealth * (2 / 3));
        //Phase3StartHealth = (myHealth.FullHealth * (1 / 3));
        myCollider = GetComponent<Collider2D>();
        bossHourglass.SetActive(false);
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

        switch (Game.current.Progress.CurrentDifficulty)
        {
            case Player_States.DifficultySetting.Easy:

                myHealth.FullHealth = 12;
                Phase2StartHealth = 6;
                Phase3StartHealth = 2;

                BossSurviveTime = 100;
                Phase2Timer = 50;
                Phase3Timer = 15;

                break;
            case Player_States.DifficultySetting.Normal:

                myHealth.FullHealth = 18;
                Phase2StartHealth = 12;
                Phase3StartHealth = 6;

                BossSurviveTime = 120;
                Phase2Timer = 80;
                Phase3Timer = 40;

                break;
            case Player_States.DifficultySetting.Hard:

                myHealth.FullHealth = 24;
                Phase2StartHealth = 20;
                Phase3StartHealth = 15;

                BossSurviveTime = 160;
                Phase2Timer = 130;
                Phase3Timer = 99;

                break;
        }


        MeleeScript.FinalBoss = this;
        HealthbarObj.SetActive(false);
        BeginningSequence = 0;
        ResetArena();
	}
	
	// Update is called once per frame
	void Update () {
        damageTimer -= Time.deltaTime;
        damageInvulnTimer -= Time.deltaTime;
        BossStatePhaseTimer -= Time.deltaTime;
        switch (BeginningSequence)
        {
            case 1:

                HealthbarObj.SetActive(false);
                bossHourglass.SetActive(false);
                BossSmoke.Stop();
                BeginningSequence = 2;
                CurrentPhase = 1;
                BossStatePhaseTimer = WakingClip.length;
                myAnim.SetBool("Waking", true);
                break;
            case 2:
                BossState = BossStates.Waking;
                if (BossStatePhaseTimer <= 0)
                {
                    myAnim.SetBool("Begin", true);
                    BeginningSequence = 3;
                }
                break;
            case 3:
                PlayerObj = GameObject.FindGameObjectWithTag("Player");
                Plyr = PlayerObj.GetComponent<Player_Main>();
                

                if (Plyr.MeleeScript.SwordsOwned > 0)
                {
                    BossMechanicsType = BossMechType.WithSwords;
                    bossHealthSlider.value = 1;
                    bossHealthSlider.maxValue = myHealth.FullHealth;
                    bossHealthSlider.value = bossHealthSlider.maxValue;
                    Debug.Log("Fight!!!");
                }
                else
                {
                    BossMechanicsType = BossMechType.NoSword;
                    bossSurvivalTimer = BossSurviveTime;

                    bossHealthSlider.value = 1;
                    bossHealthSlider.maxValue = BossSurviveTime;
                    bossHealthSlider.value = bossHealthSlider.maxValue;
                    bossHourglass.SetActive(true);
                    Debug.Log("Survive");
                }

                BossState = BossStates.Idle;
                myHealth.Health = myHealth.FullHealth;
                BeginningSequence = 4;
                break;
            case 4:

                if (BossMechanicsType == BossMechType.NoSword) { bossSurvivalTimer -= Time.deltaTime; }
                BossHealthDisplay();
                PhaseCheck();
                if (myHealth.Health >0)
                {
                    if (myHealth.Damaged)
                    {
                        Debug.Log("Damage hit");
                        myAnim.SetBool("Hurt", true);
                        myAnim.SetTrigger("HurtTrigg");
                        BossState = BossStates.Damaged;
                        myHealth.Health -= myHealth.damageTaken;
                        myHealth.Damaged = false;
                        damageTimer = DamageClip.length;
                        myHealth.damageEnabled = false;
                        BossSmoke.Play();
                        damageInvulnTimer = iFramesMax;
                        //damageInvulnTimer = (dama)
                        myDoMoveSequence.Kill();
                        Debug.Log("Is invuln");
                    }
                    if(damageInvulnTimer > 0)
                    {

                        //Debug.Log("Is invuln");
                    }

                    if (damageInvulnTimer <= 0 && BossState == BossStates.Idle)
                    {
                        damageInvulnTimer = 0;
                        BossSmoke.Stop();
                        myHealth.damageEnabled = true;
                        //Debug.Log("Not Invuln now");
                    }


                    if (BossState != BossStates.Damaged)
                    {
                        switch (CurrentPhase)
                        {
                            case 1:
                                Phase1();
                                break;
                            case 2:
                                Phase2();
                                break;
                            case 3:
                                Phase3();
                                break;
                        }

                    }
                    else
                    {
                        DamageState();
                    }
                    BossHealthDisplay();
                }
                else
                {
                    BeginningSequence = 5;
                    Debug.Log("Dead");
                }
                if (bossSurvivalTimer <=0 && BossMechanicsType == BossMechType.NoSword)
                {
                    BeginningSequence = 5;
                    Debug.Log("Time Out");
                }
                break;
            case 5:
                //temporary
                Game.current.Progress.GameplayPaused = true;
                BossState = BossStates.Defeat;
                HealthbarObj.SetActive(false);
                bossHourglass.SetActive(false);
                ResetArena();
                myHealth.Health = 0;
                CutsceneScript.EventsSequence = 6;
                BeginningSequence = 6;
                MeleeScript.KillMeleeNow = true;
                BossState = BossStates.Defeat;
                //SceneManager.LoadScene("Win");
                break;
            case 6:
                BossSmoke.Stop();
                myAnim.ResetTrigger("HurtTrigg");
                myAnim.SetInteger("Dash", 0);
                myAnim.SetInteger("Defeat", 0);
                myAnim.SetInteger("Summoning", 0);
                MeleeAnim.SetTrigger("ResetMelee");

                myDoMoveSequence.Kill();
                myAnim.SetInteger("Defeat", 1);
                BeginningSequence = 7;
                //transform.DOMove(new Vector3(transform.position.x, TouchingFloorClone.position.y, transform.position.z), 3);
                BossStatePhaseTimer = .0001f;
                
                break;
            case 7:
                BossStatePhaseTimer -= Time.deltaTime;

                if (BossStatePhaseTimer <= 0)
                {
                    CutsceneScript.GlyphExitLocation.transform.position = transform.position;
                    myAnim.SetInteger("Defeat", 2);
                    BeginningSequence = 8;
                    CutsceneScript.EventsSequence = 7;
                }

                break;
            case 8:
                CutsceneScript.GlyphExitLocation.transform.position = transform.position;
                break;
        }
	}

    public void PhaseCheck()
    {
        if (BossMechanicsType == BossMechType.WithSwords)
        {
            if (myHealth.Health > Phase2StartHealth)
            {
                CurrentPhase = 1;
            }
            else if (myHealth.Health <= Phase2StartHealth && myHealth.Health > Phase3StartHealth)
            {
                CurrentPhase = 2;
            }
            else if (myHealth.Health <= Phase3StartHealth && myHealth.Health > 0)
            {
                CurrentPhase = 3;
            }
        }
        else
        {
            if (bossSurvivalTimer > Phase2Timer)
            {
                CurrentPhase = 1;
            }
            else if (bossSurvivalTimer <= Phase2Timer && bossSurvivalTimer > Phase3Timer)
            {
                CurrentPhase = 2;
            }
            else if (bossSurvivalTimer <= Phase3Timer && bossSurvivalTimer > 0)
            {
                CurrentPhase = 3;
            }
        }
    }
    

    public void ResetArena()
    {
        BossSmoke.Play();

        HealthbarObj.SetActive(false);
        ScreenNukeSequence = 0;
        BossStatePhaseTimer = 0;
        CurrentPhase = 0;
        DashAttackSequence = 0;
        DashAttackAnimTimer = 0;
        BeginningSequence = 0;
        SummoningSequence = 0;
        SummoningWaitTimer = 0;
        IdleSequence = 0;
        IdleSequenceTimer = 0;
        PlayerObj = null;
        myDoMoveSequence.Kill();
        myHealth.Health = myHealth.FullHealth;

        foreach (GameObject gos in SummonWarning)
        {
            gos.SetActive(false);
        }

        foreach (GameObject gos in SpikeBlock)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in ProxyLaser)
        {
            gos.SetActive(false);
        }

        foreach (GameObject gos in SpikeFloor)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in FakeBlock)
        {
            gos.SetActive(false);
        }
        foreach (GameObject gos in FakeMine)
        {
            gos.SetActive(false);
        }

        SpikeBlockActive[0] = false;
        SpikeBlockActive[1] = false;

        LaserActive[0] = false;
        LaserActive[1] = false;

        foreach(GameObject gos in ProxyActive)
        {
            if (gos != null)
            {
                Destroy(gos);
            }
            
        }
        foreach(GameObject gos in ChaserActive)
        {

            if (gos != null)
            {
                Destroy(gos);
            }
        }

        foreach(FinalBossGhostMaster gos in GhostScripts)
        {
            gos.SpawnerActive = false;
        }

        myAnim.SetBool("Begin", false);
        myAnim.SetBool("Hurt", false);
        myAnim.SetBool("Waking", false);
        myAnim.SetBool("Victory", false);
        myAnim.SetBool("Dead", false);
        myAnim.ResetTrigger("HurtTrigg");
        myAnim.SetInteger("Dash", 0);
        myAnim.SetInteger("Defeat", 0);
        myAnim.SetInteger("Summoning", 0);
        MeleeAnim.SetTrigger("ResetMelee");

        myAnim.SetTrigger("ResetAll");
        transform.position = startingPos;
        myHealth.Health = myHealth.FullHealth;
    }
    public void SetupBoss()
    {
        ResetArena();
        BeginningSequence = 1;
        myHealth.Health = myHealth.FullHealth;
        HealthbarObj.SetActive(true);
        BossHealthDisplay();
    }

    void BossHealthDisplay()
    {
        
        if (BossMechanicsType == BossMechType.WithSwords)
        {
            int healthCount = myHealth.Health;
            if (healthCount <= 0)
            {
                healthCount = 0;
            }
            HealthbarObj.SetActive(true);
            bossHealthSlider.value = myHealth.Health;
            //bossHealthBar.sprite = HealthbarSprites[healthCount];
        }
        else if (BossMechanicsType == BossMechType.NoSword)
        {
            int healthCount = myHealth.Health;
            if (bossSurvivalTimer <= 0)
            {
                myHealth.Health = 0;
            }
            bossHealthSlider.value = bossSurvivalTimer;

            HealthbarObj.SetActive(true);
            //bossHealthBar.sprite = HealthbarSprites[healthCount];
        }

    }


    void DamageState()
    {
        if (damageTimer <= 0)
        {
            damageTimer = 0;
            ScreenNukeSequence = 0;
            DashAttackSequence = 0;
            DashAttackAnimTimer = 0;
            SummoningSequence = 0;
            SummoningWaitTimer = 0;
            IdleSequence = 0;
            IdleSequenceTimer = 0;
            myAnim.SetInteger("Summoning", 0);
            MeleeAnim.SetTrigger("ResetMelee");
            myAnim.SetInteger("Dash", 0);
            myAnim.SetBool("Hurt", false);
            myAnim.ResetTrigger("HurtTrigg");
            BossState = BossStates.Idle;
        }
    }

    void DecideOnNextPhase1()
    {
        IdleSequence = 0;
        int RandomChecker = 0;
        RandomChecker = Random.Range(1, 100);

        if (RandomChecker <50 || RandomChecker >=50)
        {
            BossState = BossStates.DashAttack;
        }
    }

    void DecideOnNextPhase2()
    {
        IdleSequence = 0;
        int RandomChecker = 0;
        RandomChecker = Random.Range(1, 100);

        if (RandomChecker < 50)
        {
            BossState = BossStates.DashAttack;
        }
        else
        {
            BossState = BossStates.Summonning;
        }
    }

    void DecideOnNextPhase3()
    {
        IdleSequence = 0;
        int RandomChecker = 0;
        RandomChecker = Random.Range(1, 100);

        if (RandomChecker < 50)
        {
            BossState = BossStates.DashAttack;
        }
        else
        {
            BossState = BossStates.Summonning;
        }
    }

    void Phase1()
    {
        switch (BossState)
        {
            case BossStates.Idle:
                IdleFunction();
                break;
            case BossStates.DashAttack:
                DashAttackFunction();
                break;


        }
    }

    void Phase2()
    {
        switch (BossState)
        {
            case BossStates.Idle:
                IdleFunction();
                break;
            case BossStates.DashAttack:
                DashAttackFunction();
                break;
            case BossStates.Summonning:
                SummoningFunction();
                break;


        }
    }

    void Phase3()
    {
        switch (BossState)
        {
            case BossStates.Idle:
                IdleFunction();
                break;
            case BossStates.DashAttack:
                DashAttackFunction();
                break;
            case BossStates.Summonning:
                SummoningFunction();
                break;


        }
    }

    void IdleFunction()
    {
        IdleSequenceTimer -= Time.deltaTime;
        switch (IdleSequence)
        {
            case 0:
                FindNearestIdlePoint();
                if (transform.position == NearestIdlePoint.position)
                {
                    Debug.Log("Location already");
                    IdleSequence = 2;
                }
                else
                {
                    Debug.Log("To location go!!!");
                    IdleSequence = 1;
                    IdleSequenceTimer = Speed;
                    //Transform myTrans = transform;
                    myDoMoveSequence = DOTween.Sequence();
                    myDoMoveSequence.Append(transform.DOMove(NearestIdlePoint.position, Speed).SetEase(Ease.OutSine));
                    myDoMoveSequence.Play();
                }

                break;
            case 1:
                if (IdleSequenceTimer <= 0)
                {
                    Debug.Log("Location reached");
                    IdleSequence = 2;
                    BossStatePhaseTimer = .1f;
                }
                break;
            case 2:
                if (BossStatePhaseTimer <= 0)
                {
                    switch (CurrentPhase)
                    {
                        case 1:
                            DecideOnNextPhase1();
                            break;
                        case 2:
                            DecideOnNextPhase2();
                            break;
                        case 3:
                            DecideOnNextPhase3();
                            break;
                    }
                    

                }

                break;
        }
    }

    public Transform FindNearestIdlePoint()
    {
        NearestIdlePoint = null;
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position; //Boss's position this frame.
        foreach (Transform newPos in ResetLocations)
        {
            Vector3 diff = newPos.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance) //Checks if enemy is closer than previous enemies gone through in the array.
            {
                NearestIdlePoint = newPos;
                distance = curDist;
            }
        }


        return NearestIdlePoint;
    }

    public Transform FindNextNearestIdlePoint()
    {
        NearestIdlePoint = null;
        float distance = Mathf.Infinity;
        Vector3 pos = transform.position; //Boss's position this frame.
        foreach (Transform newPos in ResetLocations)
        {
            Vector3 diff = newPos.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance && newPos != NearestIdlePoint) //Checks if enemy is closer than previous enemies gone through in the array.
            {
                NextNearestPoint = newPos;
                distance = curDist;
            }
        }


        return NextNearestPoint;
    }

    void DashAttackFunction()
    {
        DashAttackAnimTimer -= Time.deltaTime;
        switch (DashAttackSequence)
        {
            case 0:
                TargetLocationReference = PlayerObj.transform.position;
                DashAttackSequence = 1;
                if (TargetLocationReference.x < transform.position.x)
                {
                    ModelParent.transform.rotation = Quaternion.Euler(0, RotationFacingLeft, 0);
                }
                else
                {
                    ModelParent.transform.rotation = Quaternion.Euler(0, -RotationFacingRight, 0);
                }
                DashAttackAnimTimer = DashAttackDuration;
                myDoMoveSequence = DOTween.Sequence();
                myDoMoveSequence.Append(transform.DOMove(TargetLocationReference, DashAttackDuration));
                myDoMoveSequence.Play();
                
                myAnim.SetInteger("Dash", 1);
                break;
            case 1:
                if (DashAttackAnimTimer <= 0)
                {
                    DashAttackSequence = 2;
                }
                break;
            case 2:
                myAnim.SetInteger("Dash", 2);
                MeleeAnim.SetTrigger("Melee");
                DashAttackAnimTimer = DashAttackClip.length;

                DashAttackSequence = 3;

                break;
            case 3:
                if (DashAttackAnimTimer <= 0)
                {
                    MeleeAnim.SetTrigger("ResetMelee");
                    myAnim.SetInteger("Dash", 0);
                    DashAttackSequence = 0;
                    BossState = BossStates.Idle;
                }

                break;
        }
    }

    void SummoningFunction()
    {
        SummoningWaitTimer -= Time.deltaTime;
        switch (SummoningSequence)
        {
            case 0:
                SummonFakeOrNotCheck();
                FindNextNearestIdlePoint();
                //myAnim.SetInteger("Dash", 1);
                myDoMoveSequence = DOTween.Sequence();
                myDoMoveSequence.Append(transform.DOMove(NextNearestPoint.position, (Speed/2)).SetEase(Ease.OutSine));
                myDoMoveSequence.Play();
                SummoningWaitTimer = (Speed / 2);

                SummoningSequence = 1;
                break;
            case 1:
                if (SummoningWaitTimer <= 0)
                {
                    ResetSummonSequences();
                    //myAnim.SetInteger("Dash", 0);
                    {
                        switch (ProduceFakeTrap)
                        {
                            case true:
                                SummonFakeRandomizerCheckPhase2();

                                break;
                            case false:
                                switch (CurrentPhase)
                                {
                                    default:

                                        SummonRandomizerCheckPhase2();
                                        break;
                                    case 2:

                                        SummonRandomizerCheckPhase2();
                                        break;

                                    case 3:

                                        SummonRandomizerCheckPhase3();
                                        break;
                                }
                                

                                break;
                        }
                    }
                    Debug.Log("Spawning a " + SummonType.ToString()+ " next.");
                    TargetLocationReference = PlayerObj.transform.position;

                    if (TargetLocationReference.x < transform.position.x)
                    {
                        ModelParent.transform.rotation = Quaternion.Euler(0, RotationFacingLeft, 0);
                    }
                    else
                    {
                        ModelParent.transform.rotation = Quaternion.Euler(0, -RotationFacingRight, 0);
                    }

                    SummoningSequence = 2;
                }
                break;
            case 2:
                myAnim.SetInteger("Summoning", 1);
                foreach(GameObject gos in SummonWarning)
                {
                    gos.SetActive(true);
                }
                SummoningWaitTimer = SummonClip.length;

                SummoningSequence = 3;
                break;
            case 3:
                if (SummoningWaitTimer <= 0)
                {
                    foreach (GameObject gos in SummonWarning)
                    {
                        gos.SetActive(false);
                    }
                    switch (SummonType)
                    {
                        case SummonTypes.FakeBlock:
                            SummonFakeBlock();
                            break;
                        case SummonTypes.FakeMine:
                            SummonFakeMine();
                            break;
                        case SummonTypes.SpikeFloor:
                            SummonSpikes();
                            break;
                        case SummonTypes.Ghost:
                            SummonGhost();
                            break;
                        case SummonTypes.ProxyMine:
                            SummonProxyMine();
                            break;
                        case SummonTypes.SpikeBlock:
                            SummonBlock();
                            break;
                        case SummonTypes.ChaserMine:
                            SummonChaser();
                            break;
                        case SummonTypes.ProxyLaser:
                            SummonLaser();
                            break;
                    }


                    
                }
                break;

            case 4:

                myAnim.SetInteger("Summoning", 2);
                SummoningWaitTimer = Summon2Clip.length;
                SummoningSequence = 5;

                break;
            case 5:
                if (SummoningWaitTimer <= 0)
                {

                    myAnim.SetInteger("Summoning", 0);
                    SummoningSequence = 0;
                    BossState = BossStates.Idle;
                }
                break;
        }
    }

    void ResetSummonSequences()
    {
        SpikeSequence = 0;
    }

    void SummonFakeOrNotCheck()
    {
        int summonRandomChecker;
        summonRandomChecker = Random.Range(1, 100);
        if (summonRandomChecker < 33)
        {
            ProduceFakeTrap = true;
        }
        else
        {
            ProduceFakeTrap = false;
        }
    }


    void SummonRandomizerCheckPhase2()
    {
        int SummonRandomCheck;
        SummonRandomCheck = Random.Range(0, 100);
        if (SummonRandomCheck < 33)
        {
            SummonType = SummonTypes.SpikeFloor;
        }
        else if (SummonRandomCheck >= 33 && SummonRandomCheck < 66)
        {
            SummonType = SummonTypes.SpikeBlock;
        }
        else if (SummonRandomCheck >= 66)
        {
            SummonType = SummonTypes.ProxyLaser;
        }
    }


    void SummonRandomizerCheckPhase3()
    {
        int SummonRandomCheck;
        SummonRandomCheck = Random.Range(0, 100);
        if (SummonRandomCheck < 15)
        {
            SummonType = SummonTypes.SpikeFloor;
        }
        else if (SummonRandomCheck >= 15 && SummonRandomCheck < 35)
        {
            SummonType = SummonTypes.ChaserMine;
        }
        else if (SummonRandomCheck >= 35 && SummonRandomCheck < 50)
        {
            SummonType = SummonTypes.ProxyMine;
        }
        else if (SummonRandomCheck >= 50 && SummonRandomCheck < 65)
        {
            SummonType = SummonTypes.SpikeBlock;
        }
        else if (SummonRandomCheck >= 65 && SummonRandomCheck < 85)
        {
            
            SummonType = SummonTypes.Ghost;
        }
        else if (SummonRandomCheck >= 85)
        {
            SummonType = SummonTypes.ProxyLaser;
        }
    }

    void SummonFakeRandomizerCheckPhase2()
    {
        int SummonRandomCheck;
        SummonRandomCheck = Random.Range(0, 100);
        if (SummonRandomCheck < 50)
        {
            SummonType = SummonTypes.FakeBlock;
        }
        else
        {
            SummonType = SummonTypes.FakeMine;
        }
    }


    void SummonSpikes()
    {
        int randomSpike;
        randomSpike = Random.Range(0, 4);
        switch (randomSpike)
        {
            case 0:
                SpikeFloor[0].SetActive(true);
                break;
            case 1:
                SpikeFloor[1].SetActive(true);
                break;
            case 2:
                SpikeFloor[2].SetActive(true);
                break;
            case 3:
                SpikeFloor[3].SetActive(true);
                break;

        }

        SummoningSequence = 4;
    }

    public Transform FindNearestSpikeToPlayer()
    {
        NearestIdlePoint = null;
        float distance = Mathf.Infinity;
        Vector3 pos = PlayerObj.transform.position; //Boss's position this frame.
        foreach (Transform newPos in SpikeLocationCheck)
        {
            Vector3 diff = newPos.transform.position - pos;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance) //Checks if enemy is closer than previous enemies gone through in the array.
            {
                NearestSpikeSpawn = newPos;
                distance = curDist;
            }
        }


        return NearestSpikeSpawn;
    }

    void SummonBlock()
    {
        int randomBlock;
        randomBlock = Random.Range(1, 100);
        if (randomBlock <= 50)
        {
            if (!SpikeBlockActive[0])
            {
                SpikeBlock[0].SetActive(true);
                SpikeBlockActive[0] = true;

            }
            else { }
        }
        else
        {
            if (!SpikeBlockActive[1])
            {
                SpikeBlock[1].SetActive(true);
                SpikeBlockActive[1] = true;
                
            }
            else
            {
                //SummonGhost();
            }
        }

        SummoningSequence = 4;
    }
    void SummonProxyMine()
    { 
        int randomMine;
        randomMine = Random.Range(0, 3);
        Vector3 proxyPoint;
        if (ProxyActive[randomMine] == null)
        {
            switch (randomMine)
            {
                default:
                    proxyPoint = ProxyMine[0].transform.position;
                    break;
                case 0:

                    proxyPoint = ProxyMine[0].transform.position;
                    break;
                case 1:

                    proxyPoint = ProxyMine[1].transform.position;
                    break;
                case 2:

                    proxyPoint = ProxyMine[2].transform.position;
                    break;

            }
            GameObject newMine = GameObject.Instantiate(ProxyMinePrefab, proxyPoint, new Quaternion(0, 0, 0, 0));
            ProxyActive[randomMine] = newMine;
        }
        else
        {
            Debug.Log("Mine spawn issue. Looking for a different position to spawn mine.");
            bool checkMineAgain = false;
            switch (checkMineAgain)
            {
                case false:
                    randomMine = Random.Range(0, 3);
                    if (ProxyActive[randomMine] == null)
                    {
                        switch (randomMine)
                        {
                            default:
                                proxyPoint = ProxyMine[0].transform.position;
                                break;
                            case 0:

                                proxyPoint = ProxyMine[0].transform.position;
                                break;
                            case 1:

                                proxyPoint = ProxyMine[1].transform.position;
                                break;
                            case 2:

                                proxyPoint = ProxyMine[2].transform.position;
                                break;

                        }
                        GameObject newMine = GameObject.Instantiate(ProxyMinePrefab, proxyPoint, new Quaternion(0, 0, 0, 0));
                        ProxyActive[randomMine] = newMine;
                    }
                    else
                    {
                        Debug.Log("Double mine spawn issue. Not spawning any mines this frame.");
                        SummonFakeMine();
                    }
                    break;
            }
        }

        SummoningSequence = 4;

    }
    void SummonLaser()
    {
        int randomBlock;
        randomBlock = Random.Range(1, 100);
        if (randomBlock <= 50)
        {
            if (!LaserActive[0])
            {
                ProxyLaser[0].SetActive(true);
                LaserActive[0] = true;
            }
            else
            {
                if (!LaserActive[1])
                {
                    ProxyLaser[1].SetActive(true);
                    LaserActive[1] = true;
                }
                else
                {
                    Debug.Log("Both lasers active, not activating any this turn");
                }
            }
        }
        else
        {
            if (!LaserActive[1])
            {
                ProxyLaser[1].SetActive(true);
                LaserActive[1] = true;
            }
            else
            {
                if (!LaserActive[0])
                {
                    ProxyLaser[0].SetActive(true);
                    LaserActive[0] = true;
                }
                else
                {
                    Debug.Log("Both lasers active, not activating any this turn");
                }
            }
        }

        SummoningSequence = 4;
    }

    void SummonGhost()
    {
        int randomGhost;
        randomGhost = Random.Range(0, 10);

        switch (randomGhost)
        {
            case 0:
                GhostScripts[0].SpawnerActive = true;
                GhostScripts[0].disableTimer = GhostSummonExistTimer;
                break;
            case 1:
                GhostScripts[1].SpawnerActive = true;
                GhostScripts[1].disableTimer = GhostSummonExistTimer;
                break;
            case 2:
                GhostScripts[2].SpawnerActive = true;
                GhostScripts[2].disableTimer = GhostSummonExistTimer;
                break;
            case 3:
                GhostScripts[3].SpawnerActive = true;
                GhostScripts[3].disableTimer = GhostSummonExistTimer;
                break;
            case 4:
                GhostScripts[4].SpawnerActive = true;
                GhostScripts[4].disableTimer = GhostSummonExistTimer;
                break;
            case 5:
                GhostScripts[5].SpawnerActive = true;
                GhostScripts[5].disableTimer = GhostSummonExistTimer;
                break;
            case 6:
                GhostScripts[6].SpawnerActive = true;
                GhostScripts[6].disableTimer = GhostSummonExistTimer;
                break;
            case 7:
                GhostScripts[7].SpawnerActive = true;
                GhostScripts[7].disableTimer = GhostSummonExistTimer;
                break;
            case 8:
                GhostScripts[8].SpawnerActive = true;
                GhostScripts[8].disableTimer = GhostSummonExistTimer;
                break;
            case 9:
                GhostScripts[9].SpawnerActive = true;
                GhostScripts[9].disableTimer = GhostSummonExistTimer;
                break;
        }


        SummoningSequence = 4;
    }

    void SummonChaser()
    {

        int randomMine;
        randomMine = Random.Range(0, 2);
        Vector3 proxyPoint;
        if (ChaserActive[randomMine] == null)
        {
            switch (randomMine)
            {
                default:
                    proxyPoint = ProxyMine[0].transform.position;
                    break;
                case 0:

                    proxyPoint = ProxyMine[0].transform.position;
                    break;
                case 1:

                    proxyPoint = ProxyMine[1].transform.position;
                    break;
                case 2:

                    proxyPoint = ProxyMine[2].transform.position;
                    break;

            }


            GameObject newMine = GameObject.Instantiate(ChaserMinePrefab, proxyPoint, new Quaternion(0, 0, 0, 0));
            ChaserActive[randomMine] = newMine;
        }
        else
        {
            SummonChaser();
            Debug.Log("Issue spawning chasing mine. Selected spot already taken. No mine will spawn this frame");
        }
    SummoningSequence = 4;
    }

    void SummonFakeMine()
    {
        int randomMine;
        randomMine = Random.Range(1, 7);
        switch (randomMine)
        {
            default:
                FakeMine[0].SetActive(true);
                break;
            case 1:
                FakeMine[0].SetActive(true);
                break;
            case 2:
                FakeMine[1].SetActive(true);
                //SummonGhost();
                break;
            case 3:
                FakeMine[2].SetActive(true);
                //SummonGhost();
                break;
            case 4:
                FakeMine[3].SetActive(true);
                break;
            case 5:
                FakeMine[4].SetActive(true);
                //SummonGhost();
                break;
            case 6:
                FakeMine[5].SetActive(true);
                break;
        }
        SummoningSequence = 4;
    }

    void SummonFakeBlock()
    {
        int randomBlock;
        randomBlock = Random.Range(1, 100);
        if (randomBlock <= 50)
        {
            if (!SpikeBlockActive[0])
            {
                FakeBlock[0].SetActive(true);
                SpikeBlockActive[0] = true;
            }
            else
            {
                SummonSpikes();
            }
        }
        else
        {
            if (!SpikeBlockActive[1])
            {
                FakeBlock[1].SetActive(true);
                SpikeBlockActive[1] = true;
            }
            else
            {

                SummonSpikes();
            }
        }

        SummoningSequence = 4;
    }
    
}

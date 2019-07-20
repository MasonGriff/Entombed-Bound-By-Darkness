using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossGhostMaster : MonoBehaviour {

    public bool SpawnerActive;

    public GameObject GhostChild;
    public GameObject GhostPrefab;

    public FinalBossGhost GhostScript;

    public float RespawnTimerReset = 10f;
    public float RespawnTimer;

    public ParticleSystem GhostParticles;

    public float disableTimer = 0;

    [Header("Override")]
    public bool OverrideGhostZahraSpeed = true;
    public float GhostZahraSpeed = 2;
    //public GameObject PlayerObj;
    int spawnLimit = 0;
    int SpawnLimitMax = 1;

    // Use this for initialization
    void Start()
    {
        GhostZahraSpeed = 3;
        disableTimer = 0;
        if (Game.current.Progress.CurrentDifficulty == Player_States.DifficultySetting.Hard)
        {
            SpawnLimitMax = 3;
            GhostZahraSpeed = 2.5f;
        }
        SpawnerActive = false;

        GhostParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        disableTimer -= Time.deltaTime;
        RespawnTimer -= Time.deltaTime;
        if (disableTimer <= 0)
        {
            SpawnerActive = false;
        }
    }

    private void FixedUpdate()
    {
        if (SpawnerActive)
        {
            GhostParticles.Play();
            if (GhostChild == null)
            {
                if (RespawnTimer <= 0)
                {
                    spawnLimit++;
                    SpawnNewGhost();
                }
            }
            else
            {
                if (GhostScript.TargetEngage && LevelGM.Instance.playDeath)
                {
                    if (spawnLimit >= SpawnLimitMax)
                    {
                        spawnLimit = 0;
                        SpawnerActive = false;

                    }
                    Destroy(GhostChild);

                }
            }
        }
        else
        {
            GhostParticles.Stop();
        }
    }

    void SpawnNewGhost()
    {
        RespawnTimer = 0;
        GhostChild = GameObject.Instantiate(GhostPrefab, transform);
        GhostChild.transform.position = transform.position;
        GhostScript = GhostChild.GetComponent<FinalBossGhost>();
        Debug.Log("Ghost Spawned");
        TellGhostToGo(FinalBoss_Main.Instance.PlayerObj);
    }

    void TellGhostToGo(GameObject myTrigg)
    {
        if (GhostChild != null && GhostScript != null)
        {
            if (!GhostScript.TargetEngage)
            {
                Debug.Log("Player Entered");
                GhostScript.TargetIsSighted(myTrigg.transform.position,myTrigg.transform);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player" && !LevelGM.Instance.playDeath)
        {

            TellGhostToGo(myTrigg.gameObject);
        }
    }
}

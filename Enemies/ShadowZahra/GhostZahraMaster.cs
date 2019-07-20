using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostZahraMaster : MonoBehaviour {

    public GameObject GhostChild;
    public GameObject GhostPrefab;

    public GhostZahra GhostScript;

    public float RespawnTimerReset = 10f;
    public float RespawnTimer;

    [Header("Override")]
    public bool OverrideGhostZahraSpeed = true;
    public float GhostZahraSpeed = 2;
    //public GameObject PlayerObj;

	// Use this for initialization
	void Start () {
        if (GhostChild != null)
        {
            Destroy(GhostChild);
        }
        SpawnNewGhost();
	}
	
	// Update is called once per frame
	void Update () {
        RespawnTimer -= Time.deltaTime;
	}

    private void FixedUpdate()
    {
        if (GhostChild == null)
        {
            if (RespawnTimer <=0)
            {
                SpawnNewGhost();
            }
        }
        else
        {
            if (GhostScript.TargetEngage && LevelGM.Instance.playDeath)
            {
                Destroy(GhostChild);
                SpawnNewGhost();
            }
        }
    }

    void SpawnNewGhost()
    {
        RespawnTimer = 0;
        GhostChild = GameObject.Instantiate(GhostPrefab, transform);
        GhostChild.transform.position = transform.position;
        GhostScript = GhostChild.GetComponent<GhostZahra>();
        Debug.Log("Ghost Spawned");
    }

    void TellGhostToGo(GameObject myTrigg)
    {
        if (GhostChild != null && GhostScript != null)
        {
            if (!GhostScript.TargetEngage)
            {
                Debug.Log("Player Entered");
                GhostScript.TargetIsSighted(myTrigg.transform.position, myTrigg.transform); }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyMineMaster : MonoBehaviour {

    public Player_States.ProximityMineMasterTypes MineType;
    public GameObject MinePrefab;
    public GameObject MineObj;
    public ProxyMine MineScript;
    public ProxyMineExplosion ExplosionScript;
    public float OffscreenCheckDistance = 30;
    public float offscreenCheckHeight = 10;
    public float TrackingMineTimeUntilExplode = 3f;
    public GameObject PlayerObj;

    public Vector3 ExplosionInitialSize;

    public bool ExplosionHappened =false;

    // Use this for initialization
    void Start () {
        MineObj = GameObject.Instantiate(MinePrefab, transform.position, transform.rotation);
        MineObj.transform.parent = transform;
        MineScript = MineObj.GetComponent<ProxyMine>();
        MineScript.MineType = MineType;
        ExplosionScript = MineObj.transform.GetChild(0).gameObject.GetComponent<ProxyMineExplosion>();
        ExplosionInitialSize = ExplosionScript.gameObject.transform.lossyScale;
        ExplosionScript.InitialScale = ExplosionInitialSize;
        ExplosionScript.ExplosionStart = false;
        MineScript.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (ExplosionHappened)
        {
            //ExplosionScript.gameObject.SetActive(false);
            //MineScript.gameObject.SetActive(false);



            FindThePlayer();
            if (PlayerObj == null || LevelGM.Instance.playDeath)
            {
                ResetMines();
            }
        }
	}

    public GameObject FindThePlayer()
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
    }

    void ResetMines()
    {
        Destroy(MineObj);

        MineObj = GameObject.Instantiate(MinePrefab, transform.position, transform.rotation);
        MineObj.transform.parent = transform;
        MineScript = MineObj.GetComponent<ProxyMine>();
        MineScript.MineType = MineType;
        ExplosionScript = MineObj.transform.GetChild(0).gameObject.GetComponent<ProxyMineExplosion>();
        ExplosionInitialSize = ExplosionScript.gameObject.transform.lossyScale;
        ExplosionScript.InitialScale = ExplosionInitialSize;
        ExplosionScript.ExplosionStart = false;
        MineScript.enabled = true;

        /*MineScript.transform.localPosition = new Vector3(0,0,0);
        MineScript.gameObject.SetActive(true);
        MineScript.TargetToChase = null;
        MineScript.SetMineOff = false;
        MineScript.StopOperationsNow = false;
        MineScript.BeginOperationsNow = false;
        MineScript.transform.localPosition = new Vector3(0, 0, 0);
        ExplosionHappened = false;
        ExplosionScript.transform.parent = MineScript.transform;
        ExplosionScript.transform.localPosition = MineScript.transform.localPosition;
        ExplosionScript.gameObject.SetActive(true);
        ExplosionScript.ExplosionStart = false;
        ExplosionScript.Start2 = false;

        ExplosionScript.transform.localPosition = MineScript.transform.localPosition;
        ExplosionScript.gameObject.SetActive(false);*/

    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if(myTrigg.tag == "Player" && MineScript.gameObject.activeSelf == true)
        {
            MineScript.SetMineOff = true;
            MineScript.TargetToChase = myTrigg.transform;
            MineScript.ChaseSpeed = TrackingMineTimeUntilExplode;
        }
    }
}

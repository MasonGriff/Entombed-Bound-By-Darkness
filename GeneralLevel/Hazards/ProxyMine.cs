using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProxyMine : MonoBehaviour {

    ProxyMineMaster ParentScript;

    Player_Main plyr;

    public bool SetMineOff;
    public bool BeginOperationsNow;
    public bool StopOperationsNow;

    Rigidbody2D myRB;
    Collider2D myColl;

    [HideInInspector]
    public Player_States.ProximityMineMasterTypes MineType;
    [HideInInspector]
    public Transform TargetToChase;
    [HideInInspector]
    public float ChaseSpeed;

    float chaseSppedGoneBy;

	// Use this for initialization
	void Start () {
        ParentScript = transform.parent.GetComponent<ProxyMineMaster>();

        myRB = GetComponent<Rigidbody2D>();
        myColl = GetComponent<Collider2D>();
        SetMineOff = false;
        BeginOperationsNow = false;
        StopOperationsNow = false;
        chaseSppedGoneBy = 0;
	}
	
	// Update is called once per frame
	void Update () {
        chaseSppedGoneBy -= Time.deltaTime;
        GameObject playerBody = GameObject.FindGameObjectWithTag("Player");

        if (playerBody !=null)
        {
            plyr = playerBody.GetComponent<Player_Main>();
            Physics2D.IgnoreCollision(myColl, plyr.myColl);
        }
        if (SetMineOff && !BeginOperationsNow && !StopOperationsNow)
        {
            switch (MineType)
            {
                case Player_States.ProximityMineMasterTypes.Floating:
                    SittingMine();
                    break;
                case Player_States.ProximityMineMasterTypes.Tracking:
                    ChasingMine();
                    break;
                default:
                    MineType = Player_States.ProximityMineMasterTypes.Floating;
                    break;
            }
        }
        if (BeginOperationsNow && !StopOperationsNow)
        {

            transform.DOMove(TargetToChase.position, ChaseSpeed);
            if (chaseSppedGoneBy <= 0)
            {
                SittingMine();
            }
        }
        if (StopOperationsNow)
        {
            StopOperationsFunction();
        }
	}

    void StopOperationsFunction()
    {

        transform.DOMove(transform.position, 0);
        BeginOperationsNow = false;
        StopOperationsNow = false;
        SetMineOff = false;
        BeginOperationsNow = false;
        StopOperationsNow = false;

        //gameObject.SetActive(false);
    }

    void ChasingMine()
    {
        BeginOperationsNow = true;
        chaseSppedGoneBy = ChaseSpeed;
        transform.DOMove(TargetToChase.position, ChaseSpeed);
    }
    void SittingMine()
    {
        transform.DOMove(transform.position, 0);
        StopOperationsNow = true;
        ParentScript.ExplosionScript.gameObject.SetActive(true);
        ParentScript.ExplosionScript.ExplosionStart = true;
        ParentScript.ExplosionScript.MyMaster = ParentScript;
    }

}

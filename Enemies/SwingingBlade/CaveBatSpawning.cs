using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveBatSpawning : MonoBehaviour {

    public GameObject MeObj;
    public GameObject CameraObj;
    public Transform CamRight;
    public Transform CamLeft;
    public GameObject BatThatsMoving;
    public CaveBat BatScript;
    public bool BatAlive = false;
    public Transform LeftPos;
    public Transform RightPos;
    public Transform DropPos;


	// Use this for initialization
	void Awake () {
        MeObj = this.gameObject;
        BatThatsMoving = MeObj.transform.Find("Bat").gameObject;
        CameraObj = LevelGM.Instance.MainCameraObj;
        BatScript = BatThatsMoving.GetComponent<CaveBat>();
        CamRight = CameraObj.transform.Find("CamEdgeRight").transform;
        CamLeft = CameraObj.transform.Find("CamEdgeLeft").transform;
        LeftPos = MeObj.transform.Find("BatPosLeft").transform;
        RightPos = MeObj.transform.Find("BatPosRight").transform;
        DropPos = MeObj.transform.Find("BatPosLow").transform;
        
        BatAlive = false;
        BatThatsMoving.SetActive(false);
        //Player = GameObject.Find("Alyzara").transform;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (!BatAlive)
        {
            if(CamLeft.position.x > RightPos.position.x || CamRight.position.x < LeftPos.position.x)
            {
                BatRespawn();
            }
        }
	}

    void BatRespawn()
    {
        BatThatsMoving.SetActive(true);
        BatThatsMoving.transform.position = RightPos.transform.position;
        BatScript.myAnim.SetInteger("AnimState", 0);
        BatScript.myAnim.SetBool("Die", false);
        BatAlive = true;
    }

}

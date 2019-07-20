using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailScript : MonoBehaviour {

    public GameObject SmokePrefab;
    public GameObject SmokeInstance;

    public ParticleSystem SmokeSystem;

    public GameObject PlayerObj;
    public Player_Main plyr;
      
    // Use this for initialization
    void Start() {
        gameObject.SetActive(false);
        if (SmokeInstance!= null)
        { Destroy(SmokeInstance); }

        CreateNewSmoke();
    }

    // Update is called once per frame
    void Update() {

        PlayerObj = LevelGM.Instance.PlayerInLevel;
        if (SmokeInstance !=null)
        {
            if (PlayerObj != null)
            {
                plyr = PlayerObj.GetComponent<Player_Main>();


                if (plyr.ClingToWall)
                {
                    transform.position = plyr.SmokeTrailPosition.position;
                    SmokeSystem.Play();
                }
                else
                {
                    if (SmokeSystem.isPlaying)
                    {
                        SmokeInstance.transform.parent = null;
                        SmokeSystem.Stop();
                        CreateNewSmoke();
                    }
                }
            }
            else
            {
                if (SmokeSystem.isPlaying)
                {

                    SmokeInstance.transform.parent = null;
                    SmokeSystem.Stop();
                    CreateNewSmoke();
                }
            }
        }
    }

    void CreateNewSmoke()
    {
        SmokeInstance = GameObject.Instantiate(SmokePrefab, transform);
        SmokeSystem = SmokeInstance.GetComponent<ParticleSystem>();
    }
}

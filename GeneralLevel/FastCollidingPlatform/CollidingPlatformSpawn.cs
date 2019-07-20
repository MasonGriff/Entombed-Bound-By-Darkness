using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingPlatformSpawn : MonoBehaviour {

    public enum SpawnTypes { TopSmall, LowSmall, Big, TwoSmall }
    public Transform BigSpawn,TopSpwan,LowSpawn;
    public GameObject BigWall, SmallWall;
    public float WaitTime = 2;
    float timeWaited = 0;
    public Collider2D WallIgnore, FloorIgnore;

    public SpawnTypes SpawnHere = SpawnTypes.TopSmall;


	// Use this for initialization
	void Start () {
        timeWaited = 0;
        RandomSpawnCheck();
	}
	
	// Update is called once per frame
	void Update () {
        timeWaited -= Time.deltaTime;
        if (timeWaited <= 0)
        {
            RandomSpawnCheck();
        }
	}

    void RandomSpawnCheck()
    {
        timeWaited = WaitTime;
        SpawnCollidingPlatform();
    }

    void SpawnCollidingPlatform()
    {
        int RandomChanceSpawn = Random.Range(0, 100);
        if (RandomChanceSpawn > 80)
        {
            SpawnHere = SpawnTypes.Big;
        }
        else if (RandomChanceSpawn <=80 && RandomChanceSpawn > 60)
        {
            SpawnHere = SpawnTypes.LowSmall;
        }
        else if (RandomChanceSpawn <= 60 && RandomChanceSpawn > 40)
        {
            SpawnHere = SpawnTypes.TwoSmall;
        }
        else if (RandomChanceSpawn <= 40 && RandomChanceSpawn > 20)
        {
            SpawnHere = SpawnTypes.TopSmall;
        }
        else if (RandomChanceSpawn <= 20)
        {
            SpawnHere = SpawnTypes.Big;
        }
        SpawnGo();
        
    }

    void SpawnGo()
    {
        GameObject WallGoing;
        GameObject WallGoing2;
        FastCollidingPlatform FastPlatformScript;
        switch (SpawnHere)
        {
            case SpawnTypes.Big:
                WallGoing = GameObject.Instantiate(BigWall, BigSpawn.transform.position, BigSpawn.transform.rotation);
                FastPlatformScript = WallGoing.GetComponent<FastCollidingPlatform>();
                FastPlatformScript.WallIgnore = WallIgnore;
                FastPlatformScript.FloorIgnore = FloorIgnore;
                break;
            case SpawnTypes.LowSmall:
                WallGoing = GameObject.Instantiate(SmallWall, LowSpawn.transform.position, LowSpawn.transform.rotation);
                FastPlatformScript = WallGoing.GetComponent<FastCollidingPlatform>();
                FastPlatformScript.WallIgnore = WallIgnore;
                FastPlatformScript.FloorIgnore = FloorIgnore;
                break;
            case SpawnTypes.TopSmall:
                WallGoing = GameObject.Instantiate(SmallWall, TopSpwan.transform.position, TopSpwan.transform.rotation);
                FastPlatformScript = WallGoing.GetComponent<FastCollidingPlatform>();
                FastPlatformScript.WallIgnore = WallIgnore;
                FastPlatformScript.FloorIgnore = FloorIgnore;
                break;
            case SpawnTypes.TwoSmall:
                WallGoing = GameObject.Instantiate(SmallWall, LowSpawn.transform.position, LowSpawn.transform.rotation);
                FastPlatformScript = WallGoing.GetComponent<FastCollidingPlatform>();
                FastPlatformScript.WallIgnore = WallIgnore;
                FastPlatformScript.FloorIgnore = FloorIgnore;
                WallGoing2 = GameObject.Instantiate(SmallWall, TopSpwan.transform.position, TopSpwan.transform.rotation);
                FastPlatformScript = WallGoing2.GetComponent<FastCollidingPlatform>();
                FastPlatformScript.WallIgnore = WallIgnore;
                FastPlatformScript.FloorIgnore = FloorIgnore;
                break;
        }
    }
}

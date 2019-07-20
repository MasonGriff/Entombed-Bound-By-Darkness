using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProxyMineExplosion : MonoBehaviour {
   public ProxyMineMaster MyMaster; 

    public int DamageOutput = 1;

    public float ExplosionTime = 2f;
    public bool ExplosionStart = false;
    public bool Start2 = false;
    public ParticleSystem ExplosionEffect;
    public GameObject ExplosionParticleObj;
    public Vector3 InitialScale;

    float countDownNow = 0;

    private void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }
    private void Awake()
    {

        transform.localScale = new Vector3(0, 0, 0);
    }
    // Use this for initialization
    void Start ()
    {
        transform.localScale = new Vector3(0, 0, 0);

    }
	
	// Update is called once per frame
	void Update () {
        countDownNow -= Time.deltaTime;
        if (ExplosionStart && !Start2)
        {
            
            
            transform.localScale = new Vector3(0, 0, 0);
            ExplosionParticleObj.SetActive(true);
            ExplosionParticleObj.transform.parent = null;
            ExplosionEffect.Play();
            transform.DOScale(InitialScale, ExplosionTime);
            countDownNow = ExplosionTime;
            Start2 = true;
        }
        if (Start2)
        {
            transform.parent = null;
            transform.position = MyMaster.MineScript.transform.position;
            MyMaster.MineScript.gameObject.SetActive(false);
            if (countDownNow <= 0)
            {
                transform.localScale = new Vector3(0, 0, 0);
                MyMaster.ExplosionHappened = true;
                Destroy(ExplosionParticleObj);
                gameObject.SetActive(false);
            }
        }

	}


    private void OnTriggerEnter2D(Collider2D myExplodingTrigg)
    {
        if (ExplosionStart)
        {
            if (myExplodingTrigg.tag == "Player")
            {
                myExplodingTrigg.GetComponent<Player_Main>().DamageTaken(DamageOutput);
            }
        }
    }
}

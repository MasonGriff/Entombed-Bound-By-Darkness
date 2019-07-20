using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Melee : MonoBehaviour {

    public FinalBoss_Main FinalBoss;

    public bool PlayerHit = false;
    public bool KillMeleeNow = false;


    private void Start()
    {
        PlayerHit = false;
        KillMeleeNow = false;
    }

    private void Update()
    {
        if (KillMeleeNow)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            if (!PlayerHit)
            {
                PlayerHit = true;
                FinalBoss.Plyr.DamageTaken(FinalBoss.DamageOutput);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player")
        {
            PlayerHit = false;
        }
    }
}

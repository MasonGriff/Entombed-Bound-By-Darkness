using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int Health;
    public int FullHealth;
    //public int BreakMeter;
    //public int BreakingPoint;
    //public bool Broken;
    public bool damageEnabled = true;

    public bool Damaged = false;
    public int damageTaken;
    public bool MeleeImmunity = false;

    public bool dead;

    Player_Main Player;

    void Start()
    {
        
    }

    private void Update()
    { 
    }

    private void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "PlayerMelee")
        {
            if (damageEnabled && !Damaged)
            {
                Player = myTrigg.gameObject.transform.parent.parent.GetComponent<Player_Main>();
                if (Player.MeleeScript.SwordsOwned >0)
                {
                    Damaged = true;
                    damageTaken = Player.MeleeScript.DamageDealt;
                    //MeleeImmunity = true;
                    damageEnabled = false;

                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "PlayerMelee")
        {
                Player = myTrigg.gameObject.transform.parent.parent.GetComponent<Player_Main>();
                Damaged = false;
                damageTaken = 0;
        } 
    }

    void BreakingPointReached()
    {

    }

}

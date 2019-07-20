using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageToPlayer;

    public bool isSnake;
    public GameObject aggroZone;
    Snake snakeScript;
    public float retreatTime;

    

    void Start()
    {
        //Use for snake enemies since they are basically smart hazards.
        if (isSnake == true)
        {
            snakeScript = gameObject.GetComponentInParent<Snake>();
        }
        if (Game.current.Progress.CurrentDifficulty == Player_States.DifficultySetting.Easy)
        {
            damageToPlayer = 1;
        }
    }

    private void OnCollisionStay2D(Collision2D myColl)
    {
        if (myColl.gameObject.tag == "Player")
        {
            GameObject Zahra = myColl.gameObject;
            Player_Main Plyr = Zahra.GetComponent<Player_Main>();
            if (Plyr.PlayerHealthState != Player_States.PlayerHealthStates.Dead && Plyr.PlayerState!= Player_States.PlayerStates.Damaged)
            {
                if (damageToPlayer < Plyr.HealthScript.MaxHealth)
                { Plyr.DamageTaken(damageToPlayer); }
                else
                {
                    Plyr.DeathSetup();
                }
                Debug.Log("Player Hit");


                //If the hazard is a snake, it will retreat upon damaging the player.
                if (isSnake == true && snakeScript.retreat == false)
                {
                    snakeScript.aggro = false;
                    snakeScript.retreat = true;
                    StartCoroutine(Retreating());
                }
            }
        }
    }

    /*private void OnTriggerEnter2D(Collision2D myColl)
    {
        if (myColl.gameObject.tag == "Player")
        {
            GameObject Zahra = myColl.gameObject;
            Player_Main Plyr = Zahra.GetComponent<Player_Main>();
            Plyr.DamageTaken(damageToPlayer);
            Debug.Log("Player Hit");

            //If the hazard is a snake, it will retreat upon damaging the player.
            if (isSnake == true && snakeScript.retreat == false)
            {
                snakeScript.aggro = false;
                snakeScript.retreat = true;
                StartCoroutine(Retreating());
            }
        }
    }*/

    //Make the snake wait a set amount of time before it attacks again.
    IEnumerator Retreating()
    {
        yield return new WaitForSeconds(retreatTime);
        snakeScript.retreat = false;
    }
}

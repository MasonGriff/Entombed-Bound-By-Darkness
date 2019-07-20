using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEnabler : MonoBehaviour
{
    Player_Main plyr;
    public GameObject dialogue;
    public bool hasTriggered;

    void Start()
    {
        hasTriggered = false;
    }

    void OnTriggerEnter2D(Collider2D myTrigg)
    {
        if (myTrigg.tag == "Player" && hasTriggered == false)
        {
            Game.current.Progress.GameplayPaused = false;
            dialogue.SetActive(true);

            plyr = CameraController.Instance.Player.GetComponent<Player_Main>();
            plyr.myRB.velocity = new Vector2(0, 0);
            plyr.myAnim.SetInteger("AnimState", 0);
            plyr.myAnim.SetBool("Dashing", false);
            plyr.myAnim.SetBool("Jump", false);
            plyr.myAnim.SetBool("Airborne", false);
            plyr.myAnim.SetBool("Dashing", false);
            plyr.MeleeScript.ResetMeleeState();
            plyr.myAnim.SetBool("ClearMind", false);

            hasTriggered = true;
        }
    }
}

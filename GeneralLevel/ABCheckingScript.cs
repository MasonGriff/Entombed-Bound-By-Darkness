using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABCheckingScript : MonoBehaviour {
    
    public Player_States.LevelSetNumber LevelSet;
    public Player_States.StageSetNumber StageSet;

    public GameObject PathAObstacles, PathBObstacles;

    public AudioClip PathASong, PathBSong;

	// Use this for initialization
	void Start () {
        LevelGM.Instance.PathScript = this;
		if (PathAObstacles != null)
        {
            PathAObstacles.SetActive(false);

        }
        if (PathBObstacles != null)
        {
            PathBObstacles.SetActive(false);

        }

        /*if (Game.current == null) //If this isn't on a save file.
        {
            Game.current = new Game(); //Creates new default save file.
        }*/
        
        switch (LevelSet)
        {
            case Player_States.LevelSetNumber.Levels1:
                /*Game.current.Progress.Stage1Cleared = false;
                Game.current.Progress.Stage2Cleared = false;
                Game.current.Progress.Stage3Cleared = false;*/

                SaveAndLoad.Save(Game.current.Progress.FileNumber);
                break;
            case Player_States.LevelSetNumber.Levels2:
                /*Game.current.Progress.Stage1Cleared = true;
                Game.current.Progress.Stage2Cleared = false;
                Game.current.Progress.Stage3Cleared = false;*/
                if (PathAObstacles != null && PathBObstacles != null)
                { SetPathForObstacles(Game.current.Progress.Sword1); }
                SaveAndLoad.Save(Game.current.Progress.FileNumber);
                break;
            case Player_States.LevelSetNumber.Levels3:
                /*Game.current.Progress.Stage1Cleared = true;
                Game.current.Progress.Stage2Cleared = true;
                Game.current.Progress.Stage3Cleared = false;*/
                if (PathAObstacles != null && PathBObstacles != null)
                { SetPathForObstacles(Game.current.Progress.Sword2); }
                SaveAndLoad.Save(Game.current.Progress.FileNumber);
                break;
            case Player_States.LevelSetNumber.FinalBoss:
                /*Game.current.Progress.Stage1Cleared = true;
                Game.current.Progress.Stage2Cleared = true;
                Game.current.Progress.Stage3Cleared = true;*/
                SaveAndLoad.Save(Game.current.Progress.FileNumber);
                break;
        }


    }
	
	/*// Update is called once per frame
	void Update () {
		
	}*/
    


    void SetPathForObstacles(Player_States.SwordsObtainedStatus SwordStatus)
    {
        switch (SwordStatus)
        {
            case Player_States.SwordsObtainedStatus.Obtained:
                PathAObstacles.SetActive(true);
                LevelGM.Instance.BackgroundMusicSource.Stop();
                LevelGM.Instance.BackgroundMusicSource.clip = PathASong;
                LevelGM.Instance.BackgroundMusicSource.Play();
                break;
            case Player_States.SwordsObtainedStatus.Ignored:
                PathBObstacles.SetActive(true);
                LevelGM.Instance.BackgroundMusicSource.Stop();
                LevelGM.Instance.BackgroundMusicSource.clip = PathBSong;
                LevelGM.Instance.BackgroundMusicSource.Play();
                break;
        }
    }

}

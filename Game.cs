using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Game {

    /// <summary>
    /// This script is the middle man between the game and the save file currently loaded. 
    /// Certain operations cannot be performed if there is no loaded save file.
    /// To that end, there must always be an active Game.current going in the scene to make sure things are working.
    /// If there is no Game.current active at the moment, simply call the function -> Game.current = new Game();
    /// </summary>

    public static Game current; //The currently running game. It's needed to access the save file.
    public Character Progress; //The currently running save file.
    public Character Alyzara; //Outdated, kept in to prevent compiler errors for the time being
    //public OptionsSaved Options;

    public Game()
    {
        Progress = new Character();
    }


}

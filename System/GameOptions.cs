using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameOptions{
    /// <summary>
    /// This script functions much like the script Game, but it is specifically for the player's game options.
    /// Much like Game, this script requires that there is always an active GameOptions.current loaded into a scene.
    /// If there is no GameOptions.curent loaded in, create a temporary new one via the function -> GameOptions.current = new GameOptions();
    /// </summary>

         
    public static GameOptions current; //The current game options.
    public OptionsSaved Settings; //The currently loaded save file of the game options.

    public GameOptions()
    {
        Settings = new OptionsSaved();
    }

}

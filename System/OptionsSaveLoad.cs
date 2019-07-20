using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class OptionsSaveLoad : MonoBehaviour {

    /// <summary>
    /// A separate save/load system specifically for the player's game options. Functions very similarly to the regular Save/Load system.
    /// </summary>

    public static List<GameOptions> savedOptions = new List<GameOptions>();
    public static bool OptionsSaveExists = false;

    public static void SaveSettings()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {
            //myFolderExists = false;
            Debug.Log("No Folder, creating new one.");
            Directory.CreateDirectory(Application.persistentDataPath + "/entombed-bbd/");
        }
        Debug.Log("Saved your options");
        savedOptions.Add(GameOptions.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/entombed-bbd/bbd-settings.gd");
        bf.Serialize(file, OptionsSaveLoad.savedOptions);
        file.Close();
    }

    public static void LoadSettings(bool LoadNow)
    {
        //bool myFolderExists = false;
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {
            //myFolderExists = false;
            Debug.Log("No Folder");
            Directory.CreateDirectory(Application.persistentDataPath + "/entombed-bbd/");
        }
        else
        {
            // myFolderExists = true;
            Debug.Log("Directory Exists");
        }
        if (File.Exists(Application.persistentDataPath + "/entombed-bbd/bbd-settings.gd"))
        {
            OptionsSaveExists = true;
            Debug.Log("Saved Options Found");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/entombed-bbd/bbd-settings.gd", FileMode.Open);
            print(Application.persistentDataPath);
            OptionsSaveLoad.savedOptions = (List<GameOptions>)bf.Deserialize(file);
            AddLoadedOptions();
            file.Close();
        }
        else
        {
            OptionsSaveExists = false;
            Debug.Log("No Saved Options");
        }
    }

    public static void LoadSettings()
    {
        //bool myFolderExists = false;
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {
            //myFolderExists = false;
            Debug.Log("No Folder");
            Directory.CreateDirectory(Application.persistentDataPath + "/entombed-bbd/");
        }
        else
        {
           // myFolderExists = true;
            Debug.Log("Directory Exists");
        }
        if (!File.Exists(Application.persistentDataPath + "/entombed-bbd/bbd-settings.gd"))
        {
            OptionsSaveExists = false;
            Debug.Log("No Saved Options");
            GameOptions.current = new GameOptions();
            SaveSettings();
        }
        else
        {

            OptionsSaveExists = true;
            Debug.Log("Saved Options Found");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/entombed-bbd/bbd-settings.gd", FileMode.Open);
            print(Application.persistentDataPath);
            OptionsSaveLoad.savedOptions = (List<GameOptions>)bf.Deserialize(file);
            AddLoadedOptions();
            file.Close();
        }
    }


    public static void AddLoadedOptions()
    {
        foreach (GameOptions g in savedOptions)
        {
            GameOptions.current = g;
            Debug.Log("Options loaded correctly");
        }
    }
    
}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveAndLoad
{
    /// <summary>
    /// Public static script that holds the functions for saving and loading a save file.
    /// </summary>

    public static List<Game> savedGames = new List<Game>();
    public static bool SaveExists = false;
    public static bool Save2Exists = false;

    public static void Save(int SaveFileNumber)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {
            
            Debug.Log("No Folder");
            Directory.CreateDirectory(Application.persistentDataPath + "/entombed-bbd/");
        }
        Game.current.Progress.SaveDate = System.DateTime.Now.ToString();
        savedGames.Add(Game.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + ("/entombed-bbd/SaveGameFile"+SaveFileNumber+".gd"));
        bf.Serialize(file, SaveAndLoad.savedGames);
        file.Close();
    }
    
    public static void Load(int SaveFileNumber)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {

            Debug.Log("No Folder");
            Directory.CreateDirectory(Application.persistentDataPath + "/bbdentombed/");
        }
        if (File.Exists(Application.persistentDataPath + ("/entombed-bbd/SaveGameFile" + SaveFileNumber + ".gd")))
        {
            if (SaveFileNumber == 1)
            { SaveExists = true; }
            else
            {
                Save2Exists = true;
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + ("/entombed-bbd/SaveGameFile" + SaveFileNumber + ".gd"), FileMode.Open);
            
            //File.Delete(file.Name);
            SaveAndLoad.savedGames = (List<Game>)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            if (SaveFileNumber == 1)
            { SaveExists = false; }
            else
            {
                Save2Exists = false;
            }
        }
    }

    public static void LoadExists()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/entombed-bbd/"))
        {

            Debug.Log("No Folder");
            Directory.CreateDirectory(Application.persistentDataPath + "/entombed-bbd/");
        }
        if (File.Exists(Application.persistentDataPath + ("/entombed-bbd/SaveGameFile" + 1 + ".gd")))
        {
            SaveExists = true;
        }
        else
        {
            SaveExists = false;
        }
        if (File.Exists(Application.persistentDataPath + ("/entombed-bbd/SaveGameFile" + 2 + ".gd")))
        {
            Save2Exists = true;
        }
        else
        {
            Save2Exists = false;
        }
    }

    public static void DeleteSave(int SaveFileNumber)
    {
        if (File.Exists(Application.persistentDataPath + "/entombed-bbd/SaveGameFile" + SaveFileNumber + ".gd"))
        {
            SaveExists = true;
            BinaryFormatter bf = new BinaryFormatter();
            File.Delete(Application.persistentDataPath + "/entombed-bbd/SaveGameFile" + SaveFileNumber + ".gd");
            //File.Delete(file.Name);
            //SaveAndLoad.savedGames = (List<Game>)bf.Deserialize(file);
            //File.Delete(Application.persistentDataPath + "/savedGames.gd");
            //file.Close();
            //file = null;
            if (SaveFileNumber == 1)
            { SaveExists = false; }
            else
            {
                Save2Exists = false;
            }
        }
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsSaved{
    /// <summary>
    /// This is the script containing everything in a save file for game options.
    /// </summary>


    

    //=== Volume ===
    public float VolumeSFX;
    public float VolumeVoice;
    public float VolumeMusic;
    public float VolumeMaster;

    public Player_States.PlayerLanguageSetting GameLanguageOptions;

    public OptionsSaved()
    {
        VolumeMaster = 1;
        VolumeMusic = .9f;
        VolumeVoice = .9f;
        VolumeSFX = .9f;

        GameLanguageOptions = Player_States.PlayerLanguageSetting.English;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;

public class VolumeSettingsFromOptions : MonoBehaviour {

    public enum myAudioTypeToPlay { BGM, SFX, Voice, MasterOnly}
    public myAudioTypeToPlay SoundControlType;
    public AudioSource myAudioPlayer;


	// Use this for initialization
	void Start () {
		if (myAudioPlayer == null)
        {
            myAudioPlayer = GetComponent<AudioSource>();
        }
        if (GameOptions.current != null)
        {
            switch (SoundControlType)
            {
                case myAudioTypeToPlay.BGM:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeMusic * GameOptions.current.Settings.VolumeMaster);

                    break;
                case myAudioTypeToPlay.SFX:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeSFX * GameOptions.current.Settings.VolumeMaster * 0.25f);

                    break;
                case myAudioTypeToPlay.Voice:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeSFX * GameOptions.current.Settings.VolumeMaster);

                    break;
                case myAudioTypeToPlay.MasterOnly:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeMaster);

                    break;
            }
        }
    
}
	
	// Update is called once per frame
	void Update () {
		if (GameOptions.current != null)
        {
            switch (SoundControlType)
            {
                case myAudioTypeToPlay.BGM:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeMusic * GameOptions.current.Settings.VolumeMaster);

                    break;
                case myAudioTypeToPlay.SFX:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeSFX * GameOptions.current.Settings.VolumeMaster * 0.5f);

                    break;
                case myAudioTypeToPlay.Voice:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeSFX * GameOptions.current.Settings.VolumeMaster);

                    break;
                case myAudioTypeToPlay.MasterOnly:
                    myAudioPlayer.volume = (GameOptions.current.Settings.VolumeMaster);

                    break;
            }
        }
	}
}

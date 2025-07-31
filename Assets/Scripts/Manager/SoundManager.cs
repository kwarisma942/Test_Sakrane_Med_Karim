using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Public Variable

    public static SoundManager Instance;

    public AudioSource MusicSource, FxSource;
    public AudioClip[] FxClips;

    public Text SoundTxt, MusicTxt;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("SFXOff", 0) == 1)
        {
            FxSource.volume = 0;
            SoundTxt.text = "SFX ON";
        }

        if (PlayerPrefs.GetInt("MUSICOFF", 0) == 1)
        {
            MusicSource.volume = 0;
            MusicTxt.text = "MUSIC ON";
        }
    }

    #endregion

    #region Public Methode

    public void PlaySoundFX(int index)
    {
        FxSource.PlayOneShot(FxClips[index]);
    }

    public void SoundOnOff()
    {
        if (PlayerPrefs.GetInt("SFXOff", 0) == 1)
        {
            PlayerPrefs.SetInt("SFXOff", 0);
            FxSource.volume = 1;
            SoundTxt.text = "SFX OFF";
        }
        else
        {
            PlayerPrefs.SetInt("SFXOff", 1);
            FxSource.volume = 0;
            SoundTxt.text = "SFX ON";
        }
    }

    public void MusicOnOff()
    {
        if (PlayerPrefs.GetInt("MUSICOFF", 0) == 1)
        {
            PlayerPrefs.SetInt("MUSICOFF", 0);
            MusicSource.volume = 1;
            MusicTxt.text = "MUSIC OFF";
        }
        else
        {
            PlayerPrefs.SetInt("MUSICOFF", 1);
            MusicSource.volume = 0;
            MusicTxt.text = "MUSIC ON";
        }
    }

    #endregion
}

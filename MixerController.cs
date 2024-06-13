using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SFXVolume";
    public const string MIXER_MASTER = "MasterVolume";

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_SFX, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_MASTER, 1f);
    }
    void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat(AudioManager.KEY_MUSIC, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.KEY_SFX, sfxSlider.value);
        PlayerPrefs.SetFloat(AudioManager.KEY_MASTER, masterSlider.value);
        PlayerPrefs.Save();
    }

    void SetMusicVolume(float value2)
    {
        audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value2) * 20);
    }  
    void SetSFXVolume(float value3)
    {
        audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(value3) * 20);
    }
    void SetMasterVolume(float value)
    {
        audioMixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }
    
}

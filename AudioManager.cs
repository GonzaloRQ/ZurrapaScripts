using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    public const string KEY_MUSIC = "MusicVolume";
    public const string KEY_SFX = "SFXVolume";
    public const string KEY_MASTER = "MasterVolume";

    void Start()
    {
        Load();   
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    void Load() //Volumen guardado en el mixercontroller
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_SFX, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(AudioManager.KEY_MASTER, 1f);
        float musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        float masterVolume = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        mixer.SetFloat(MixerController.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(MixerController.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
        mixer.SetFloat(MixerController.MIXER_MASTER, Mathf.Log10(masterVolume) * 20);
    }
}

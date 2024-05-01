using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSidler;
    [SerializeField] private Slider SFXSidler;

    private void Start() => Checker();

    public void SetMusicVolume()
    {
        float volume = musicSidler.value;
        myMixer.SetFloat("Background Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSidler.value;
        myMixer.SetFloat("SFX Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        musicSidler.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXSidler.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMusicVolume();
        SetSFXVolume();
    }

    private void Checker()
    {
        if (PlayerPrefs.HasKey("MusicVolume")) LoadVolume();
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [SerializeField]
    Button runButton;
    [SerializeField]
    Button exitButton;

    [SerializeField]
    Toggle fullscreen_Toggle;
    [SerializeField]
    Toggle vsync_Toggle;   

    [SerializeField]
    Toggle music_Toggle;
    [SerializeField]
    AudioSource musicClip;

    [SerializeField]
    Toggle sfx_Toggle;
    [SerializeField]
    AudioSource sfxClip;

    [SerializeField]
    Slider musicVolumeSlider;
    [SerializeField]
    Slider sfxVolumeSlider;

    private void Awake()
    {
        Application.targetFrameRate = 240;
    }

    private void OnEnable()
    {
        exitButton.onClick.AddListener(() => { Application.Quit(); });
        runButton.onClick.AddListener(delegate { RunButtonAction(); });
        vsync_Toggle.onValueChanged.AddListener(delegate { ToggleVsync(); });
        fullscreen_Toggle.onValueChanged.AddListener(delegate { ToggleFullscreen(); });
        music_Toggle.onValueChanged.AddListener(delegate { ToggleMusic(); });
        sfx_Toggle.onValueChanged.AddListener(delegate { ToggleSFX(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    void RunButtonAction()
    {
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
           
        if (sfxClip.isPlaying == false)
        {
            sfxClip.Play();
        }
    }

    void ToggleVsync()
    {
        if (vsync_Toggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
    void ToggleFullscreen()
    {
        if (fullscreen_Toggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    void ToggleMusic()
    {
        if (music_Toggle.isOn)
        {
            musicClip.mute = false;
        }
        else
        {
            musicClip.mute = true;
        }
    }

    void ToggleSFX()
    {
        if (sfx_Toggle.isOn)
        {
            sfxClip.mute = false;
        }
        else
        {
            sfxClip.mute = true;
        }
    }

    void SetMusicVolume()
    {
        musicClip.volume = musicVolumeSlider.value;
    }

    void SetSFXVolume()
    {
        sfxClip.volume = sfxVolumeSlider.value;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class menu2 : MonoBehaviour
{
    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Options")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MasterVolumeKey = "CTAS_MasterVolume";
    private const string MusicVolumeKey = "CTAS_MusicVolume";
    private const string SFXVolumeKey = "CTAS_SFXVolume";
    private const string FullscreenKey = "CTAS_Fullscreen";
    void Start()
    {
        optionsPanel.SetActive(false);
        LoadOptions();
    }

    // =========================
    // OPTIONS
    // =========================

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        PlayerPrefs.Save();
        optionsPanel.SetActive(false);
    }

    // =========================
    // OPTIONS - VOLUME
    // =========================

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        Debug.Log("Master Volume: " + value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        Debug.Log("Music Volume: " + value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
        Debug.Log("SFX Volume: " + value);
    }

    // =========================
    // FULLSCREEN
    // =========================

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt(
            FullscreenKey,
            fullscreen ? 1 : 0
        );

        PlayerPrefs.Save();
    }

        private void LoadOptions()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        Screen.fullScreen = fullscreen;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }
}

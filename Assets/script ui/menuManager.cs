using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("New Game Panel")]
    [SerializeField] private GameObject newGamePanel;

    [Header("Load Game Panel")]
    [SerializeField] private GameObject loadGamePanel;

    [Header("Exit Confirmation Panel")]
    [SerializeField] private GameObject exitConfirmPanel;

    [Header("Options")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string SaveKey = "CTAS_SaveData";
    private const string MasterVolumeKey = "CTAS_MasterVolume";
    private const string MusicVolumeKey = "CTAS_MusicVolume";
    private const string SFXVolumeKey = "CTAS_SFXVolume";
    private const string FullscreenKey = "CTAS_Fullscreen";

    private void Start()
    {
        InitializeMenu();
        LoadOptions();
    }

    // =========================
    // MENU
    // =========================

    private void InitializeMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
        exitConfirmPanel.SetActive(false);
    }

    // =========================
    // NEW GAME
    // =========================
    public void OpenNewGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            mainMenuPanel.SetActive(false);
            newGamePanel.SetActive(true);
        }
        else
        {
            // Masuk ke game
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void CloseNew()
    {
        newGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void NewGame()
    {
        // Hapus save lama
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        // Masuk ke game
        SceneManager.LoadScene(gameSceneName);
    }

    // =========================
    // LOAD GAME
    // =========================

    public void OpenLoadGame()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        loadGamePanel.SetActive(true);
        exitConfirmPanel.SetActive(false);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("Save ditemukan. Memuat game...");

            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("Tidak ada save data.");
        }
    }

    public void CloseLoad()
    {
        loadGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // =========================
    // OPTIONS
    // =========================

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        loadGamePanel.SetActive(false);
        exitConfirmPanel.SetActive(false);
    }

    public void CloseOptions()
    {
        PlayerPrefs.Save();
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // =========================
    // EXIT
    // =========================

    public void OpenExitConfirmation()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        exitConfirmPanel.SetActive(true);
    }

    public void CancelExit()
    {
        exitConfirmPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari game.");

        Application.Quit();
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

    // =========================
    // LOAD OPTIONS
    // =========================

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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Load Game Panel")]
    [SerializeField] private GameObject loadGamePanel;

    [Header("Exit Confirmation Panel")]
    [SerializeField] private GameObject exitConfirmPanel;

    [Header("Options")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

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
        loadGamePanel.SetActive(false);
        exitConfirmPanel.SetActive(false);
    }

    // =========================
    // NEW GAME
    // =========================

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
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();

        Debug.Log("Master Volume: " + value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();

        Debug.Log("Music Volume: " + value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
        PlayerPrefs.Save();

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
        float masterVolume =
            PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

        float musicVolume =
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        float sfxVolume =
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        bool fullscreen =
            PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = fullscreen;

        Screen.fullScreen = fullscreen;
    }
}
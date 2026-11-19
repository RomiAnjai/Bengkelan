using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Credit Game Panel")]
    [SerializeField] private GameObject creditPanel;
    public bool isCredit;

    [Header("Exit Confirmation Panel")]
    [SerializeField] private GameObject exitConfirmPanel;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private int minJumlahNyangkut = 2;
    [SerializeField] private int maxJumlahNyangkut = 4;
    [SerializeField] private float minDurasiNyangkut = 0.35f;
    [SerializeField] private float maxDurasiNyangkut = 1.1f;
    [SerializeField] private float minKecepatanBar = 0.25f;
    [SerializeField] private float maxKecepatanBar = 0.65f;

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

    private bool isLoading = false;

    private void Start()
    {
        Time.timeScale = 1f;
        InitializeMenu();
        LoadOptions();
    }

    private void InitializeMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditPanel != null) creditPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (loadingSlider != null) loadingSlider.value = 0f;
        if (loadingText != null) loadingText.text = "0%";
        isCredit = false;
    }

    public void NewGame()
    {
        if (isLoading) return;

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("Belum ada progress. Memulai game baru...");
            PlayerPrefs.SetInt(SaveKey, 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Progress ditemukan. Melanjutkan game sebelumnya...");
        }

        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        isLoading = true;

        if (loadingSlider != null) loadingSlider.value = 0f;
        if (loadingText != null) loadingText.text = "0%";

        if (loadingPanel != null)
        {
            CanvasGroup canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            loadingPanel.SetActive(true);

            float fadeTimer = 0f;
            while (fadeTimer < fadeInDuration)
            {
                fadeTimer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(fadeTimer / fadeInDuration);
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditPanel != null) creditPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        int minCount = Mathf.Max(1, minJumlahNyangkut);
        int maxCount = Mathf.Max(minCount, maxJumlahNyangkut);
        int jumlahNyangkut = Random.Range(minCount, maxCount + 1);

        List<float> titikNyangkut = new List<float>();
        float batasAwal = 0.12f;
        float rentangBagian = 0.84f / jumlahNyangkut;

        for (int i = 0; i < jumlahNyangkut; i++)
        {
            float minTitik = batasAwal + (i * rentangBagian);
            float maxTitik = Mathf.Min(minTitik + (rentangBagian * 0.8f), 0.96f);
            titikNyangkut.Add(Random.Range(minTitik, maxTitik));
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;
        int indexNyangkut = 0;
        float kecepatanSaatIni = Random.Range(minKecepatanBar, maxKecepatanBar);

        while (displayedProgress < 1f || operation.progress < 0.9f)
        {
            float batasSceneAsli = operation.progress >= 0.9f ? 1f : Mathf.Min(0.92f, operation.progress / 0.9f);
            float targetLangkah = batasSceneAsli;

            if (indexNyangkut < titikNyangkut.Count)
            {
                targetLangkah = Mathf.Min(batasSceneAsli, titikNyangkut[indexNyangkut]);
            }

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetLangkah, Time.unscaledDeltaTime * kecepatanSaatIni);

            if (loadingSlider != null)
            {
                loadingSlider.value = displayedProgress;
            }

            if (loadingText != null)
            {
                loadingText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";
            }

            if (indexNyangkut < titikNyangkut.Count && Mathf.Abs(displayedProgress - titikNyangkut[indexNyangkut]) <= 0.001f)
            {
                float durasiJeda = Random.Range(minDurasiNyangkut, maxDurasiNyangkut);
                yield return new WaitForSecondsRealtime(durasiJeda);
                indexNyangkut++;
                kecepatanSaatIni = Random.Range(minKecepatanBar, maxKecepatanBar);
            }
            else
            {
                yield return null;
            }
        }

        if (loadingSlider != null) loadingSlider.value = 1f;
        if (loadingText != null) loadingText.text = "100%";

        yield return new WaitForSecondsRealtime(0.2f);
        operation.allowSceneActivation = true;
    }

    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        PlayerPrefs.Save();
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void CreditOption()
    {
        if (!isCredit)
        {
            if (creditPanel != null) creditPanel.SetActive(true);
            isCredit = true;
        }
        else
        {
            if (creditPanel != null) creditPanel.SetActive(false);
            isCredit = false;
        }
    }

    public void OpenExitConfirmation()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(true);
    }

    public void CancelExit()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari game.");
        Application.Quit();
    }

    public void SetMasterVolume(float value)
    {
        float safeValue = Mathf.Clamp(value, 0.0001f, 1f);
        if (audioMixer != null) audioMixer.SetFloat("MasterVolume", Mathf.Log10(safeValue) * 20f);
        PlayerPrefs.SetFloat(MasterVolumeKey, safeValue);
        Debug.Log("Master Volume: " + safeValue);
    }

    public void SetMusicVolume(float value)
    {
        float safeValue = Mathf.Clamp(value, 0.0001f, 1f);
        if (audioMixer != null) audioMixer.SetFloat("MusicVolume", Mathf.Log10(safeValue) * 20f);
        PlayerPrefs.SetFloat(MusicVolumeKey, safeValue);
        Debug.Log("Music Volume: " + safeValue);
    }

    public void SetSFXVolume(float value)
    {
        float safeValue = Mathf.Clamp(value, 0.0001f, 1f);
        if (audioMixer != null) audioMixer.SetFloat("SFXVolume", Mathf.Log10(safeValue) * 20f);
        PlayerPrefs.SetFloat(SFXVolumeKey, safeValue);
        Debug.Log("SFX Volume: " + safeValue);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadOptions()
    {
        float masterVolume = Mathf.Clamp(PlayerPrefs.GetFloat(MasterVolumeKey, 1f), 0.0001f, 1f);
        float musicVolume = Mathf.Clamp(PlayerPrefs.GetFloat(MusicVolumeKey, 1f), 0.0001f, 1f);
        float sfxVolume = Mathf.Clamp(PlayerPrefs.GetFloat(SFXVolumeKey, 1f), 0.0001f, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        Screen.fullScreen = fullscreen;

        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20f);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20f);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("Balance")]
    [SerializeField] private TMP_Text balanceText;

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitPanel;

    [Header("Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private int balance = 0;

    private const string BalanceKey = "CTAS_Balance";
    private const string MasterVolumeKey = "CTAS_MasterVolume";
    private const string FullscreenKey = "CTAS_Fullscreen";

    private void Start()
    {
        LoadPlayerData();
        LoadSettings();

        CloseAllPanels();

        UpdateBalanceUI();
    }

    // =========================================================
    // OBJECTIVE
    // =========================================================

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }

    // =========================================================
    // BALANCE
    // =========================================================

    public void SetBalance(int amount)
    {
        balance = amount;

        UpdateBalanceUI();
        SaveBalance();
    }

    public void AddMoney(int amount)
    {
        balance += amount;

        UpdateBalanceUI();
        SaveBalance();
    }

    public void RemoveMoney(int amount)
    {
        balance -= amount;

        if (balance < 0)
        {
            balance = 0;
        }

        UpdateBalanceUI();
        SaveBalance();
    }

    public int GetBalance()
    {
        return balance;
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = "Rp " + balance.ToString("N0");
        }
    }

    // =========================================================
    // INVENTORY
    // =========================================================

    public void OpenInventory()
    {
        CloseAllPanels();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    // =========================================================
    // SETTINGS
    // =========================================================

    public void OpenSettings()
    {
        CloseAllPanels();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // =========================================================
    // EXIT
    // =========================================================

    public void OpenExitPanel()
    {
        CloseAllPanels();

        if (exitPanel != null)
        {
            exitPanel.SetActive(true);
        }
    }

    public void CancelExit()
    {
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }
    }

    public void ExitToMainMenu()
    {
        SaveBalance();

        SceneManager.LoadScene("MainMenu");
    }

    // =========================================================
    // CLOSE ALL PANELS
    // =========================================================

    private void CloseAllPanels()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }
    }

    // =========================================================
    // SETTINGS - VOLUME
    // =========================================================

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    // =========================================================
    // SETTINGS - FULLSCREEN
    // =========================================================

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt(
            FullscreenKey,
            fullscreen ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    // =========================================================
    // SAVE / LOAD
    // =========================================================

    private void SaveBalance()
    {
        PlayerPrefs.SetInt(BalanceKey, balance);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        balance = PlayerPrefs.GetInt(BalanceKey, 150000);
    }

    private void LoadSettings()
    {
        float masterVolume =
            PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

        bool fullscreen =
            PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
        }

        Screen.fullScreen = fullscreen;
    }
}
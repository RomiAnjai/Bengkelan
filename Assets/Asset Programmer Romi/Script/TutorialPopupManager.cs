using UnityEngine;

public class TutorialPopupManager : MonoBehaviour
{
    public static TutorialPopupManager instance;

    [Header("UI Tutorial Awal")]
    public GameObject panelTutorial;
    public GameObject panelKedua;
    public GameObject tombolNext;
    public bool tutorialTerbuka = false;
    public bool tutorialKeduaTerbuka = false;

    private const string TutorialKey = "CTAS_SudahLihatTutorial";

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        panelTutorial.SetActive(false);
        panelKedua.SetActive(false);
        tombolNext.SetActive(false);
    }

    void Start()
    {
        bool sudahPernahLihat = PlayerPrefs.GetInt(TutorialKey, 0) == 1;

        if (!sudahPernahLihat)
        {
            TampilkanTutorial();
        }
        else
        {
            tutorialTerbuka = false;
            if (panelTutorial != null)
            {
                panelTutorial.SetActive(false);
                panelKedua.SetActive(false);
                tombolNext.SetActive(false);
            }
        }
    }

    public void TampilkanTutorial()
    {
        if (panelTutorial != null)
        {
            tutorialTerbuka = true;
            panelTutorial.SetActive(true);
            tombolNext.SetActive(true);
        }
    }

    public void NextTutorial()
    {
        if (panelTutorial != null && tutorialKeduaTerbuka == false)
        {
            panelTutorial.SetActive(false);
            panelKedua.SetActive(true);
            tutorialKeduaTerbuka = true;
        } else if (tutorialKeduaTerbuka == true)
        {
            panelKedua.SetActive(false);
            panelTutorial.SetActive(false);
            tombolNext.SetActive(false);
            tutorialTerbuka = false;
        }

        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset Status Tutorial")]
    public void ResetStatusTutorial()
    {
        PlayerPrefs.DeleteKey(TutorialKey);
        PlayerPrefs.Save();
        Debug.Log("Status tutorial berhasil di-reset. Pop-up akan muncul lagi saat Play berikutnya.");
    }
}
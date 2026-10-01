using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ObjectiveHUD : MonoBehaviour
{
    public static ObjectiveHUD instance;

    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private RectTransform panelObjektif;
    [SerializeField] private float jarakSlide = 600f;
    [SerializeField] private float durasiSlide = 0.4f;
    [SerializeField] private float jedaSebelumMasukAwal = 0.5f;

    [Header("Balance")]
    [SerializeField] private TMP_Text balanceText;

    [Header("Database")]
    public DatabaseObjektif databaseObjektif;

    [Header("Audio")]
    public AudioSource audioUI;
    public AudioClip sfxObjektifBaru;
    public AudioClip sfxWin;

    private int balance = 0;
    private const string BalanceKey = "CTAS_Balance";
    private bool sudahSelesai = false;
    private Vector2 posisiNormalLayar;
    private Coroutine animasiRoutine;
    private bool banBaru = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateBalanceUI();

        if (panelObjektif == null && objectiveText != null)
        {
            panelObjektif = objectiveText.GetComponentInParent<RectTransform>();
        }

        if (panelObjektif != null)
        {
            posisiNormalLayar = panelObjektif.anchoredPosition;
            panelObjektif.anchoredPosition = posisiNormalLayar + new Vector2(-jarakSlide, 0f);

            if (animasiRoutine != null) StopCoroutine(animasiRoutine);
            animasiRoutine = StartCoroutine(ProsesSlideMasukAwal());
        }
    }

    public void SetBalance(int amount)
    {
        balance = amount;
        UpdateBalanceUI();
        SaveBalance();
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        PlaySFX(sfxWin); // PUTAR SFX
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

    private void SaveBalance()
    {
        PlayerPrefs.SetInt(BalanceKey, balance);
        PlayerPrefs.Save();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioUI != null && clip != null)
        {
            audioUI.PlayOneShot(clip);
        }
    }

    public void TampilkanPesanManual(string pesan)
    {
        if (sudahSelesai) return;

        string teksFormat = "- " + pesan;

        if (panelObjektif != null)
        {
            if (animasiRoutine != null) StopCoroutine(animasiRoutine);
            animasiRoutine = StartCoroutine(ProsesGantiTeksDenganSlide(teksFormat));
        }
        else if (objectiveText != null)
        {
            objectiveText.text = teksFormat;
        }
    }

    public void UpdateHUD(JenisMotor jenisMotor, string namaMasalah, int indexLangkah)
    {
        string teksBaru = "";
        if (sudahSelesai) return;
        if (databaseObjektif == null) return;
        if (banBaru == false)
        {
            teksBaru = databaseObjektif.AmbilTeksObjektif(jenisMotor, namaMasalah, indexLangkah);
            string teksFormat = "- " + teksBaru;
            PlaySFX(sfxObjektifBaru);
            if (panelObjektif != null)
            {
                if (animasiRoutine != null) StopCoroutine(animasiRoutine);
                animasiRoutine = StartCoroutine(ProsesGantiTeksDenganSlide(teksFormat));
            }
            else if (objectiveText != null)
            {
                objectiveText.text = teksFormat;
            }
        }
        if (indexLangkah == 6) banBaru = true;

        if (teksBaru == "Objektif Selesai!")
        {
            SembunyikanHUD();
            return;
        }
    }

    public void SembunyikanHUD()
    {
        if (sudahSelesai) return;
        sudahSelesai = true;

        if (panelObjektif != null)
        {
            if (animasiRoutine != null) StopCoroutine(animasiRoutine);
            animasiRoutine = StartCoroutine(ProsesSlideKeluarPermanen());
        }
    }

    private IEnumerator ProsesSlideMasukAwal()
    {
        yield return new WaitForSeconds(jedaSebelumMasukAwal);

        Vector2 posisiAwalLuar = panelObjektif.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < durasiSlide)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiSlide);
            panelObjektif.anchoredPosition = Vector2.Lerp(posisiAwalLuar, posisiNormalLayar, t);
            yield return null;
        }

        panelObjektif.anchoredPosition = posisiNormalLayar;
    }

    private IEnumerator ProsesGantiTeksDenganSlide(string teksBaru)
    {
        Vector2 posisiAwal = panelObjektif.anchoredPosition;
        Vector2 posisiLuar = posisiNormalLayar + new Vector2(-jarakSlide, 0f);
        float elapsed = 0f;

        while (elapsed < durasiSlide)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiSlide);
            panelObjektif.anchoredPosition = Vector2.Lerp(posisiAwal, posisiLuar, t);
            yield return null;
        }

        panelObjektif.anchoredPosition = posisiLuar;

        if (objectiveText != null)
        {
            objectiveText.text = teksBaru;
        }

        yield return new WaitForSeconds(0.1f);

        elapsed = 0f;
        while (elapsed < durasiSlide)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiSlide);
            panelObjektif.anchoredPosition = Vector2.Lerp(posisiLuar, posisiNormalLayar, t);
            yield return null;
        }

        panelObjektif.anchoredPosition = posisiNormalLayar;
    }

    private IEnumerator ProsesSlideKeluarPermanen()
    {
        Vector2 posisiAwal = panelObjektif.anchoredPosition;
        Vector2 posisiLuar = posisiNormalLayar + new Vector2(-jarakSlide, 0f);
        float elapsed = 0f;

        while (elapsed < durasiSlide)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiSlide);
            panelObjektif.anchoredPosition = Vector2.Lerp(posisiAwal, posisiLuar, t);
            yield return null;
        }

        panelObjektif.anchoredPosition = posisiLuar;
        panelObjektif.gameObject.SetActive(false);
    }
}
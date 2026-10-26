using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum GameState
{
    PemainKeliling,
    TungguPesanan,
    MotorBelumDatang,
    MotorMasukBengkel,
    MotorSedangServis,
    MotorSelesai
}

[System.Serializable]
public class DataPesanan
{
    public string namaMasalah;
    public JenisMotor jenisMotor;
    public int indexMasalahSequence;
    public GameObject prefabMotor;
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;
    public GameState currentState;

    [Header("Sistem Pesanan HP")]
    public List<DataPesanan> daftarPesanan;
    public TMP_Dropdown dropdownPesananUI;
    public GameObject acceptbutton;
    public GameObject dropdown;
    public GameObject tombolInspeksi;

    private DataPesanan pesananAktif;

    [Header("Sistem Inspeksi")]
    public GameObject tombolSelesaiInspeksiUI;
    public bool sudahDiinspeksi = false;
    private bool sequenceSudahMulai = false;

    [Header("Spawn & Posisi")]
    public Transform posisiSpawn;
    public Transform posisiParkir;
    public Transform posisiKeluar;
    public float jedaSebelumJalan = 2f;
    public float waktuPerjalanan = 3f;
    public float waktuKeluar = 2f;

    public bool sudahAmbilHP = false;
    public TutorialHighlight[] tutorLight;

    [Header("Pengaturan Animasi HP")]
    public GameObject hpTangan;
    public RectTransform hpUIPanel;
    public float jarakSlideHP = 1000f;
    public float durasiSlideHP = 0.4f;

    [Header("Audio General")]
    public AudioSource audioBGM;
    public AudioSource audioHP;
    public AudioClip sfxOrderMasuk;
    public bool hpSedangBuka = false;

    private bool rewardSudahDiberikan = false;
    private Vector2 posisiHPTengah;
    private Coroutine animasiHPRoutine;
    private GameObject motorAktif;

    void Awake()
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

    void Start()
    {
        currentState = GameState.PemainKeliling;
        TutorialHighlight.NyalakanBerdasarkanID("HP Awal");

        if (hpTangan != null) hpTangan.SetActive(false);
        if (tombolSelesaiInspeksiUI != null) tombolSelesaiInspeksiUI.SetActive(false);

        if (hpUIPanel != null)
        {
            posisiHPTengah = hpUIPanel.anchoredPosition;
            hpUIPanel.anchoredPosition = posisiHPTengah + new Vector2(0f, -jarakSlideHP);
            hpUIPanel.gameObject.SetActive(false);
        }

        SiapkanDropdownUI();

        if (tombolInspeksi != null)
        {
            tombolInspeksi.SetActive(false);
        }
    }

    void SiapkanDropdownUI()
    {
        if (dropdownPesananUI != null)
        {
            dropdownPesananUI.ClearOptions();
            dropdownPesananUI.AddOptions(new List<string> { "Pesanan Servis Motor Umum" });
        }
    }

    void Update()
    {
        bool stateMengizinkanHP = (currentState == GameState.PemainKeliling ||
                                    currentState == GameState.TungguPesanan ||
                                    currentState == GameState.MotorSelesai);

        if (sudahAmbilHP && Input.GetKeyDown(KeyCode.Tab))
        {
            if (stateMengizinkanHP || hpSedangBuka)
            {
                ToggleHP();
            }
        }

        switch (currentState)
        {
            case GameState.PemainKeliling:
                break;
            case GameState.TungguPesanan:
                break;
            case GameState.MotorBelumDatang:
                if (pesananAktif != null && pesananAktif.prefabMotor != null && posisiSpawn != null && posisiParkir != null)
                {
                    motorAktif = Instantiate(pesananAktif.prefabMotor, posisiSpawn.position, posisiSpawn.rotation);
                    currentState = GameState.MotorMasukBengkel;
                    StartCoroutine(ProsesMotorParkir(motorAktif));
                }
                break;
            case GameState.MotorMasukBengkel:
                break;
            case GameState.MotorSedangServis:
                break;
            case GameState.MotorSelesai:
                if (!rewardSudahDiberikan)
                {
                    rewardSudahDiberikan = true;

                    if (ObjectiveHUD.instance != null)
                    {
                        ObjectiveHUD.instance.AddMoney(60000);
                        ObjectiveHUD.instance.TampilkanPesanManual("Selamat! Anda mendapatkan Rp 60.000\nMotor akan segera dikembalikan.");
                    }

                    StartCoroutine(ProsesMotorKeluar());
                }
                break;
        }
    }

    public void ToggleHP()
    {
        hpSedangBuka = !hpSedangBuka;
        if (hpTangan != null) hpTangan.SetActive(hpSedangBuka);

        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null)
        {
            mc.KunciKursor(!hpSedangBuka);
        }

        if (hpUIPanel != null)
        {
            if (animasiHPRoutine != null) StopCoroutine(animasiHPRoutine);
            animasiHPRoutine = StartCoroutine(ProsesSlideHP(hpSedangBuka));
        }
    }

    private IEnumerator ProsesSlideHP(bool buka)
    {
        if (buka) hpUIPanel.gameObject.SetActive(true);

        Vector2 posisiAwal = hpUIPanel.anchoredPosition;
        Vector2 posisiTujuan = buka ? posisiHPTengah : posisiHPTengah + new Vector2(0f, -jarakSlideHP);

        float elapsed = 0f;
        while (elapsed < durasiSlideHP)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiSlideHP);
            hpUIPanel.anchoredPosition = Vector2.Lerp(posisiAwal, posisiTujuan, t);
            yield return null;
        }

        hpUIPanel.anchoredPosition = posisiTujuan;
        if (!buka) hpUIPanel.gameObject.SetActive(false);
    }

    public void AmbilHP()
    {
        if (sudahAmbilHP) return;

        sudahAmbilHP = true;
        TutorialHighlight.MatikanBerdasarkanID("HP Awal");
        currentState = GameState.TungguPesanan;

        if (ObjectiveHUD.instance != null)
        {
            ObjectiveHUD.instance.TampilkanPesanManual("Tekan TAB untuk buka HP dan terima pesanan");
        }
    }

    public void TerimaPesanan()
    {
        if (currentState == GameState.TungguPesanan)
        {
            if (audioHP != null && sfxOrderMasuk != null)
            {
                audioHP.PlayOneShot(sfxOrderMasuk);
            }

            if (daftarPesanan.Count > 0)
            {
                pesananAktif = daftarPesanan[Random.Range(0, daftarPesanan.Count)];
            }

            currentState = GameState.MotorBelumDatang;
            sudahDiinspeksi = false;
            sequenceSudahMulai = false; 

            if (ObjectiveHUD.instance != null)
            {
                ObjectiveHUD.instance.TampilkanPesanManual("Tunggu pelanggan parkir");
            }

            if (hpSedangBuka)
            {
                ToggleHP();
            }
        }
    }

    private IEnumerator ProsesMotorParkir(GameObject motor)
    {
        yield return new WaitForSeconds(jedaSebelumJalan);

        float waktuBerjalan = 0f;
        Vector3 posisiAwal = motor.transform.position;
        Quaternion rotasiAwal = motor.transform.rotation;

        while (waktuBerjalan < waktuPerjalanan)
        {
            waktuBerjalan += Time.deltaTime;
            float persentase = waktuBerjalan / waktuPerjalanan;
            float smoothT = Mathf.SmoothStep(0f, 1f, persentase);

            motor.transform.position = Vector3.Lerp(posisiAwal, posisiParkir.position, smoothT);
            motor.transform.rotation = Quaternion.Lerp(rotasiAwal, posisiParkir.rotation, smoothT);
            yield return null;
        }

        motor.transform.position = posisiParkir.position;
        motor.transform.rotation = posisiParkir.rotation;

        currentState = GameState.MotorSedangServis;
    }

    private IEnumerator ProsesMotorKeluar()
    {
        yield return new WaitForSeconds(2f);

        if (motorAktif != null)
        {
            float elapsed = 0f;
            Vector3 startPos = motorAktif.transform.position;
            Vector3 targetPos = posisiKeluar != null ? posisiKeluar.position : startPos + (motorAktif.transform.right * 25f) + (Vector3.up * 10f);
            
            Vector3 startRot = motorAktif.transform.eulerAngles;
            Vector3 targetRot = startRot + new Vector3(Random.Range(360f, 720f), Random.Range(360f, 720f), Random.Range(360f, 720f));

            while (elapsed < waktuKeluar)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / waktuKeluar;
                
                float parabola = Mathf.Sin(t * Mathf.PI) * 7f;
                
                motorAktif.transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, parabola, 0);
                motorAktif.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, targetRot, t));
                
                yield return null;
            }

            Destroy(motorAktif);
        }

        motorAktif = null;
        rewardSudahDiberikan = false;
        pesananAktif = null;
        sudahDiinspeksi = false;
        currentState = GameState.TungguPesanan;

        if (dropdown != null) dropdown.SetActive(true);
        if (acceptbutton != null) acceptbutton.SetActive(true);

        if (ObjectiveHUD.instance != null)
        {
            ObjectiveHUD.instance.TampilkanPesanManual("Tekan TAB untuk buka HP dan terima pesanan baru");
        }
    }

    public void ProsesKlikMotor()
    {
        if (!sudahDiinspeksi)
        {
            MulaiInspeksi();
        }
        else
        {
            MulaiServis();
        }
    }

    private void MulaiInspeksi()
    {
        if (tombolInspeksi != null)
        {
            tombolInspeksi.SetActive(true);
        }

        if (motorAktif != null)
        {
            TutorialHighlight.NyalakanInspeksiMotor(motorAktif);
        }

        if (InteraksiMotor.instance != null)
        {
            InteraksiMotor.instance.FokusKeInspeksi();
        }

        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null)
        {
            mc.KunciKursor(false);
        }

        if (tombolSelesaiInspeksiUI != null) tombolSelesaiInspeksiUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelesaiInspeksi()
    {
        if (tombolInspeksi != null)
        {
            tombolInspeksi.SetActive(false);
        }

        if (motorAktif != null)
        {
            TutorialHighlight.MatikanInspeksiMotor(motorAktif);
        }

        if (tombolSelesaiInspeksiUI != null) tombolSelesaiInspeksiUI.SetActive(false);

        sudahDiinspeksi = true;

        if (InteraksiMotor.instance != null)
        {
            InteraksiMotor.instance.KembaliKePemain();
        }

        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null)
        {
            mc.KunciKursor(true);
        }
    }

    public void MulaiServis()
    {
        if (!sequenceSudahMulai && SequenceService.instance != null && pesananAktif != null)
        {
            SequenceService.instance.motorYangSedangDiservis = pesananAktif.jenisMotor;
            SequenceService.instance.MulaiServis(pesananAktif.indexMasalahSequence);
            sequenceSudahMulai = true;
        }

        InteraksiMotor.instance.FokusKeMotor();
        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null)
        {
            mc.KunciKursor(true);
        }
    }
}
using UnityEngine;
using TMPro;

public class InteraksiPemain : MonoBehaviour
{
    [Header("Referensi")]
    public Camera kameraPemain;
    public Transform titikPegangan;
    public TextMeshProUGUI teksNamaUI;
    public GameObject crosshairUI;

    [Header("Referensi Sistem Oli")]
    public string tagBotolOliRak = "BotolOliRak";

    [Header("Pengaturan UI Kursor")]
    public Vector2 offsetTeksKursor = new Vector2(0f, -35f);

    [Header("Pengaturan Interaksi")]
    public float jarakJangkauan = 3f;
    public float kekuatanTarik = 10f;

    [Header("Audio Interaksi")]
    public AudioSource audioInteraksi;
    public AudioClip sfxPickup;
    public AudioClip sfxDrop;

    private Rigidbody barangDipegang;
    private DataSparepart partDisorot;
    private Baut bautDisorot;
    private InteraksiMotor motorDisorot;
    private StationInteraction stationDisorot;
    private IInteraksi objekInteraksiDisorot;
    private BautOli bautOliDisorot;
    private botolOli botolOliDisorot;
    private penempatanItem areaItemDisorot;
    private GameObject botolRakDisorot;

    private bool sudahAmbilBotolOli = false;
    private RectTransform rectTeksNama;
    private Vector3 posisiAwalTeksUI;

    void Start()
    {
        if (teksNamaUI != null)
        {
            rectTeksNama = teksNamaUI.GetComponent<RectTransform>();
            posisiAwalTeksUI = rectTeksNama.position;
            teksNamaUI.raycastTarget = false;
        }
    }

    void Update()
    {
        if (SequenceService.instance != null && SequenceService.instance.sedangTransisi)
        {
            if (teksNamaUI != null) teksNamaUI.text = "";
            if (crosshairUI != null) crosshairUI.SetActive(false);
            return;
        }

        AturModeBidik();
        UpdatePosisiTeksUI();
        SorotBarang();
        CekInputPegang();
    }

    void FixedUpdate()
    {
        if (barangDipegang != null)
        {
            Vector3 arahTarik = (titikPegangan.position - barangDipegang.position);
            barangDipegang.linearVelocity = arahTarik * kekuatanTarik;
        }
    }

    void AturModeBidik()
    {
        DetailLangkah langkah = null;
        if (SequenceService.instance != null)
        {
            langkah = SequenceService.instance.GetLangkahSaatIni();
        }

        bool motorFokus = InteraksiMotor.instance != null && InteraksiMotor.instance.sedangFokus;
        bool mejaFokus = StationInteraction.instance != null && StationInteraction.instance.sedangFokusMeja;
        bool hpBuka = GameManagerScript.instance != null && GameManagerScript.instance.hpSedangBuka;

        bool aktifkanCrosshair = true;

        if (hpBuka)
        {
            aktifkanCrosshair = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (motorFokus)
        {
            if (langkah != null)
            {
                aktifkanCrosshair = langkah.pakaiCrosshair;
            }
            if (aktifkanCrosshair)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else if (mejaFokus)
        {
            aktifkanCrosshair = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            aktifkanCrosshair = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(aktifkanCrosshair);
        }
    }

    void UpdatePosisiTeksUI()
    {
        if (rectTeksNama == null) return;

        bool modeCrosshair = crosshairUI != null && crosshairUI.activeSelf;

        if (modeCrosshair)
        {
            rectTeksNama.position = posisiAwalTeksUI;
        }
        else
        {
            rectTeksNama.position = Input.mousePosition + (Vector3)offsetTeksKursor;
        }
    }

    void ResetSemuaSorotan()
    {
        partDisorot = null;
        bautDisorot = null;
        motorDisorot = null;
        stationDisorot = null;
        objekInteraksiDisorot = null;
        bautOliDisorot = null;
        botolOliDisorot = null;
        areaItemDisorot = null;
        botolRakDisorot = null;
    }

    void SorotBarang()
    {
        Ray ray;
        if (crosshairUI != null && crosshairUI.activeSelf)
        {
            ray = kameraPemain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }
        else
        {
            ray = kameraPemain.ScreenPointToRay(Input.mousePosition);
        }

        RaycastHit hit;
        DetailLangkah langkah = null;
        if (SequenceService.instance != null)
        {
            langkah = SequenceService.instance.GetLangkahSaatIni();
        }

        ResetSemuaSorotan();

        if (barangDipegang == null && Physics.Raycast(ray, out hit, jarakJangkauan, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (!sudahAmbilBotolOli && hit.collider.CompareTag(tagBotolOliRak))
            {
                botolRakDisorot = hit.collider.gameObject;
                if (teksNamaUI != null) teksNamaUI.text = "Ambil Botol Oli";
                return;
            }

            IInteraksi cekInteraksi = hit.collider.GetComponentInParent<IInteraksi>();
            if (cekInteraksi != null && !string.IsNullOrEmpty(cekInteraksi.DapatkanNamaPetunjuk()))
            {
                objekInteraksiDisorot = cekInteraksi;
                if (teksNamaUI != null) teksNamaUI.text = objekInteraksiDisorot.DapatkanNamaPetunjuk();
                return;
            }

            if (InteraksiMotor.instance != null && !InteraksiMotor.instance.sedangFokus)
            {
                InteraksiMotor cekMotor = hit.collider.GetComponentInParent<InteraksiMotor>();
                if (cekMotor != null)
                {
                    if (SequenceService.instance != null && SequenceService.instance.indexLangkahSaatIni <= 1)
                    {
                        motorDisorot = cekMotor;
                        if (teksNamaUI != null) teksNamaUI.text = "Mulai Servis";
                        return;
                    }
                    else if (sudahAmbilBotolOli)
                    {
                        motorDisorot = cekMotor;
                        if (teksNamaUI != null) teksNamaUI.text = "Fokus & Masukkan Oli";
                        return;
                    }
                }
            }

            Baut cekBaut = hit.collider.GetComponent<Baut>();
            if (cekBaut != null)
            {
                if (langkah != null && langkah.jenisAksi == cekBaut.tipeAksiDibutuhkan)
                {
                    bautDisorot = cekBaut;
                    if (teksNamaUI != null) teksNamaUI.text = bautDisorot.namaObjek;
                    return;
                }
            }

            DataSparepart cekPart = hit.collider.GetComponent<DataSparepart>();
            if (cekPart == null) cekPart = hit.collider.GetComponentInParent<DataSparepart>();
            if (cekPart != null)
            {
                if (cekPart.bisaDiambil)
                {
                    partDisorot = cekPart;
                    if (teksNamaUI != null) teksNamaUI.text = partDisorot.namaObjek;
                    return;
                }
            }

            StationInteraction cekStation = hit.collider.GetComponentInParent<StationInteraction>();
            if (cekStation != null)
            {
                if ((cekStation.CompareTag("Station") || hit.collider.CompareTag("Station")) && cekStation.BisaInteraksi())
                {
                    stationDisorot = cekStation;
                    if (teksNamaUI != null) teksNamaUI.text = "Meja Kerja";
                    return;
                }
            }

            BautOli cekBautOli = hit.collider.GetComponent<BautOli>();
            if (cekBautOli == null) cekBautOli = hit.collider.GetComponentInParent<BautOli>();
            if (cekBautOli != null)
            {
                bautOliDisorot = cekBautOli;
                if (teksNamaUI != null) teksNamaUI.text = "Tahan untuk Putar Baut";
                return;
            }

            botolOli cekbotolOli = hit.collider.GetComponent<botolOli>();
            if (cekbotolOli == null) cekbotolOli = hit.collider.GetComponentInParent<botolOli>();
            if (cekbotolOli != null)
            {
                botolOliDisorot = cekbotolOli;
                if (teksNamaUI != null) teksNamaUI.text = "Tahan untuk Tuang Oli";
                return;
            }

            penempatanItem cekAreaItem = hit.collider.GetComponent<penempatanItem>();
            if (cekAreaItem == null) cekAreaItem = hit.collider.GetComponentInParent<penempatanItem>();
            if (cekAreaItem != null)
            {
                areaItemDisorot = cekAreaItem;
                if (teksNamaUI != null)
                {
                    teksNamaUI.text = cekAreaItem.isTerpasang ? "Klik untuk Ambil Item" : "Klik untuk Pasang Item";
                }
                return;
            }
        }

        if (teksNamaUI != null) teksNamaUI.text = "";
    }

    void CekInputPegang()
    {
        if (Input.GetMouseButtonDown(0) && botolRakDisorot != null)
        {
            sudahAmbilBotolOli = true;
            botolRakDisorot.SetActive(false);
            if (audioInteraksi != null && sfxPickup != null) audioInteraksi.PlayOneShot(sfxPickup);
            botolRakDisorot = null;
            return;
        }

        if (Input.GetMouseButtonDown(0) && objekInteraksiDisorot != null)
        {
            objekInteraksiDisorot.EksekusiAksi();
            return;
        }

        if (Input.GetMouseButtonDown(0) && stationDisorot != null)
        {
            stationDisorot.InteraksiMeja();
            stationDisorot = null;
            return;
        }

        if (Input.GetMouseButtonDown(0) && motorDisorot != null)
        {
            if (SequenceService.instance != null && SequenceService.instance.indexLangkahSaatIni <= 1)
            {
                GameManagerScript.instance.MulaiServis();
            }
            else if (sudahAmbilBotolOli)
            {
                sudahAmbilBotolOli = false;

                if (SequenceService.instance != null)
                {
                    DetailLangkah langkahSekarang = SequenceService.instance.GetLangkahSaatIni();
                    if (langkahSekarang != null && langkahSekarang.namaLangkah.ToLower().Contains("ambil botol"))
                    {
                        SequenceService.instance.MajukanLangkahTanpaJeda();
                    }
                }

                if (InteraksiMotor.instance != null)
                {
                    InteraksiMotor.instance.FokusKeMotorPaksa();
                }

                if (OilChangeManager.instance != null)
                {
                    OilChangeManager.instance.MunculkanBotolDiMotor();
                }
            }
            motorDisorot = null;
            return;
        }

        if (Input.GetMouseButton(0) && bautDisorot != null)
        {
            bautDisorot.ProsesInteraksi();
        }

        if (Input.GetMouseButtonDown(0) && partDisorot != null)
        {
            if (partDisorot.bisaDiambil)
            {
                Rigidbody rbTarget = partDisorot.GetComponent<Rigidbody>();
                if (rbTarget == null)
                {
                    rbTarget = partDisorot.GetComponentInChildren<Rigidbody>();
                }
                AmbilBarang(rbTarget, partDisorot.gameObject.tag);
            }
        }

        if (Input.GetMouseButton(0) && bautOliDisorot != null)
        {
            bautOliDisorot.ProsesInteraksi();
        }

        if (Input.GetMouseButton(0) && botolOliDisorot != null)
        {
            botolOliDisorot.ProsesInteraksi();
        }

        if (Input.GetMouseButtonDown(0) && areaItemDisorot != null)
        {
            if (!areaItemDisorot.isTerpasang)
            {
                areaItemDisorot.PasangItem();
            }
            else
            {
                areaItemDisorot.AmbilItem();
            }
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (bautOliDisorot != null) bautOliDisorot.HentikanInteraksiManual();
            if (botolOliDisorot != null) botolOliDisorot.HentikanInteraksi();

            if (barangDipegang != null)
            {
                LepasBarang();
            }
        }
    }

    void AmbilBarang(Rigidbody rb, string partTag)
    {
        if (rb == null) return;

        barangDipegang = rb;
        barangDipegang.isKinematic = false;
        barangDipegang.useGravity = false;
        barangDipegang.linearDamping = 10f;
        barangDipegang.angularDamping = 10f;

        if (audioInteraksi != null && sfxPickup != null) audioInteraksi.PlayOneShot(sfxPickup);

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah != null && langkah.jenisAksi == TipeAksi.LepasBanLuar && partTag == langkah.targetPartTag)
        {
            SequenceService.instance.SelesaikanLangkah();
        }
    }

    void LepasBarang()
    {
        if (audioInteraksi != null && sfxDrop != null) audioInteraksi.PlayOneShot(sfxDrop);

        barangDipegang.useGravity = true;
        barangDipegang.linearDamping = 0f;
        barangDipegang.angularDamping = 0.05f;
        barangDipegang = null;
    }
}
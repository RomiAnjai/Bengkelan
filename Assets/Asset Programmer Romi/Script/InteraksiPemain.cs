using UnityEngine;
using TMPro;

public class InteraksiPemain : MonoBehaviour
{
    [Header("Referensi")]
    public Camera kameraPemain;
    public Transform titikPegangan;
    public TextMeshProUGUI teksNamaUI;
    public GameObject crosshairUI; 

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

    void Update()
    {
        if (SequenceService.instance != null && SequenceService.instance.sedangTransisi)
        {
            teksNamaUI.text = "";
            if (crosshairUI != null) crosshairUI.SetActive(false);
            return;
        }

        AturModeBidik();
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

        if (barangDipegang == null && Physics.Raycast(ray, out hit, jarakJangkauan, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            IInteraksi cekInteraksi = hit.collider.GetComponentInParent<IInteraksi>();
            if (cekInteraksi != null && !string.IsNullOrEmpty(cekInteraksi.DapatkanNamaPetunjuk()))
            {
                objekInteraksiDisorot = cekInteraksi;
                partDisorot = null;
                bautDisorot = null;
                motorDisorot = null;
                stationDisorot = null;
                if (teksNamaUI != null) teksNamaUI.text = objekInteraksiDisorot.DapatkanNamaPetunjuk();
                return;
            }

            if (InteraksiMotor.instance != null && !InteraksiMotor.instance.sedangFokus)
            {
                if (SequenceService.instance != null && SequenceService.instance.indexLangkahSaatIni <= 1)
                {
                    InteraksiMotor cekMotor = hit.collider.GetComponentInParent<InteraksiMotor>();
                    if (cekMotor != null)
                    {
                        motorDisorot = cekMotor;
                        bautDisorot = null;
                        partDisorot = null;
                        stationDisorot = null;
                        objekInteraksiDisorot = null;
                        if (teksNamaUI != null) teksNamaUI.text = "Mulai Servis";
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
                    partDisorot = null;
                    motorDisorot = null;
                    stationDisorot = null;
                    objekInteraksiDisorot = null;
                    if (teksNamaUI != null) teksNamaUI.text = bautDisorot.namaObjek;
                    return;
                }
            }

            DataSparepart cekPart = hit.collider.GetComponent<DataSparepart>();
            if (cekPart != null)
            {
                if (cekPart.bisaDiambil)
                {
                    partDisorot = cekPart;
                    bautDisorot = null;
                    motorDisorot = null;
                    stationDisorot = null;
                    objekInteraksiDisorot = null;
                    if (teksNamaUI != null) teksNamaUI.text = partDisorot.namaObjek;
                    return;
                }
            }

            StationInteraction cekStation = hit.collider.GetComponentInParent<StationInteraction>();
            if (cekStation != null)
            {
                if ((cekStation.CompareTag("Station") || hit.collider.CompareTag("Station")) && cekStation.BisaInteraksi())
                {
                    partDisorot = null;
                    bautDisorot = null;
                    motorDisorot = null;
                    stationDisorot = cekStation;
                    objekInteraksiDisorot = null;
                    if (teksNamaUI != null) teksNamaUI.text = "Meja Kerja";
                    return;
                }
            }

            BautOli cekBautOli = hit.collider.GetComponent<BautOli>();
            if (cekBautOli != null)
            {
                bautOliDisorot = cekBautOli;
                partDisorot = null; bautDisorot = null; motorDisorot = null; stationDisorot = null; objekInteraksiDisorot = null; botolOliDisorot = null; areaItemDisorot = null;
                if (teksNamaUI != null) teksNamaUI.text = "Tahan untuk Putar Baut";
                return;
            }

            botolOli cekbotolOli = hit.collider.GetComponent<botolOli>();
            if (cekbotolOli == null) cekbotolOli = hit.collider.GetComponentInParent<botolOli>();
            if (cekbotolOli != null)
            {
                botolOliDisorot = cekbotolOli;
                partDisorot = null; bautDisorot = null; motorDisorot = null; stationDisorot = null; objekInteraksiDisorot = null; bautOliDisorot = null; areaItemDisorot = null;
                if (teksNamaUI != null) teksNamaUI.text = "Tahan untuk Tuang Oli";
                return;
            }

            penempatanItem cekAreaItem = hit.collider.GetComponent<penempatanItem>();
            if (cekAreaItem != null)
            {
                areaItemDisorot = cekAreaItem;
                partDisorot = null; bautDisorot = null; motorDisorot = null; stationDisorot = null; objekInteraksiDisorot = null; bautOliDisorot = null; botolOliDisorot = null;
                if (teksNamaUI != null) teksNamaUI.text = "Klik untuk Pasang/Ambil Item";
                return;
            }
        }

        partDisorot = null;
        bautDisorot = null;
        motorDisorot = null;
        stationDisorot = null;
        objekInteraksiDisorot = null;
        bautOliDisorot = null;     // Tambahan baru
        botolOliDisorot = null;    // Tambahan baru
        areaItemDisorot = null;    // Tambahan baru
        if (teksNamaUI != null) teksNamaUI.text = "";
    }

    void CekInputPegang()
    {
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
            GameManagerScript.instance.MulaiServis();
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

        if (Input.GetMouseButtonUp(0) && barangDipegang != null)
        {
            LepasBarang();
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
            areaItemDisorot.PasangItem(); 
            // Catatan: Jika ingin membuat sistem ambil/pasang satu tombol, 
            // kamu bisa memodifikasi logika if/else sederhana di sini nantinya.
            return;
        }
        // --- INPUT LEPAS KLIK (SISTEM OLI & LAMA) ---
        if (Input.GetMouseButtonUp(0))
        {
            // Hentikan animasi/suara baut oli dan botol oli jika klik dilepas
            if (bautOliDisorot != null) bautOliDisorot.HentikanInteraksiManual();
            if (botolOliDisorot != null) botolOliDisorot.HentikanInteraksi();

            // Kode lamamu untuk melepas barang yang dipegang dengan Rigidbody
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

        if (audioInteraksi != null && sfxPickup != null) audioInteraksi.PlayOneShot(sfxPickup); // SFX AMBIL

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah != null && langkah.jenisAksi == TipeAksi.LepasBanLuar && partTag == langkah.targetPartTag)
        {
            SequenceService.instance.SelesaikanLangkah();
        }
    }

    void LepasBarang()
    {
        if (audioInteraksi != null && sfxDrop != null) audioInteraksi.PlayOneShot(sfxDrop); // SFX LEPAS
        
        barangDipegang.useGravity = true;
        barangDipegang.linearDamping = 0f;
        barangDipegang.angularDamping = 0.05f;
        barangDipegang = null;
    }
}
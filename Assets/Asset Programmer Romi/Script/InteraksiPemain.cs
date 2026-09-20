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

    private Rigidbody barangDipegang;
    private DataSparepart partDisorot;
    private Baut bautDisorot;
    private InteraksiMotor motorDisorot;
    private StationInteraction stationDisorot;
    private IInteraksi objekInteraksiDisorot;

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
        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        bool aktifkanCrosshair = true;
        
        bool motorFokus = InteraksiMotor.instance != null && InteraksiMotor.instance.sedangFokus;
        bool mejaFokus = StationInteraction.instance != null && StationInteraction.instance.sedangFokusMeja;

        if (motorFokus && langkah != null)
        {
            aktifkanCrosshair = langkah.pakaiCrosshair;
        }
        
        if (mejaFokus)
        {
            aktifkanCrosshair = false;
        }

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(aktifkanCrosshair);
        }

        if (motorFokus)
        {
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();

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
                teksNamaUI.text = objekInteraksiDisorot.DapatkanNamaPetunjuk();
                return;
            }

            if (InteraksiMotor.instance != null && !InteraksiMotor.instance.sedangFokus)
            {
                if (SequenceService.instance.indexLangkahSaatIni <= 1)
                {
                    InteraksiMotor cekMotor = hit.collider.GetComponentInParent<InteraksiMotor>();
                    if (cekMotor != null)
                    {
                        motorDisorot = cekMotor;
                        bautDisorot = null;
                        partDisorot = null;
                        stationDisorot = null;
                        objekInteraksiDisorot = null;
                        teksNamaUI.text = "Mulai Servis";
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
                    teksNamaUI.text = bautDisorot.namaObjek;
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
                    teksNamaUI.text = partDisorot.namaObjek;
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
                    teksNamaUI.text = "Meja Kerja";
                    return;
                }
            }
        }

        partDisorot = null;
        bautDisorot = null;
        motorDisorot = null;
        stationDisorot = null;
        objekInteraksiDisorot = null;
        teksNamaUI.text = "";
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
    }

    void AmbilBarang(Rigidbody rb, string partTag)
    {
        if (rb == null) return;
        barangDipegang = rb;
        barangDipegang.isKinematic = false;
        barangDipegang.useGravity = false;
        barangDipegang.linearDamping = 10f;
        barangDipegang.angularDamping = 10f;

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah != null && langkah.jenisAksi == TipeAksi.LepasBanLuar && partTag == langkah.targetPartTag)
        {
            SequenceService.instance.SelesaikanLangkah();
        }
    }

    void LepasBarang()
    {
        barangDipegang.useGravity = true;
        barangDipegang.linearDamping = 0f;
        barangDipegang.angularDamping = 0.05f;
        barangDipegang = null;
    }
}
using UnityEngine;
using TMPro;

public class InteraksiPemain : MonoBehaviour
{
    [Header("Referensi")]
    public Camera kameraPemain;
    public Transform titikPegangan;
    public TextMeshProUGUI teksNamaUI;

    [Header("Pengaturan Interaksi")]
    public float jarakJangkauan = 3f;
    public float kekuatanTarik = 10f;

    private Rigidbody barangDipegang;
    private DataSparepart partDisorot;
    private Baut bautDisorot;

    void Update()
    {
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

    void SorotBarang()
    {
        Ray ray = kameraPemain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();

        if (barangDipegang == null && Physics.Raycast(ray, out hit, jarakJangkauan, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            Baut cekBaut = hit.collider.GetComponent<Baut>();
            if (cekBaut != null)
            {
                if (langkah != null && langkah.jenisAksi == cekBaut.tipeAksiDibutuhkan)
                {
                    bautDisorot = cekBaut;
                    partDisorot = null;
                    teksNamaUI.text = bautDisorot.namaObjek;
                    return;
                }
            }

            DataSparepart cekPart = hit.collider.GetComponent<DataSparepart>();
            if (cekPart != null)
            {
                if (langkah != null && cekPart.CompareTag(langkah.targetPartTag))
                {
                    partDisorot = cekPart;
                    bautDisorot = null;
                    teksNamaUI.text = partDisorot.namaObjek;
                    return;
                }
            }
        }

        partDisorot = null;
        bautDisorot = null;
        teksNamaUI.text = "";
    }

    void CekInputPegang()
    {
        if (Input.GetMouseButton(0) && bautDisorot != null)
        {
            bautDisorot.ProsesLepas();
        }

        if (Input.GetMouseButtonDown(0) && partDisorot != null)
        {
            if (partDisorot.bisaDiambil)
            {
                AmbilBarang(partDisorot.GetComponent<Rigidbody>(), partDisorot.gameObject.tag);
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
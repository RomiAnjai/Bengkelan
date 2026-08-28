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

        if (barangDipegang == null && Physics.Raycast(ray, out hit, jarakJangkauan))
        {
            DataSparepart cekPart = hit.collider.GetComponent<DataSparepart>();
            if (cekPart != null)
            {
                partDisorot = cekPart;
                teksNamaUI.text = partDisorot.namaObjek;
                return;
            }
        }
        
        partDisorot = null;
        teksNamaUI.text = "";
    }

    void CekInputPegang()
    {
        if (Input.GetMouseButtonDown(0) && partDisorot != null)
        {
            AmbilBarang(partDisorot.GetComponent<Rigidbody>());
        }

        if (Input.GetMouseButtonUp(0) && barangDipegang != null)
        {
            LepasBarang();
        }
    }

    void AmbilBarang(Rigidbody rb)
    {
        barangDipegang = rb;
        barangDipegang.useGravity = false;
        barangDipegang.linearDamping = 10f;
        barangDipegang.angularDamping = 10f;
    }

    void LepasBarang()
    {
        barangDipegang.useGravity = true;
        barangDipegang.linearDamping = 0f;
        barangDipegang.angularDamping = 0.05f;
        barangDipegang = null;
    }
}
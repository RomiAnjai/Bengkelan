using UnityEngine;

public class PenerimaBanMotor : MonoBehaviour, IInteraksi
{
    public Transform titikPasangBan;
    public int indexMasalahPasang = 1; 
    public GameObject[] bautPasangan;

    private bool banSudahDiMotor = false;
    private bool servisDimulai = false;
    private GameObject banYangDipasang;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ban Luar Pasang") && !banSudahDiMotor)
        {
            if (other.attachedRigidbody == null) return;
            
            banYangDipasang = other.attachedRigidbody.gameObject;
            banSudahDiMotor = true;

            banYangDipasang.transform.position = titikPasangBan.position;
            banYangDipasang.transform.rotation = titikPasangBan.rotation;

            Collider[] cols = banYangDipasang.GetComponentsInChildren<Collider>();
            foreach (Collider c in cols)
            {
                c.enabled = false;
            }

            DataSparepart dataPart = banYangDipasang.GetComponent<DataSparepart>();
            if (dataPart == null) dataPart = banYangDipasang.GetComponentInChildren<DataSparepart>();
            if (dataPart != null) dataPart.bisaDiambil = false;

            Rigidbody[] rbs = banYangDipasang.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody r in rbs)
            {
                r.isKinematic = true;
                r.linearVelocity = Vector3.zero;
                r.angularVelocity = Vector3.zero;
            }

            SequenceService.instance.colTriggerMotor.enabled = false;
        }
    }

    public string DapatkanNamaPetunjuk()
    {
        if (banSudahDiMotor && !servisDimulai) return "Mulai Perbaiki Motor";
        return "";
    }

    public void EksekusiAksi()
    {
        if (banSudahDiMotor && !servisDimulai)
        {
            servisDimulai = true;

            foreach (GameObject baut in bautPasangan)
            {
                if (baut != null) baut.SetActive(true);
            }

            SequenceService.instance.MulaiServis(indexMasalahPasang);
            InteraksiMotor.instance.FokusKeMotor();
            
            MekanikController mc = FindAnyObjectByType<MekanikController>();
            if (mc != null) mc.KunciKursor(true);
        }
    }
}
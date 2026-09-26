using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum TipeAksi
{
    LepasBaut,
    LepasAsDepan,
    LepasBanLuar,
    GantiBanDalam,
    PompaBan,
    PasangBanLuar,
    PasangBaut,
    PasangAsDepan
}

[System.Serializable]
public class DetailLangkah
{
    public string namaLangkah;
    public TipeAksi jenisAksi;
    public string targetPartTag; 
    public Transform posisiKamera; 
    public bool pakaiCrosshair; 
    public string idObjekTutorial; 
}

[System.Serializable]
public class MasalahMotor
{
    public string namaKerusakan;
    public List<DetailLangkah> urutanLangkah;
}

public class SequenceService : MonoBehaviour
{
    public static SequenceService instance;

    public List<MasalahMotor> daftarMasalahMotor;
    public bool sedangTransisi = false;

    public int indexLangkahSaatIni = 0;

    public JenisMotor motorYangSedangDiservis;

    [Header("Referensi Velg")]
    public Rigidbody rbVelg;
    public Collider colVelg;
    public DataSparepart dataVelg;
    public Collider colVelgDalam;
    public BoxCollider colTambahanPemental;

    [Header("Referensi Collider Motor")]
    public Collider colTriggerMotor;

    [Header("Pengaturan Geser Velg")]
    public Vector3 jarakGeserVelg = new Vector3(0.5f, 0f, 0f);
    private MasalahMotor masalahAktif;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void MulaiServis(int indexMasalah)
    {
        if (daftarMasalahMotor.Count == 0 || indexMasalah < 0 || indexMasalah >= daftarMasalahMotor.Count) return;

        masalahAktif = daftarMasalahMotor[indexMasalah];

        indexLangkahSaatIni = 0;
        sedangTransisi = false;
        TampilkanLangkahSaatIni();
    }

    public DetailLangkah GetLangkahSaatIni()
    {
        if (masalahAktif == null || indexLangkahSaatIni >= masalahAktif.urutanLangkah.Count) return null;
        return masalahAktif.urutanLangkah[indexLangkahSaatIni];
    }

    public void SelesaikanLangkah()
    {
        if (masalahAktif == null || sedangTransisi) return;
        StartCoroutine(ProsesJedaDanGantiLangkah());
    }

    private IEnumerator ProsesJedaDanGantiLangkah()
    {
        sedangTransisi = true;

        float durasiJeda = 1f;
        if (InteraksiMotor.instance != null)
        {
            durasiJeda = InteraksiMotor.instance.jedaPindahKamera;
        }

        DetailLangkah langkahSelesai = masalahAktif.urutanLangkah[indexLangkahSaatIni];
        TutorialHighlight.MatikanBerdasarkanID(langkahSelesai.idObjekTutorial);

        yield return new WaitForSeconds(durasiJeda);

        indexLangkahSaatIni++;

        if (indexLangkahSaatIni >= masalahAktif.urutanLangkah.Count)
        {
            GameManagerScript.instance.currentState = GameState.MotorSelesai;

            if (InteraksiMotor.instance != null)
            {
                InteraksiMotor.instance.SelesaiServisBebaskanKamera();
            }
        }
        else
        {
            TampilkanLangkahSaatIni();
            if (InteraksiMotor.instance != null)
            {
                InteraksiMotor.instance.UpdateFokusKamera();
            }
        }

        sedangTransisi = false;
    }

    void TampilkanLangkahSaatIni()
    {
        DetailLangkah langkah = masalahAktif.urutanLangkah[indexLangkahSaatIni];
        Debug.Log("=== STEP SEKARANG: " + indexLangkahSaatIni + " | Nama: " + langkah.namaLangkah + " | Aksi: " + langkah.jenisAksi + " ===");
        
        ObjectiveHUD.instance.UpdateHUD(motorYangSedangDiservis, masalahAktif.namaKerusakan, indexLangkahSaatIni);
        if (ObjectiveHUD.instance != null)
        {
            ObjectiveHUD.instance.UpdateHUD(motorYangSedangDiservis, masalahAktif.namaKerusakan, indexLangkahSaatIni);
            Debug.Log("sdsd");
        }

        TutorialHighlight.NyalakanBerdasarkanID(langkah.idObjekTutorial);
    }

    public void PreteliVelg()
    {
        if (rbVelg != null)
        {
            rbVelg.transform.SetParent(null);
        }

        rbVelg.isKinematic = false;
        colVelg.enabled = true;
        dataVelg.enabled = true;
        colVelgDalam.enabled = true;
        colTambahanPemental.enabled = true;
        rbVelg.AddForce(jarakGeserVelg, ForceMode.Impulse);
    }

    public void NyalakanColliderMotor()
    {
        colTriggerMotor.enabled = true;
    }
}
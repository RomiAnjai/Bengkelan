using UnityEngine;
using System.Collections.Generic;

public enum TipeAksi
{
    LepasBaut,
    LepasAsDepan,
    LepasBanLuar,
    GantiBanDalam,
    PompaBan,
    PasangBanLuar,
    PasangBaut
}

[System.Serializable]
public class DetailLangkah
{
    public string namaLangkah;
    public TipeAksi jenisAksi;
    public string targetPartTag; 
}

[System.Serializable]
public class MasalahMotor
{
    public string namaKerusakan;
    
    [Header("Urutan Langkah Perbaikan")]
    public List<DetailLangkah> urutanLangkah;
}

public class SequenceService : MonoBehaviour
{
    public static SequenceService instance;

    [Header("Database Masalah Motor")]
    public List<MasalahMotor> daftarMasalahMotor;

    private int indexLangkahSaatIni = 0;
    private MasalahMotor masalahAktif;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void MulaiServis(int indexMasalah)
    {
        if (daftarMasalahMotor.Count == 0) return;

        masalahAktif = daftarMasalahMotor[indexMasalah];
        indexLangkahSaatIni = 0;

        Debug.Log("Mulai Servis: " + masalahAktif.namaKerusakan);
        TampilkanLangkahSaatIni();
    }

    public DetailLangkah GetLangkahSaatIni()
    {
        if (masalahAktif == null || indexLangkahSaatIni >= masalahAktif.urutanLangkah.Count) return null;
        return masalahAktif.urutanLangkah[indexLangkahSaatIni];
    }

    public void SelesaikanLangkah()
    {
        if (masalahAktif == null) return;

        Debug.Log("Langkah Selesai: " + masalahAktif.urutanLangkah[indexLangkahSaatIni].namaLangkah);
        indexLangkahSaatIni++;

        if (indexLangkahSaatIni >= masalahAktif.urutanLangkah.Count)
        {
            Debug.Log("SERVIS SELESAI! Motor siap diserahkan.");
            GameManagerScript.instance.currentState = GameState.MotorSelesai;
        }
        else
        {
            TampilkanLangkahSaatIni();
        }
    }

    void TampilkanLangkahSaatIni()
    {
        DetailLangkah langkah = masalahAktif.urutanLangkah[indexLangkahSaatIni];
        Debug.Log("Langkah Berikutnya (" + (indexLangkahSaatIni + 1) + "/" + masalahAktif.urutanLangkah.Count + "): " + langkah.namaLangkah);
    }
}
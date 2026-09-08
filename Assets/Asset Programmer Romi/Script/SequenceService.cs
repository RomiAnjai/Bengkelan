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
    PasangBaut
}

[System.Serializable]
public class DetailLangkah
{
    public string namaLangkah;
    public TipeAksi jenisAksi;
    public string targetPartTag; 
    public Transform posisiKamera; 
    public bool pakaiCrosshair; 
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
    private MasalahMotor masalahAktif;
    public GameObject freeCam;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void MulaiServis(int indexMasalah)
    {
        if (daftarMasalahMotor.Count == 0) return;

        masalahAktif = daftarMasalahMotor[indexMasalah];
        
        if (freeCam != null)
        {
            freeCam.SetActive(true);
        }

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
    }
}
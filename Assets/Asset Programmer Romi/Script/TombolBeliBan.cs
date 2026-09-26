using UnityEngine;

public class TombolBeliBan : MonoBehaviour, IInteraksi
{
    public GameObject prefabBanLuarBaru;
    public Transform titikMunculBan;
    public int batasBeli = 1;
    public int beliSaatIni = 0;

    public string DapatkanNamaPetunjuk()
    {
        if (SequenceService.instance == null || SequenceService.instance.indexLangkahSaatIni <= 2)
        {
            return "";
        }

        if (beliSaatIni >= batasBeli)
        {
            return "Stok Ban Habis";
        }

        return "Beli Ban Luar";
    }

    public void EksekusiAksi()
    {
        if (prefabBanLuarBaru == null || titikMunculBan == null) return;

        if (SequenceService.instance != null && SequenceService.instance.indexLangkahSaatIni > 2)
        {
            if (beliSaatIni < batasBeli)
            {
                Instantiate(prefabBanLuarBaru, titikMunculBan.position, titikMunculBan.rotation);
                beliSaatIni++;
                SequenceService.instance.SelesaikanLangkah();
            }
        }
    }
}
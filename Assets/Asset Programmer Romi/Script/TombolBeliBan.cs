using UnityEngine;

public class TombolBeliBan : MonoBehaviour, IInteraksi
{
    public GameObject prefabBanLuarBaru;
    public Transform titikMunculBan;
    
    public string idHighlightMeja = "Meja Kerja";
    public int hargaBeliBan = 15000; 

    public string DapatkanNamaPetunjuk()
    {
        if (SequenceService.instance == null || SequenceService.instance.indexLangkahSaatIni <= 2)
        {
            return "";
        }

        if (ObjectiveHUD.instance != null && ObjectiveHUD.instance.GetBalance() < hargaBeliBan)
        {
            return "Uang Tidak Cukup! (Butuh Rp " + hargaBeliBan.ToString("N0") + ")";
        }

        return "Beli Ban Luar (Rp " + hargaBeliBan.ToString("N0") + ")";
    }

    public void EksekusiAksi()
    {
        if (prefabBanLuarBaru == null || titikMunculBan == null) return;

        if (SequenceService.instance != null && SequenceService.instance.indexLangkahSaatIni > 2)
        {
            if (ObjectiveHUD.instance != null && ObjectiveHUD.instance.GetBalance() >= hargaBeliBan)
            {
                ObjectiveHUD.instance.RemoveMoney(hargaBeliBan);

                if (DailyReportManager.instance != null)
                {
                    DailyReportManager.instance.CatatPengeluaran(hargaBeliBan);
                }

                Instantiate(prefabBanLuarBaru, titikMunculBan.position, titikMunculBan.rotation);
                
                TutorialHighlight.NyalakanBerdasarkanID(idHighlightMeja);
                
                if (SequenceService.instance.indexLangkahSaatIni == 4)
                {
                    SequenceService.instance.SelesaikanLangkah();
                }
            }
        }
    }
}   
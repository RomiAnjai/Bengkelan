using UnityEngine;
using TMPro;

public class DailyReportManager : MonoBehaviour
{
    public static DailyReportManager instance;

    [Header("UI Laporan Harian")]
    public GameObject panelLaporanKertas;
    public TextMeshProUGUI teksJumlahMotor;
    public TextMeshProUGUI teksIncome;
    public TextMeshProUGUI teksPengeluaran;
    public TextMeshProUGUI teksTotalUntung;

    public bool laporanTerbuka = false;

    private int motorDiservisHariIni = 0;
    private int incomeHariIni = 0;
    private int pengeluaranHariIni = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (panelLaporanKertas != null)
        {
            panelLaporanKertas.SetActive(false);
        }
    }

    public void TambahMotorSelesai(int uangDidapat)
    {
        motorDiservisHariIni++;
        incomeHariIni += uangDidapat;
    }

    public void CatatPengeluaran(int uangKeluar)
    {
        pengeluaranHariIni += uangKeluar;
    }

    public void TampilkanLaporanHarian()
    {
        if (panelLaporanKertas != null)
        {
            laporanTerbuka = true;
            int totalUntung = incomeHariIni - pengeluaranHariIni;

            if (teksJumlahMotor != null) teksJumlahMotor.text = "Motor Diservis: " + motorDiservisHariIni.ToString();
            if (teksIncome != null) teksIncome.text = "Income: Rp " + incomeHariIni.ToString("N0");
            if (teksPengeluaran != null) teksPengeluaran.text = "Pengeluaran: Rp " + pengeluaranHariIni.ToString("N0");
            if (teksTotalUntung != null) teksTotalUntung.text = "Total Keuntungan: Rp " + totalUntung.ToString("N0");

            panelLaporanKertas.SetActive(true);

            MekanikController mc = FindAnyObjectByType<MekanikController>();
            if (mc != null) mc.bisaGerak = false;
        }
    }

    public void TutupLaporanHarian()
    {
        laporanTerbuka = false;

        if (panelLaporanKertas != null)
        {
            panelLaporanKertas.SetActive(false);
        }

        motorDiservisHariIni = 0;
        incomeHariIni = 0;
        pengeluaranHariIni = 0;

        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null) mc.bisaGerak = true;

        if (DayManagerScript.instance != null)
        {
            DayManagerScript.instance.GantiKePagi();
        }
    }
}
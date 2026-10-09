using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class OilChangeManager : MonoBehaviour
{
    [Header("Status Langkah (Terbaca Otomatis)")]
    public bool isDrainTrayPlaced = false;
    public bool isOilCapRemoved = false;
    public bool isDrainBoltRemoved = false;
    public bool isFunnelPlaced = false;
    public bool isOldOilDrained = false;
    public bool isNewOilFilled = false;

    [Header("Pengaturan Oli")]
    [Tooltip("Waktu yang dibutuhkan (detik) sampai oli kotor habis terkuras")]
    [SerializeField] private float waktuKurasOli = 5.0f;
    [Tooltip("Kapasitas penuh oli baru (liter)")]
    public float kapasitasOliMaksimal = 1.0f;
    public float volumeOliBaruSaatIni = 0f;

    [Header("Referensi Komponen (Wajib Diisi)")]
    [Tooltip("Masukkan AreaPenempatanItem untuk corong di sini")]
    [SerializeField] private penempatanItem areaCorong;
    [Tooltip("Masukkan BautOli untuk baut tap bawah di sini")]
    [SerializeField] private BautOli bautTapBawah;

    [Header("Events (Efek Visual & Integrasi)")]
    [Tooltip("Dipanggil saat baut bawah terbuka dan oli kotor mulai menetes. (Nyalakan partikel oli hitam di sini)")]
    public UnityEvent OnOliKotorMulaiKeluar;
    
    [Tooltip("Dipanggil saat oli kotor sudah habis. (Matikan partikel oli hitam di sini)")]
    public UnityEvent OnOliKotorHabis;
    
    [Tooltip("Sinyal Utama: Hubungkan ke SequenceService -> FungsiLanjutStep() untuk menyelesaikan misi!")]
    public UnityEvent OnOilServiceCompleted;

    private void Start()
    {
        // MENGUNCI PRASYARAT (Sistem Modular / Decoupled)
        // 1. Corong HANYA BISA dipasang jika Tutup Oli Atas sudah dibuka.
        if (areaCorong != null)
        {
            areaCorong.CekSyaratPasang = () => { return isOilCapRemoved; };
        }

        // 2. Baut Tap Bawah HANYA BISA dibuka jika Wadah Oli (Drain Tray) sudah dipasang.
        if (bautTapBawah != null)
        {
            bautTapBawah.CekSyaratBuka = () => { return isDrainTrayPlaced; };
        }
    }

    // ==========================================
    // FUNGSI UNTUK DIPANGGIL DARI INSPECTOR (UNITY EVENT)
    // ==========================================

    // --- WADAH OLI (DRAIN TRAY) ---
    public void PasangDrainTray() { isDrainTrayPlaced = true; CekSelesai(); }
    public void AmbilDrainTray() { isDrainTrayPlaced = false; }

    // --- TUTUP OLI ATAS (OIL CAP) ---
    public void BukaTutupAtas() { isOilCapRemoved = true; }
    public void TutupTutupAtas() { isOilCapRemoved = false; CekSelesai(); }

    // --- BAUT TAP BAWAH ---
    public void BukaBautBawah()
    {
        isDrainBoltRemoved = true;
        // Begitu baut bawah terbuka, otomatis mulai kuras oli
        if (!isOldOilDrained)
        {
            StartCoroutine(ProsesKurasOliKotor());
        }
    }
    public void TutupBautBawah() { isDrainBoltRemoved = false; CekSelesai(); }

    // --- CORONG (FUNNEL) ---
    public void PasangCorong() { isFunnelPlaced = true; }
    public void AmbilCorong() { isFunnelPlaced = false; CekSelesai(); }

    // ==========================================
    // LOGIKA INTERNAL SIMULASI
    // ==========================================

    // Coroutine untuk mensimulasikan waktu tunggu oli keluar
    private IEnumerator ProsesKurasOliKotor()
    {
        Debug.Log("Oli kotor mulai mengalir...");
        OnOliKotorMulaiKeluar?.Invoke();

        // Tunggu beberapa detik sesuai pengaturan
        yield return new WaitForSeconds(waktuKurasOli);

        isOldOilDrained = true;
        Debug.Log("Oli kotor sudah habis terkuras!");
        OnOliKotorHabis?.Invoke();
    }

    /// <summary>
    /// Fungsi ini akan terus dipanggil oleh BotolOli.cs saat pemain menuangkan oli.
    /// </summary>
    public void TambahVolumeOli(float jumlahTuang)
    {
        // Pengaman: Jangan tuang jika corong tidak ada atau baut bawah masih bocor!
        if (!isFunnelPlaced) return;
        if (isDrainBoltRemoved)
        {
            Debug.LogWarning("BAHAYA: Menuang oli tapi baut bawah belum ditutup! Oli tumpah!");
            return; // (Opsional: Tambahkan logika pengurangan skor di sini)
        }

        if (!isNewOilFilled)
        {
            volumeOliBaruSaatIni += jumlahTuang;
            if (volumeOliBaruSaatIni >= kapasitasOliMaksimal)
            {
                volumeOliBaruSaatIni = kapasitasOliMaksimal;
                isNewOilFilled = true;
                Debug.Log("Oli baru sudah terisi penuh!");
                CekSelesai();
            }
        }
    }

    // ==========================================
    // PENGECEKAN FINAL
    // ==========================================
    
    private void CekSelesai()
    {
        // Game mengecek: Apakah semua hal sudah dikembalikan ke kondisi aman?
        bool kondisiAman = isOldOilDrained       // Oli lama sudah habis
                        && isNewOilFilled        // Oli baru sudah terisi
                        && !isDrainBoltRemoved   // Baut bawah sudah DITUTUP
                        && !isOilCapRemoved      // Tutup atas sudah DITUTUP
                        && !isFunnelPlaced;      // Corong sudah DIAMBIL

        if (kondisiAman)
        {
            Debug.Log("--- SISTEM GANTI OLI SELESAI DENGAN SEMPURNA! ---");
            OnOilServiceCompleted?.Invoke(); 
        }
    }
}
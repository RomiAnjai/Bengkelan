using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class botolOli : MonoBehaviour
{
    [Header("Referensi Sistem")]
    [Tooltip("Masukkan objek yang memiliki script OilChangeManager ke kolom ini")]
    [SerializeField] private OilChangeManager oilManager;

    [Header("Pengaturan Penuangan")]
    [Tooltip("Berapa liter oli yang tertuang per detik saat ditahan")]
    [SerializeField] private float kecepatanTuang = 0.25f;

    [Header("Visual & Audio")]
    [Tooltip("Model visual botol yang akan diputar/dimiringkan saat menuang")]
    [SerializeField] private Transform modelVisualBotol;
    [Tooltip("Target rotasi saat botol dimiringkan (Misal miring 90 derajat di sumbu Z)")]
    [SerializeField] private Vector3 rotasiMiring = new Vector3(0, 0, 90f);
    [Tooltip("Kecepatan transisi animasi miring")]
    [SerializeField] private float kecepatanAnimasi = 8f;
    
    [SerializeField] private ParticleSystem partikelOli;
    [SerializeField] private AudioSource audioTuang;

    [Header("Event Callback")]
    [Tooltip("Dipanggil jika pemain mencoba menuang tapi corong belum ada, atau oli sudah penuh")]
    public UnityEvent OnTuangDitolak;

    // Variabel internal
    private Vector3 rotasiAwal;
    private bool sedangDituang = false;

    private void Start()
    {
        if (modelVisualBotol != null)
        {
            rotasiAwal = modelVisualBotol.localEulerAngles;
        }
        else
        {
            Debug.LogWarning("Model Visual Botol belum diisi di Inspector!");
        }
    }

    /// <summary>
    /// Fungsi ini HARUS dipanggil TERUS MENERUS oleh script Interaksi/Raycast 
    /// SELAMA pemain menahan tombol klik pada objek botol ini.
    /// </summary>
    public void ProsesInteraksi()
    {
        if (oilManager == null) return;

        // Validasi: Apakah corong sudah terpasang? Dan apakah oli belum penuh?
        if (!oilManager.isFunnelPlaced || oilManager.isNewOilFilled)
        {
            // Jika syarat tidak terpenuhi, tolak interaksi
            OnTuangDitolak?.Invoke();
            HentikanInteraksi(); // Pastikan animasi/partikel mati
            return;
        }

        // --- Proses Menuang Berjalan ---
        sedangDituang = true;

        // 1. Jalankan Partikel & Audio (hanya jika belum menyala)
        if (partikelOli != null && !partikelOli.isPlaying) partikelOli.Play();
        if (audioTuang != null && !audioTuang.isPlaying) audioTuang.Play();

        // 2. Tambahkan volume oli ke Manager (Berdasarkan waktu agar mulus)
        float jumlahTuangFrameIni = kecepatanTuang * Time.deltaTime;
        oilManager.TambahVolumeOli(jumlahTuangFrameIni);
    }

    /// <summary>
    /// Fungsi ini dipanggil SEKALI ketika pemain MELEPAS tombol klik (berhenti interaksi).
    /// </summary>
    public void HentikanInteraksi()
    {
        sedangDituang = false;

        // Matikan efek partikel dan audio
        if (partikelOli != null && partikelOli.isPlaying) partikelOli.Stop();
        if (audioTuang != null && audioTuang.isPlaying) audioTuang.Stop();
    }

    private void Update()
    {
        // Animasi rotasi (Smoothing / Lerp)
        // Diletakkan di Update agar pergerakan memiringkan botol dan mengembalikannya tegak terlihat sangat mulus
        if (modelVisualBotol != null)
        {
            Vector3 targetRotasi = sedangDituang ? rotasiAwal + rotasiMiring : rotasiAwal;
            
            // Quaternion.Lerp mencegah Gimbal Lock dan membuat transisi rotasi halus
            modelVisualBotol.localRotation = Quaternion.Lerp(
                modelVisualBotol.localRotation, 
                Quaternion.Euler(targetRotasi), 
                Time.deltaTime * kecepatanAnimasi
            );
        }
    }
}
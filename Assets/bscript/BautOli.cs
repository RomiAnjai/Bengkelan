using UnityEngine;
using UnityEngine.Events;

public class BautOli : MonoBehaviour
{
    [Header("Pengaturan Gerakan Baut")]
    [Tooltip("Waktu yang dibutuhkan (dalam detik) untuk membuka baut sepenuhnya")]
    [SerializeField] private float durasiBuka = 1.5f;
    [SerializeField] private float jarakKeluar = 0.1f;
    [SerializeField] private Vector3 arahKeluar = Vector3.forward;
    
    [Tooltip("Sumbu putaran baut (X, Y, atau Z)")]
    [SerializeField] private Vector3 sumbuPutar = new Vector3(0, 0, 1);
    [SerializeField] private float kecepatanPutar = 500f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioBaut;
    [SerializeField] private AudioClip sfxPutar;

    [Header("Event Callback (Opsional)")]
    [Tooltip("Dipanggil ketika baut sukses dilepas (Hubungkan ke Manager/SequenceService di sini)")]
    public UnityEvent OnBautSelesaiDilepas;
    
    [Tooltip("Dipanggil jika baut diklik tapi ditolak (misal: wadah oli belum ada)")]
    public UnityEvent OnInteraksiDitolak;

    // --- Variabel Internal State ---
    private Vector3 posisiAwal;
    private Vector3 posisiAkhir;
    private float progres = 0f;
    private bool selesai = false;

    // Delegate untuk mengecek prasyarat dari luar tanpa bergantung pada satu class tertentu (Decoupled)
    // Func<bool> mengembalikan nilai True/False.
    public System.Func<bool> CekSyaratBuka;

    void Start()
    {
        // Cache posisi awal dan akhir untuk optimasi
        posisiAwal = transform.localPosition;
        posisiAkhir = posisiAwal + (arahKeluar.normalized * jarakKeluar);
    }

    /// <summary>
    /// Dipanggil terus-menerus (terus ditahan/diklik) oleh skrip Interaksi kursor pemain.
    /// </summary>
    public void ProsesInteraksi()
    {
        if (selesai) return;

        // 1. Validasi Prasyarat (Apakah boleh dibuka?)
        // Jika delegate CekSyaratBuka ada isinya, dan mengembalikan false, maka tolak.
        if (CekSyaratBuka != null && !CekSyaratBuka.Invoke())
        {
            OnInteraksiDitolak?.Invoke();
            return; // Hentikan proses eksekusi
        }

        // 2. Mainkan Audio jika belum menyala
        if (audioBaut != null && sfxPutar != null && !audioBaut.isPlaying)
        {
            audioBaut.clip = sfxPutar;
            audioBaut.loop = true;
            audioBaut.Play();
        }

        // 3. Kalkulasi Progres (0 hingga 1)
        progres += Time.deltaTime / durasiBuka;

        // 4. Update Visual (Rotasi dan Posisi)
        // Menggunakan transform.Rotate lebih aman dari Euler Angles manual untuk menghindari Gimbal Lock
        transform.Rotate(sumbuPutar.normalized * kecepatanPutar * Time.deltaTime, Space.Self);
        transform.localPosition = Vector3.Lerp(posisiAwal, posisiAkhir, progres);

        // 5. Cek Selesai
        if (progres >= 1f)
        {
            SelesaiInteraksi();
        }
    }

    /// <summary>
    /// Panggil fungsi ini jika pemain melepas klik sebelum baut sepenuhnya terbuka.
    /// </summary>
    public void HentikanInteraksiManual()
    {
        if (audioBaut != null && audioBaut.isPlaying)
        {
            audioBaut.Stop();
        }
    }

    private void SelesaiInteraksi()
    {
        selesai = true;
        
        // Pastikan posisi snap sempurna di titik akhir
        transform.localPosition = posisiAkhir;
        
        // Matikan suara
        if (audioBaut != null) audioBaut.Stop();

        // Beritahu sistem lain (seperti OilChangeManager atau SequenceService)
        OnBautSelesaiDilepas?.Invoke();

        // Nonaktifkan objek ini (Lebih optimal daripada Destroy)
        gameObject.SetActive(false);
    }
}
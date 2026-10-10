using UnityEngine;
using UnityEngine.Events;

public class BautOli : MonoBehaviour
{
    [Header("Pengaturan Gerakan Baut")]
    [Tooltip("Waktu yang dibutuhkan (dalam detik) untuk membuka/menutup baut sepenuhnya")]
    [SerializeField] private float durasiBuka = 1.5f;
    [SerializeField] private float jarakKeluar = 0.1f;
    [SerializeField] private Vector3 arahKeluar = Vector3.forward;

    [Tooltip("Sumbu putaran baut (X, Y, atau Z)")]
    [SerializeField] private Vector3 sumbuPutar = new Vector3(0, 0, 1);
    [SerializeField] private float kecepatanPutar = 500f;

    [Header("Status Mode")]
    [Tooltip("False = Proses Melepas (Keluar), True = Proses Memasang (Masuk)")]
    public bool modePasang = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioBaut;
    [SerializeField] private AudioClip sfxPutar;

    [Header("Event Callback")]
    [Tooltip("Dipanggil ketika baut sukses dilepas")]
    public UnityEvent OnBautSelesaiDilepas;

    [Tooltip("Dipanggil ketika baut sukses dipasang/ditutup kembali")]
    public UnityEvent OnBautSelesaiDipasang;

    [Tooltip("Dipanggil jika baut diklik tapi ditolak syaratnya")]
    public UnityEvent OnInteraksiDitolak;

    // --- Variabel Internal State ---
    private Vector3 posisiAwal;     // Posisi rapat di mesin
    private Vector3 posisiAkhir;    // Posisi keluar/terbuka
    private float progres = 0f;
    private bool selesai = false;

    // Delegate untuk mengecek prasyarat dari luar
    public System.Func<bool> CekSyaratBuka;
    public System.Func<bool> CekSyaratPasang;

    void Start()
    {
        // Cache posisi awal (rapat) dan posisi akhir (terbuka)
        posisiAwal = transform.localPosition;
        posisiAkhir = posisiAwal + (arahKeluar.normalized * jarakKeluar);

        // Jika diset pasang dari awal, posisikan di luar
        if (modePasang)
        {
            transform.localPosition = posisiAkhir;
        }
    }

    /// <summary>
    /// Dipanggil terus-menerus selama pemain menahan klik pada baut.
    /// </summary>
    public void ProsesInteraksi()
    {
        if (selesai) return;

        // Validasi Prasyarat Berdasarkan Mode
        if (!modePasang)
        {
            if (CekSyaratBuka != null && !CekSyaratBuka.Invoke())
            {
                OnInteraksiDitolak?.Invoke();
                return;
            }
        }
        else
        {
            if (CekSyaratPasang != null && !CekSyaratPasang.Invoke())
            {
                OnInteraksiDitolak?.Invoke();
                return;
            }
        }

        // Mainkan Audio Putar
        if (audioBaut != null && sfxPutar != null && !audioBaut.isPlaying)
        {
            audioBaut.clip = sfxPutar;
            audioBaut.loop = true;
            audioBaut.Play();
        }

        // Kalkulasi Progres (0 hingga 1)
        progres += Time.deltaTime / durasiBuka;

        // Update Visual (Rotasi dan Posisi)
        if (!modePasang)
        {
            // Mode Melepas: Putar searah, bergerak ke luar
            transform.Rotate(sumbuPutar.normalized * kecepatanPutar * Time.deltaTime, Space.Self);
            transform.localPosition = Vector3.Lerp(posisiAwal, posisiAkhir, progres);
        }
        else
        {
            // Mode Memasang: Putar berlawanan arah, bergerak kembali ke dalam
            transform.Rotate(-sumbuPutar.normalized * kecepatanPutar * Time.deltaTime, Space.Self);
            transform.localPosition = Vector3.Lerp(posisiAkhir, posisiAwal, progres);
        }

        // Cek Apakah Selesai
        if (progres >= 1f)
        {
            SelesaiInteraksi();
            Debug.Log("selesai");
        }
    }

    /// <summary>
    /// Dipanggil saat pemain melepaskan klik sebelum progres penuh.
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
        if (audioBaut != null) audioBaut.Stop();

        if (!modePasang)
        {
            // Selesai Dilepas
            transform.localPosition = posisiAkhir;
            OnBautSelesaiDilepas?.Invoke();
            // Jangan didestroy/disable total agar objeknya tetap ada di tempat untuk nanti dipasang lagi
        }
        else
        {
            // Selesai Dipasang Kembali
            transform.localPosition = posisiAwal;
            OnBautSelesaiDipasang?.Invoke();
            //gameObject.SetActive(false); // Nonaktifkan setelah rapat sepenuhnya
        }
    }

    /// <summary>
    /// Fungsi untuk mengubah baut ke mode pasang (dipanggil oleh OilChangeManager saat oli kotor habis)
    /// </summary>
    public void SiapkanModePasang()
    {
        gameObject.SetActive(true); // Munculkan kembali jika tadinya disembunyikan
        modePasang = true;          // Ubah mode jadi pasang
        selesai = false;
        progres = 0f;
        transform.localPosition = posisiAkhir; // Mulai dari posisi luar
    }
}
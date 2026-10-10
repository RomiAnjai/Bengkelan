using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class penempatanItem : MonoBehaviour
{
    [Header("Pengaturan Objek Fisik")]
    [Tooltip("Masukkan objek 3D (Wadah Oli / Corong) yang sudah diposisikan pas di area ini. Objek ini akan otomatis disembunyikan saat game dimulai.")]
    [SerializeField] private GameObject objekItemTarget;

    [Header("Event Callback (Hubungkan ke Manager)")]
    [Tooltip("Dipanggil saat item berhasil diletakkan (Misal: panggil fungsi PlaceDrainTray di Manager)")]
    public UnityEvent OnItemTerpasang;
    
    [Tooltip("Dipanggil jika item bisa diambil kembali (Misal: panggil fungsi RemoveFunnel di Manager)")]
    public UnityEvent OnItemDiambil;

    [Tooltip("Dipanggil jika pemain mencoba menaruh item tapi syaratnya belum terpenuhi")]
    public UnityEvent OnPasangDitolak;

    // --- Variabel Internal ---
    public bool isTerpasang = false;
    private Collider areaCollider;

    // Delegate untuk mengecek prasyarat dari script luar (Decoupled)
    // Contoh: Corong mengecek apakah tutup oli atas sudah dibuka.
    public System.Func<bool> CekSyaratPasang;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();
        objekItemTarget.SetActive(false);
        // Pastikan saat game mulai, objek fisiknya disembunyikan terlebih dahulu
        if (objekItemTarget != null)
        {
            
        }
        else
        {
            Debug.LogWarning("Objek Item Target pada " + gameObject.name + " belum diisi di Inspector!");
        }
    }

    /// <summary>
    /// Panggil fungsi ini via Raycast/InteraksiMotor saat pemain mengklik area penempatan.
    /// </summary>
    public void PasangItem()
    {
        // Cegah eksekusi ganda
        if (isTerpasang) return;

        // Validasi prasyarat (Jika ada syarat, dan tidak terpenuhi, tolak interaksi)
        if (CekSyaratPasang != null && !CekSyaratPasang.Invoke())
        {
            OnPasangDitolak?.Invoke();
            Debug.Log("Penempatan ditolak: Prasyarat belum terpenuhi.");
            return;
        }

        // Munculkan objek fisik
        if (objekItemTarget != null)
        {
            objekItemTarget.SetActive(true);
            Debug.Log("taruh");
        }

        isTerpasang = true;
        areaCollider.enabled = false; 
        
        // Matikan area klik agar tidak bisa diklik berulang kali

        // Beritahu OilChangeManager bahwa item sudah dipasang
        OnItemTerpasang?.Invoke();
    }

    /// <summary>
    /// Panggil fungsi ini jika pemain mengklik objek (seperti corong) untuk mengambilnya kembali ke inventory.
    /// </summary>
    public void AmbilItem()
    {
        if (!isTerpasang) return;
        Debug.Log("ambil");

        // Sembunyikan objek fisik
        if (objekItemTarget != null)
        {
            objekItemTarget.SetActive(false);
            Debug.Log("disembunyikan");
        }

        isTerpasang = false;
        areaCollider.enabled = true; // Hidupkan lagi area klik untuk penempatan di masa depan

        // Beritahu OilChangeManager bahwa item telah diambil
        OnItemDiambil?.Invoke();
    }
}
using UnityEngine;
using UnityEngine.Events;

public class OilChangeManager : MonoBehaviour
{
    [Header("Status Persiapan (Bebas)")]
    public bool isDrainTrayPlaced = false;
    public bool isOilCapRemoved = false;
    public bool isFunnelPlaced = false;

    [Header("Status Kritis (Terkunci)")]
    public bool isDrainBoltRemoved = false;
    public bool isOldOilDrained = false;
    public bool isNewOilFilled = false;

    [Header("Volume Oli")]
    public float maxOilCapacity = 1.0f; // 1 Liter
    public float currentCleanOil = 0f;

    [Header("Events (Koneksi ke Sistem Lama)")]
    // Event ini yang akan kita hubungkan ke SequenceService di Inspector!
    public UnityEvent OnOilServiceCompleted; 
    public UnityEvent OnStepUpdated; // Berguna jika ingin memicu highlight tutorial

    // --- FUNGSI UNTUK DIPANGGIL OLEH OBJEK 3D ---

    public void PlaceDrainTray()
    {
        isDrainTrayPlaced = true;
        Debug.Log("Wadah oli telah diletakkan.");
        CheckCompletion();
    }

    // Fungsi ini dipanggil saat pemain mengklik Baut Tap Bawah
    public bool TryRemoveDrainBolt()
    {
        // HARD LOCK: Cek apakah wadah sudah ada
        if (!isDrainTrayPlaced)
        {
            Debug.LogWarning("TIDAK BISA BUKA BAUT: Wadah oli belum dipasang!");
            // Di sini kamu bisa tambahkan suara error atau notifikasi UI
            return false; 
        }

        isDrainBoltRemoved = true;
        Debug.Log("Baut tap bawah berhasil dibuka. Oli mulai mengalir...");
        
        // Panggil fungsi simulasi menguras oli
        StartDrainingOil(); 
        return true;
    }

    private void StartDrainingOil()
    {
        // Logika menguras oli kotor (misal pakai Coroutine/Timer)
        // Setelah selesai:
        isOldOilDrained = true;
        Debug.Log("Oli kotor habis.");
        CheckCompletion();
    }

    public void CloseDrainBolt()
    {
        if (isOldOilDrained)
        {
            isDrainBoltRemoved = false;
            Debug.Log("Baut tap bawah ditutup rapat.");
            CheckCompletion();
        }
    }

    // --- PENGECEKAN FINAL ---
    private void CheckCompletion()
    {
        OnStepUpdated?.Invoke();

        // Cek apakah semua kondisi akhir yang aman sudah terpenuhi
        if (!isDrainBoltRemoved && !isOilCapRemoved && isOldOilDrained && isNewOilFilled && !isFunnelPlaced)
        {
            Debug.Log("SISTEM GANTI OLI SELESAI SECARA SEMPURNA!");
            // Memicu sinyal ke SequenceService bahwa misi ganti oli kelar
            OnOilServiceCompleted?.Invoke(); 
        }
    }
}
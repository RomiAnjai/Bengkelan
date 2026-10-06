using UnityEngine;

public class BautTapOli : MonoBehaviour
{
    [Header("Referensi Manager")]
    public OilChangeManager oilManager;

    // Fungsi ini dipanggil saat pemain mengklik baut ini (via Raycast/EventTrigger)
    public void OnInteract()
    {
        // Meminta izin ke Manager untuk membuka baut
        bool isAllowed = oilManager.TryRemoveDrainBolt();

        if (isAllowed)
        {
            // Lakukan animasi baut copot, putar suara SFX, atau matikan mesh
            gameObject.SetActive(false); 
        }
        else
        {
            // Baut tidak bisa dibuka (karena wadah belum ditaruh)
            // Bisa mainkan suara "terkunci" atau getaran kecil
        }
    }
}
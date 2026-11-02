using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class penempatanItem : MonoBehaviour
{
    [Header("Pengaturan Objek Fisik")]
    [SerializeField] private GameObject objekItemTarget;

    [Header("Event Callback (Hubungkan ke Manager)")]
    public UnityEvent OnItemTerpasang;
    public UnityEvent OnItemDiambil;
    public UnityEvent OnPasangDitolak;

    public bool isTerpasang = false;
    private Collider areaCollider;

    public System.Func<bool> CekSyaratPasang;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();

        if (objekItemTarget != null)
        {
            Collider[] colliderTarget = objekItemTarget.GetComponentsInChildren<Collider>(true);
            foreach (Collider col in colliderTarget)
            {
                if (col != areaCollider)
                {
                    col.enabled = false;
                }
            }

            objekItemTarget.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Objek Item Target pada " + gameObject.name + " belum diisi di Inspector!");
        }
    }

    public void PasangItem()
    {
        if (isTerpasang) return;

        if (CekSyaratPasang != null && !CekSyaratPasang.Invoke())
        {
            OnPasangDitolak?.Invoke();
            Debug.Log("Penempatan ditolak: Prasyarat belum terpenuhi.");
            return;
        }

        if (objekItemTarget != null)
        {
            objekItemTarget.SetActive(true);
            Debug.Log("taruh");
        }

        isTerpasang = true;
        OnItemTerpasang?.Invoke();
    }

    public void AmbilItem()
    {
        if (!isTerpasang) return;

        Debug.Log("ambil");
        if (objekItemTarget != null)
        {
            objekItemTarget.SetActive(false);
            Debug.Log("disembunyikan");
        }

        isTerpasang = false;
        if (areaCollider != null) areaCollider.enabled = true;

        OnItemDiambil?.Invoke();
    }
}
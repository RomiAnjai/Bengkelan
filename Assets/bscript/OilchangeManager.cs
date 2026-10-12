using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class OilChangeManager : MonoBehaviour
{
    public static OilChangeManager instance;

    [Header("Status Langkah")]
    public bool isDrainTrayPlaced = false;
    public bool isOilCapRemoved = false;
    public bool isDrainBoltRemoved = false;
    public bool isFunnelPlaced = false;
    public bool isOldOilDrained = false;
    public bool isNewOilFilled = false;

    [Header("Pengaturan Oli")]
    [SerializeField] private float waktuKurasOli = 5.0f;
    public float kapasitasOliMaksimal = 1.0f;
    public float volumeOliBaruSaatIni = 0f;

    [Header("Referensi Internal Prefab (Assign di dalam Prefab)")]
    [SerializeField] private penempatanItem areaDrainTray;
    [SerializeField] private penempatanItem areaCorong;
    [SerializeField] private GameObject objekAreaCorong;
    [SerializeField] private BautOli bautTapBawah;
    public GameObject botolOliDiMotor;

    [Header("Events Internal Prefab (Partikel & Audio)")]
    public UnityEvent OnOliKotorMulaiKeluar;
    public UnityEvent OnOliKotorHabis;
    public UnityEvent OnOilServiceCompleted;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (botolOliDiMotor != null)
        {
            botolOliDiMotor.SetActive(false);
        }

        if (objekAreaCorong != null)
        {
            objekAreaCorong.SetActive(false);
        }

        if (areaDrainTray != null)
        {
            areaDrainTray.CekSyaratPasang = () => { return true; };
        }

        if (areaCorong != null)
        {
            areaCorong.CekSyaratPasang = () => { return isOilCapRemoved; };
        }

        if (bautTapBawah != null)
        {
            bautTapBawah.CekSyaratBuka = () => { return isDrainTrayPlaced; };
        }
    }

    public void MunculkanBotolDiMotor()
    {
        if (botolOliDiMotor != null)
        {
            botolOliDiMotor.SetActive(true);
        }
    }

    public void PasangDrainTray() { isDrainTrayPlaced = true; CekSelesai(); }
    public void AmbilDrainTray() { isDrainTrayPlaced = false; }

    public void BukaTutupAtas() 
    { 
        isOilCapRemoved = true; 
        if (objekAreaCorong != null) objekAreaCorong.SetActive(true);
    }

    public void TutupTutupAtas() { isOilCapRemoved = false; CekSelesai(); }

    public void BukaBautBawah()
    {
        isDrainBoltRemoved = true;
        if (!isOldOilDrained)
        {
            StartCoroutine(ProsesKurasOliKotor());
        }
    }

    public void TutupBautBawah() { isDrainBoltRemoved = false; CekSelesai(); }

    public void PasangCorong() { isFunnelPlaced = true; }
    public void AmbilCorong() { isFunnelPlaced = false; CekSelesai(); }

    private IEnumerator ProsesKurasOliKotor()
    {
        OnOliKotorMulaiKeluar?.Invoke();
        yield return new WaitForSeconds(waktuKurasOli);
        isOldOilDrained = true;
        OnOliKotorHabis?.Invoke();
    }

    public void TambahVolumeOli(float jumlahTuang)
    {
        if (!isFunnelPlaced) return;
        if (isDrainBoltRemoved) return;

        if (!isNewOilFilled)
        {
            volumeOliBaruSaatIni += jumlahTuang;
            if (volumeOliBaruSaatIni >= kapasitasOliMaksimal)
            {
                volumeOliBaruSaatIni = kapasitasOliMaksimal;
                isNewOilFilled = true;

                if (botolOliDiMotor != null)
                {
                    botolOliDiMotor.SetActive(false);
                }

                if (SequenceService.instance != null)
                {
                    SequenceService.instance.SelesaikanLangkah();
                }

                CekSelesai();
            }
        }
    }

    private void CekSelesai()
    {
        bool kondisiAman = isOldOilDrained
                        && isNewOilFilled
                        && !isDrainBoltRemoved
                        && !isOilCapRemoved
                        && !isFunnelPlaced;

        if (kondisiAman)
        {
            OnOilServiceCompleted?.Invoke();
            if (SequenceService.instance != null)
            {
                SequenceService.instance.SelesaikanLangkah();
            }
        }
    }
}
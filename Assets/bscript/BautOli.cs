using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BautOli : MonoBehaviour
{
    [Header("Pengaturan Gerakan Baut")]
    [SerializeField] private float durasiBuka = 1.5f;
    [SerializeField] private float jarakKeluar = 0.1f;
    [SerializeField] private Vector3 arahKeluar = Vector3.forward;
    [SerializeField] private Vector3 sumbuPutar = new Vector3(0, 0, 1);
    [SerializeField] private float kecepatanPutar = 500f;

    [Header("Status Mode")]
    public bool modePasang = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioBaut;
    [SerializeField] private AudioClip sfxPutar;

    [Header("Event Callback")]
    public UnityEvent OnBautSelesaiDilepas;
    public UnityEvent OnBautSelesaiDipasang;
    public UnityEvent OnInteraksiDitolak;

    private Vector3 posisiAwal;
    private Vector3 posisiAkhir;
    private float progres = 0f;
    private bool selesaiSemua = false;
    private bool butuhLepasKlik = false;

    public System.Func<bool> CekSyaratBuka;
    public System.Func<bool> CekSyaratPasang;

    void Start()
    {
        posisiAwal = transform.localPosition;
        posisiAkhir = posisiAwal + (arahKeluar.normalized * jarakKeluar);

        if (modePasang)
        {
            transform.localPosition = posisiAkhir;
        }
    }

    public void ProsesInteraksi()
    {
        if (selesaiSemua || butuhLepasKlik) return;

        if (!modePasang)
        {
            if (CekSyaratBuka != null && !CekSyaratBuka.Invoke())
            {
                OnInteraksiDitolak?.Invoke();
                return;
            }

            NyalakanAudio();
            progres += Time.deltaTime / durasiBuka;
            transform.Rotate(sumbuPutar.normalized * kecepatanPutar * Time.deltaTime, Space.Self);
            transform.localPosition = Vector3.Lerp(posisiAwal, posisiAkhir, progres);

            if (progres >= 1f)
            {
                SelesaiBuka();
            }
        }
        else
        {
            if (CekSyaratPasang != null && !CekSyaratPasang.Invoke())
            {
                OnInteraksiDitolak?.Invoke();
                return;
            }

            NyalakanAudio();
            progres += Time.deltaTime / durasiBuka;
            transform.Rotate(-sumbuPutar.normalized * kecepatanPutar * Time.deltaTime, Space.Self);
            transform.localPosition = Vector3.Lerp(posisiAkhir, posisiAwal, progres);

            if (progres >= 1f)
            {
                SelesaiPasang();
            }
        }
    }

    private void NyalakanAudio()
    {
        if (audioBaut != null && sfxPutar != null && !audioBaut.isPlaying)
        {
            audioBaut.clip = sfxPutar;
            audioBaut.loop = true;
            audioBaut.Play();
        }
    }

    public void HentikanInteraksiManual()
    {
        butuhLepasKlik = false;

        if (audioBaut != null && audioBaut.isPlaying)
        {
            audioBaut.Stop();
        }
    }

    private void SelesaiBuka()
    {
        if (audioBaut != null) audioBaut.Stop();

        transform.localPosition = posisiAkhir;
        progres = 0f;
        modePasang = true;
        butuhLepasKlik = true;

        OnBautSelesaiDilepas?.Invoke();
    }

    private void SelesaiPasang()
    {
        if (audioBaut != null) audioBaut.Stop();

        transform.localPosition = posisiAwal;
        progres = 0f;
        selesaiSemua = true;

        OnBautSelesaiDipasang?.Invoke();
    }
}
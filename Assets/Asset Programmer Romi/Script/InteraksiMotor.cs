using UnityEngine;
using System.Collections;

public class InteraksiMotor : MonoBehaviour
{
    public static InteraksiMotor instance;
    
    public float jedaPindahKamera = 1.5f;
    public float waktuPerpindahanKamera = 1f;

    private GameObject kamera;
    private GameObject kamerapos;
    private MekanikController mc;
    
    public bool sedangFokus = false;
    private Vector3 posisiAwalKamera;
    private Quaternion rotasiAwalKamera;
    private Coroutine transisiKameraRoutine;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        kamera = GameObject.FindWithTag("MainCamera");
        kamerapos = GameObject.FindWithTag("PosisiKamera");
        mc = FindAnyObjectByType<MekanikController>();
    }

    void Update()
    {
        
    }

    public void FokusKeMotor()
    {
        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah == null || langkah.posisiKamera == null) return;

        posisiAwalKamera = kamerapos.transform.position;
        rotasiAwalKamera = kamerapos.transform.rotation;

        mc.bisaGerak = false;

        if (transisiKameraRoutine != null) StopCoroutine(transisiKameraRoutine);
        transisiKameraRoutine = StartCoroutine(PindahKameraSmooth(langkah.posisiKamera.position, langkah.posisiKamera.rotation));
        
        sedangFokus = true;
    }

    public void UpdateFokusKamera()
    {
        if (sedangFokus == false) return;
        StartCoroutine(ProsesJedaKamera());
    }

    private IEnumerator ProsesJedaKamera()
    {
        yield return new WaitForSeconds(jedaPindahKamera);

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();

        if (langkah != null && langkah.posisiKamera != null)
        {
            if (transisiKameraRoutine != null) StopCoroutine(transisiKameraRoutine);
            transisiKameraRoutine = StartCoroutine(PindahKameraSmooth(langkah.posisiKamera.position, langkah.posisiKamera.rotation));
        }
        else
        {
            KembaliKePemain();
        }
    }

    private IEnumerator PindahKameraSmooth(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        Vector3 startPos = kamera.transform.position;
        Quaternion startRot = kamera.transform.rotation;

        while (elapsed < waktuPerpindahanKamera)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / waktuPerpindahanKamera;
            
            kamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            kamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        kamera.transform.position = targetPos;
        kamera.transform.rotation = targetRot;
    }

    public void KembaliKePemain()
    {
        if (sedangFokus == false) return;

        if (transisiKameraRoutine != null) StopCoroutine(transisiKameraRoutine);
        transisiKameraRoutine = StartCoroutine(PindahKameraSmooth(posisiAwalKamera, rotasiAwalKamera));

        mc.bisaGerak = true;
        sedangFokus = false;
    }

    public void SelesaiServisBebaskanKamera()
    {
        StartCoroutine(ProsesSelesaiServis());
    }

    private IEnumerator ProsesSelesaiServis()
    {
        yield return new WaitForSeconds(jedaPindahKamera);
        KembaliKePemain();
    }

    public void StartServis()
    {
        if (sedangFokus == false)
        {
            FokusKeMotor();
        }
        else
        {
            KembaliKePemain();
        }
    }
}
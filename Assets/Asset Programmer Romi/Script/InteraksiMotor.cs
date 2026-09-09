using UnityEngine;
using System.Collections;

public class InteraksiMotor : MonoBehaviour
{
    public static InteraksiMotor instance;
    
    public float jedaPindahKamera = 1.5f;
    public float waktuPerpindahanKamera = 1f;
    
    [Header("Pengaturan FreeCam")]
    public float speedFreeCam = 5f;
    public float sensKamera = 200f;

    private GameObject kamera;
    private GameObject kamerapos;
    private MekanikController mc;
    
    private Transform batas1;
    private Transform batas2;
    
    public bool sedangFokus = false;
    private bool modeFreeCamAktif = false;
    
    private Vector3 posisiAwalKamera;
    private Quaternion rotasiAwalKamera;
    private Coroutine transisiKameraRoutine;
    
    private float xRot = 0f;
    private float yRot = 0f;

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

        GameObject objBatas1 = GameObject.FindWithTag("Batas1");
        GameObject objBatas2 = GameObject.FindWithTag("Batas2");

        if (objBatas1 != null) batas1 = objBatas1.transform;
        if (objBatas2 != null) batas2 = objBatas2.transform;
    }

    void Update()
    {
        if (modeFreeCamAktif)
        {
            GerakFreeCam();
        }
    }

    void GerakFreeCam()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensKamera * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensKamera * Time.deltaTime;

            xRot -= mouseY;
            yRot += mouseX;
            kamera.transform.rotation = Quaternion.Euler(xRot, yRot, 0f);
        }

        float gerakX = Input.GetAxis("Horizontal");
        float gerakZ = Input.GetAxis("Vertical");
        float gerakY = 0f;

        if (Input.GetKey(KeyCode.E)) gerakY = 1f;
        if (Input.GetKey(KeyCode.Q)) gerakY = -1f;

        Vector3 arahGerak = (kamera.transform.right * gerakX) + (kamera.transform.up * gerakY) + (kamera.transform.forward * gerakZ);
        kamera.transform.position += arahGerak * speedFreeCam * Time.deltaTime;

        if (batas1 != null && batas2 != null)
        {
            float minX = Mathf.Min(batas1.position.x, batas2.position.x);
            float maxX = Mathf.Max(batas1.position.x, batas2.position.x);
            float minY = Mathf.Min(batas1.position.y, batas2.position.y);
            float maxY = Mathf.Max(batas1.position.y, batas2.position.y);
            float minZ = Mathf.Min(batas1.position.z, batas2.position.z);
            float maxZ = Mathf.Max(batas1.position.z, batas2.position.z);

            Vector3 pos = kamera.transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            kamera.transform.position = pos;
        }
    }

    public void FokusKeMotor()
    {
        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah == null || langkah.posisiKamera == null) return;

        posisiAwalKamera = kamerapos.transform.position;
        rotasiAwalKamera = kamerapos.transform.rotation;

        mc.bisaGerak = false;
        mc.bisaRotasiKamera = false;
        modeFreeCamAktif = false;

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
        modeFreeCamAktif = false;
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
        modeFreeCamAktif = false;
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
        
        Vector3 rotasiSekarang = kamera.transform.eulerAngles;
        xRot = rotasiSekarang.x;
        yRot = rotasiSekarang.y;
        
        modeFreeCamAktif = true;
    }

    public void KembaliKePemain()
    {
        if (sedangFokus == false) return;
        
        modeFreeCamAktif = false;

        if (transisiKameraRoutine != null) StopCoroutine(transisiKameraRoutine);
        transisiKameraRoutine = StartCoroutine(PindahKameraSmoothKeAwal(posisiAwalKamera, rotasiAwalKamera));

        mc.bisaGerak = true;
        mc.bisaRotasiKamera = true;
        sedangFokus = false;
    }

    private IEnumerator PindahKameraSmoothKeAwal(Vector3 targetPos, Quaternion targetRot)
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

    public void SelesaiServisBebaskanKamera()
    {
        StartCoroutine(ProsesSelesaiServis());
    }

    private IEnumerator ProsesSelesaiServis()
    {
        modeFreeCamAktif = false;
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
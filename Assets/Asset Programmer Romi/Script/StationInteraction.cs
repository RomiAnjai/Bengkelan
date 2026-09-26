using UnityEngine;
using System.Collections;

public class StationInteraction : MonoBehaviour
{
    public static StationInteraction instance;
    public Transform titikKameraMeja;
    public GameObject grupTitikMinigame;
    public float waktuPerpindahan = 1f;

    [Header("Fase Pasang Ban")]
    public GameObject prefabBanMenyatu;
    public bool fasePasangBan = false;
    public bool adaBan = false;
    public bool sedangFokusMeja = false;
    public bool minigameSelesai = false;

    public GameObject velgDiMeja;
    public GameObject banBaruDiMeja;

    private int sisaTitik = 5;
    private int indexTitikSaatIni = 0;
    private GameObject banAktif;
    private GameObject kamera;
    private MekanikController mc;
    private Vector3 posisiAwalKamera;
    private Quaternion rotasiAwalKamera;
    private Coroutine transisiRoutine;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        kamera = GameObject.FindWithTag("MainCamera");
        mc = FindAnyObjectByType<MekanikController>();
        if (grupTitikMinigame != null) grupTitikMinigame.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ban Luar") && !adaBan && !minigameSelesai && !fasePasangBan)
        {
            if (other.attachedRigidbody == null) return;

            KondisiBan statusBan = other.GetComponent<KondisiBan>();
            if (statusBan == null) statusBan = other.GetComponentInParent<KondisiBan>();

            if (statusBan != null && statusBan.isBanBekas)
            {
                return;
            }

            adaBan = true;
            banAktif = other.attachedRigidbody.gameObject;
            KunciKeMeja(banAktif);
            MatikanHighlightSaatIni();
            return;
        }

        if (minigameSelesai && !fasePasangBan)
        {
            return;
        }

        if (other.CompareTag("Velg Depan") && velgDiMeja == null)
        {
            velgDiMeja = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            KunciKeMeja(velgDiMeja);
            MatikanHighlightSaatIni();
        }

        if (other.CompareTag("BanLuarBaru") && banBaruDiMeja == null)
        {
            banBaruDiMeja = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            KunciKeMeja(banBaruDiMeja);
            MatikanHighlightSaatIni();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ban Luar") && banAktif != null && !fasePasangBan)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.gameObject == banAktif)
            {
                adaBan = false;
                banAktif = null;
                Invoke("ResetMeja", 1f);
            }
        }

        if (other.CompareTag("Velg Depan") && velgDiMeja != null)
        {
            GameObject objKeluar = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            if (objKeluar == velgDiMeja) velgDiMeja = null;
        }

        if (other.CompareTag("BanLuarBaru") && banBaruDiMeja != null)
        {
            GameObject objKeluar = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
            if (objKeluar == banBaruDiMeja) banBaruDiMeja = null;
        }
    }

    void KunciKeMeja(GameObject obj)
    {
        obj.transform.position = transform.position + new Vector3(0, 0.7f, 0);
        if(obj.CompareTag("Velg Depan"))
        {
            obj.transform.position = transform.position + new Vector3(0, 0.85f, 0);
        }
        obj.transform.rotation = Quaternion.Euler(90, 0, 0);

        Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            r.isKinematic = true;
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
        }
    }

    void MatikanHighlightSaatIni()
    {
        if (SequenceService.instance != null)
        {
            DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
            if (langkah != null)
            {
                TutorialHighlight.MatikanBerdasarkanID(langkah.idObjekTutorial);
            }
        }
    }

    void ResetMeja()
    {
        minigameSelesai = false;
    }

    public bool BisaInteraksi()
    {
        if (sedangFokusMeja) return false;
        if (adaBan && !minigameSelesai && !fasePasangBan) return true;
        if (velgDiMeja != null && banBaruDiMeja != null && !fasePasangBan) return true;
        return false;
    }

    public void InteraksiMeja()
    {
        if (!BisaInteraksi()) return;

        if (velgDiMeja != null && banBaruDiMeja != null)
        {
            fasePasangBan = true;
            minigameSelesai = false;
        }

        FokusKeMeja();
    }

    void FokusKeMeja()
    {
        posisiAwalKamera = kamera.transform.position;
        rotasiAwalKamera = kamera.transform.rotation;
        mc.bisaGerak = false;
        mc.bisaRotasiKamera = false;

        if (transisiRoutine != null) StopCoroutine(transisiRoutine);
        transisiRoutine = StartCoroutine(PindahKameraSmooth(titikKameraMeja.position, titikKameraMeja.rotation));

        sedangFokusMeja = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (grupTitikMinigame != null) 
        {
            grupTitikMinigame.SetActive(true);
            sisaTitik = grupTitikMinigame.transform.childCount;
            indexTitikSaatIni = 0;

            for (int i = 0; i < sisaTitik; i++)
            {
                grupTitikMinigame.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (sisaTitik > 0)
            {
                grupTitikMinigame.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void TitikDitekan(GameObject titik)
    {
        titik.SetActive(false);
        sisaTitik--;

        if (sisaTitik > 0)
        {
            indexTitikSaatIni++;
            if (indexTitikSaatIni < grupTitikMinigame.transform.childCount)
            {
                grupTitikMinigame.transform.GetChild(indexTitikSaatIni).gameObject.SetActive(true);
            }
        }
        else
        {
            SelesaiMinigame();
        }
    }

    void SelesaiMinigame()
    {
        SequenceService.instance.SelesaikanLangkah();
        if (grupTitikMinigame != null) grupTitikMinigame.SetActive(false);
        
        minigameSelesai = true;

        if (!fasePasangBan)
        {
            SequenceService.instance.PreteliVelg();

            if (SequenceService.instance.rbVelg != null)
            {
                SequenceService.instance.rbVelg.isKinematic = false;
            }

            if (banAktif != null)
            {
                KondisiBan statusBan = banAktif.GetComponent<KondisiBan>();
                if (statusBan == null) statusBan = banAktif.GetComponentInParent<KondisiBan>();
                
                if (statusBan != null)
                {
                    statusBan.isBanBekas = true;
                }

                Rigidbody[] rbs = banAktif.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody r in rbs)
                {
                    r.isKinematic = false;
                }
            }
            velgDiMeja = null;
        }
        else
        {
            if (velgDiMeja != null) Destroy(velgDiMeja);
            if (banBaruDiMeja != null) Destroy(banBaruDiMeja);

            if (prefabBanMenyatu != null)
            {
                Instantiate(prefabBanMenyatu, transform.position + new Vector3(0, 0.7f, 0), Quaternion.Euler(90, 0, 0));
                SequenceService.instance.NyalakanColliderMotor();
            }
            
            fasePasangBan = false;
        }

        if (transisiRoutine != null) StopCoroutine(transisiRoutine);
        transisiRoutine = StartCoroutine(PindahKameraSmooth(posisiAwalKamera, rotasiAwalKamera, true));
    }

    private IEnumerator PindahKameraSmooth(Vector3 targetPos, Quaternion targetRot, bool kembaliKePemain = false)
    {
        float elapsed = 0f;
        Vector3 startPos = kamera.transform.position;
        Quaternion startRot = kamera.transform.rotation;

        while (elapsed < waktuPerpindahan)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / waktuPerpindahan;
            kamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            kamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        kamera.transform.position = targetPos;
        kamera.transform.rotation = targetRot;

        if (kembaliKePemain)
        {
            mc.bisaGerak = true;
            mc.bisaRotasiKamera = true;
            sedangFokusMeja = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
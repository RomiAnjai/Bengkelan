using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StationInteraction : MonoBehaviour
{
    public static StationInteraction instance;

    public Transform titikKameraMeja;
    public GameObject grupTitikMinigame;
    public float waktuPerpindahan = 1f;
    public string idHighlightMeja = "Meja Kerja";

    [Header("Pengaturan Hold & UI Lingkaran")]
    public Image uiLingkaranProgress;
    public float durasiTahanCongkel = 3f;

    [Header("Pengaturan Animasi 3D Linggis")]
    public GameObject objekLinggis;
    public float durasiAnimasiLinggis = 0.6f;
    public Vector3 offsetPosisiLinggis = new Vector3(0f, 0.05f, 0f);
    public Vector3 rotasiAwalLinggis = new Vector3(15f, 0f, 0f);
    public Vector3 rotasiCongkelLinggis = new Vector3(-55f, 0f, 0f);

    [Header("Fase Pasang Ban")]
    public GameObject prefabBanMenyatu;
    public bool fasePasangBan = false;

    public bool adaBan = false;
    public bool sedangFokusMeja = false;
    public bool minigameSelesai = false;

    [Header("Audio Meja Kerja")]
    public AudioSource audioMeja;
    public AudioClip sfxTaruhMeja;
    public AudioClip sfxCongkel;

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

    private float timerCongkel = 0f;
    private bool sedangAnimasiLinggis = false;

    private Camera kameraUtama;
    private GameObject titikDitahanSaatIni = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (grupTitikMinigame != null)
        {
            for (int i = 0; i < grupTitikMinigame.transform.childCount; i++)
            {
                grupTitikMinigame.transform.GetChild(i).gameObject.SetActive(false);
            }
            grupTitikMinigame.SetActive(false);
        }
    }

    void Start()
    {
        kamera = GameObject.FindWithTag("MainCamera");
        kameraUtama = kamera.GetComponent<Camera>();
        mc = FindAnyObjectByType<MekanikController>();

        if (uiLingkaranProgress != null)
        {
            uiLingkaranProgress.raycastTarget = false;
            uiLingkaranProgress.fillAmount = 0f;
            uiLingkaranProgress.gameObject.SetActive(false);
        }

        if (objekLinggis != null)
        {
            Collider[] colLinggis = objekLinggis.GetComponentsInChildren<Collider>(true);
            foreach (Collider c in colLinggis)
            {
                c.enabled = false;
            }
            objekLinggis.SetActive(false);
        }
    }

    void Update()
    {
        if (uiLingkaranProgress != null && uiLingkaranProgress.gameObject.activeSelf)
        {
            uiLingkaranProgress.rectTransform.position = Input.mousePosition;
        }

        if (sedangFokusMeja && !sedangAnimasiLinggis)
        {
            if (Input.GetMouseButton(0))
            {
                MendeteksiTitikCongkelViaRaycast();
            }
            else if (timerCongkel > 0f)
            {
                ResetProgressCongkel();
            }
        }
    }

    private void MendeteksiTitikCongkelViaRaycast()
    {
        if (kameraUtama == null) return;

        Ray ray = kameraUtama.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            TitikCongkelBan titik = hit.collider.GetComponent<TitikCongkelBan>();
            if (titik == null) titik = hit.collider.GetComponentInParent<TitikCongkelBan>();

            if (titik != null)
            {
                titikDitahanSaatIni = titik.gameObject;
                ProsesTahanTitik(titikDitahanSaatIni);
            }
            else
            {
                ResetProgressCongkel();
            }
        }
        else
        {
            ResetProgressCongkel();
        }
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
            TutorialHighlight.MatikanBerdasarkanID(idHighlightMeja);
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
        if (audioMeja != null && sfxTaruhMeja != null) audioMeja.PlayOneShot(sfxTaruhMeja);

        obj.transform.position = transform.position + new Vector3(0, 0.7f, 0);
        if (obj.CompareTag("Velg Depan"))
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
        sedangAnimasiLinggis = false;
        ResetProgressCongkel();

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

    public void ProsesTahanTitik(GameObject titik)
    {
        if (!sedangFokusMeja || sedangAnimasiLinggis) return;

        if (uiLingkaranProgress != null)
        {
            if (!uiLingkaranProgress.gameObject.activeSelf)
            {
                uiLingkaranProgress.gameObject.SetActive(true);
            }
            uiLingkaranProgress.rectTransform.position = Input.mousePosition;
        }

        timerCongkel += Time.deltaTime;

        if (uiLingkaranProgress != null)
        {
            uiLingkaranProgress.fillAmount = Mathf.Clamp01(timerCongkel / durasiTahanCongkel);
        }

        if (timerCongkel >= durasiTahanCongkel)
        {
            StartCoroutine(ProsesAnimasiCongkel(titik));
        }
    }

    public void ResetProgressCongkel()
    {
        timerCongkel = 0f;
        titikDitahanSaatIni = null;
        if (uiLingkaranProgress != null)
        {
            uiLingkaranProgress.fillAmount = 0f;
            if (uiLingkaranProgress.gameObject.activeSelf)
            {
                uiLingkaranProgress.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ProsesAnimasiCongkel(GameObject titik)
    {
        sedangAnimasiLinggis = true;
        ResetProgressCongkel();

        if (audioMeja != null && sfxCongkel != null)
        {
            audioMeja.PlayOneShot(sfxCongkel);
        }

        if (objekLinggis != null && titik != null)
        {
            objekLinggis.SetActive(true);
            objekLinggis.transform.position = titik.transform.position + titik.transform.TransformDirection(offsetPosisiLinggis);

            Quaternion rotAwal = titik.transform.rotation * Quaternion.Euler(rotasiAwalLinggis);
            Quaternion rotAkhir = titik.transform.rotation * Quaternion.Euler(rotasiCongkelLinggis);

            objekLinggis.transform.rotation = rotAwal;

            float elapsed = 0f;
            while (elapsed < durasiAnimasiLinggis)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / durasiAnimasiLinggis);
                objekLinggis.transform.rotation = Quaternion.Slerp(rotAwal, rotAkhir, t);
                yield return null;
            }

            objekLinggis.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(durasiAnimasiLinggis);
        }

        TitikDitekan(titik);
        sedangAnimasiLinggis = false;
    }

    public void TitikDitekan(GameObject titik)
    {
        if (titik != null) titik.SetActive(false);
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
        ResetProgressCongkel();
        if (objekLinggis != null) objekLinggis.SetActive(false);
        if (grupTitikMinigame != null) grupTitikMinigame.SetActive(false);
        minigameSelesai = true;

        if (!fasePasangBan)
        {
            SequenceService.instance.PreteliVelg();
            SequenceService.instance.SelesaikanLangkah();

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

            ObjectiveHUD.instance.UpdateHUD(SequenceService.instance.motorYangSedangDiservis, SequenceService.instance.masalahAktif.namaKerusakan, 6);

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
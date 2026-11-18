using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class DayManagerScript : MonoBehaviour
{
    public static DayManagerScript instance;

    public Material skyBoxMaterialPagi;
    public Material skyBoxMaterialMalam;
    public Light lampuMatahari;
    public Light lampuBohlam;
    public bool isPagi = true;
    public float durasiTransisi = 2.5f;

    [Header("Pengaturan Suhu & Eksposur")]
    public float suhuMatahariPagi = 6500f;
    public float suhuMatahariSore = 3000f;
    public float eksposurPagi = 1f;
    public float eksposurMalam = 0.5f;

    [Header("Animasi Rotasi Langit & Waktu")]
    public float kecepatanPutarLangit = 0.5f;
    public float waktuSatuHari = 600f; 

    private bool sedangTransisi = false;
    private float rotasiLangitSaatIni = 0f;
    private float timerHariSaatIni = 0f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (lampuMatahari != null)
        {
            lampuMatahari.useColorTemperature = true;
            lampuMatahari.colorTemperature = isPagi ? suhuMatahariPagi : suhuMatahariSore;
        }

        RenderSettings.skybox = isPagi ? skyBoxMaterialPagi : skyBoxMaterialMalam;
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            RenderSettings.skybox.SetFloat("_Exposure", isPagi ? eksposurPagi : eksposurMalam);
        }
    }

    void Update()
    {
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Rotation"))
        {
            rotasiLangitSaatIni += Time.deltaTime * kecepatanPutarLangit;
            RenderSettings.skybox.SetFloat("_Rotation", rotasiLangitSaatIni);
        }

        if (!sedangTransisi && isPagi)
        {
            timerHariSaatIni += Time.deltaTime;
            
            if (timerHariSaatIni >= waktuSatuHari)
            {
                if (DailyReportManager.instance != null)
                {
                    DailyReportManager.instance.TampilkanLaporanHarian();
                }

                timerHariSaatIni = 0f;
                GantiKeMalam();
            }
        }
    }

    public void GantiKeMalam()
    {
        if (isPagi && !sedangTransisi)
        {
            StartCoroutine(ProsesUbahHari());
        }
    }

    public void GantiKePagi()
    {
        if (!isPagi && !sedangTransisi)
        {
            timerHariSaatIni = 0f;
            StartCoroutine(ProsesUbahHari());
        }
    }

    private IEnumerator ProsesUbahHari()
    {
        sedangTransisi = true;
        
        float targetIntensitasLampu = isPagi ? 0f : 1.5f;
        float targetAmbient = isPagi ? 0.4f : 0.8f;
        float targetPadangLampu = isPagi ? 2f : 0f;
        float targetSuhuMatahari = isPagi ? suhuMatahariSore : suhuMatahariPagi;
        float targetEksposurAkhir = isPagi ? eksposurMalam : eksposurPagi;
        
        float intensitasLampuAwal = lampuMatahari.intensity;
        float padangLampuAwal = lampuBohlam.intensity;
        float ambientAwal = RenderSettings.ambientIntensity;
        float suhuMatahariAwal = lampuMatahari.colorTemperature;
        float eksposurAwal = 1f;
        
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            eksposurAwal = RenderSettings.skybox.GetFloat("_Exposure");
        }
        
        Material skyboxLama = RenderSettings.skybox;
        Material skyboxBaru = isPagi ? skyBoxMaterialMalam : skyBoxMaterialPagi;
        
        bool materialSudahDitukar = false;
        float waktuBerjalan = 0f;
        
        while (waktuBerjalan < durasiTransisi)
        {
            waktuBerjalan += Time.deltaTime;
            float persentase = waktuBerjalan / durasiTransisi;
            float smoothT = Mathf.SmoothStep(0f, 1f, persentase);
            
            lampuMatahari.intensity = Mathf.Lerp(intensitasLampuAwal, targetIntensitasLampu, smoothT);
            lampuMatahari.colorTemperature = Mathf.Lerp(suhuMatahariAwal, targetSuhuMatahari, smoothT);
            RenderSettings.ambientIntensity = Mathf.Lerp(ambientAwal, targetAmbient, smoothT);
            lampuBohlam.intensity = Mathf.Lerp(padangLampuAwal, targetPadangLampu, smoothT);
            
            if (persentase < 0.5f)
            {
                if (skyboxLama.HasProperty("_Exposure"))
                {
                    float fadeOut = Mathf.Lerp(eksposurAwal, 0f, persentase * 2f);
                    skyboxLama.SetFloat("_Exposure", fadeOut);
                }
            }
            else
            {
                if (!materialSudahDitukar)
                {
                    RenderSettings.skybox = skyboxBaru;
                    materialSudahDitukar = true;
                }
                
                if (skyboxBaru.HasProperty("_Exposure"))
                {
                    float fadeIn = Mathf.Lerp(0f, targetEksposurAkhir, (persentase - 0.5f) * 2f);
                    skyboxBaru.SetFloat("_Exposure", fadeIn);
                }
            }
            
            yield return null;
        }

        lampuMatahari.intensity = targetIntensitasLampu;
        lampuMatahari.colorTemperature = targetSuhuMatahari;
        RenderSettings.ambientIntensity = targetAmbient;
        lampuBohlam.intensity = targetPadangLampu;
        
        if (skyboxBaru.HasProperty("_Exposure"))
        {
            skyboxBaru.SetFloat("_Exposure", targetEksposurAkhir);
        }

        if (skyboxLama.HasProperty("_Exposure"))
        {
            float eksposurReset = isPagi ? eksposurPagi : eksposurMalam;
            skyboxLama.SetFloat("_Exposure", eksposurReset);
        }
        
        isPagi = !isPagi;
        sedangTransisi = false;
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class DayManagerScript : MonoBehaviour
{
    public Material skyBoxMaterialPagi;
    public Material skyBoxMaterialMalam;
    public Light lampuMatahari;
    public Light lampuBohlam;
    public bool isPagi = true;
    public float durasiTransisi = 2.5f;

    private bool sedangTransisi = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !sedangTransisi)
        {
            StartCoroutine(ProsesUbahHari());
        }
    }

    private IEnumerator ProsesUbahHari()
    {
        sedangTransisi = true;
        
        float targetIntensitasLampu = isPagi ? 0f : 1.5f;
        float targetAmbient = isPagi ? 0.4f : 0.8f;
        float targetPadangLampu = isPagi ? 2f : 0f;
        
        float intensitasLampuAwal = lampuMatahari.intensity;
        float padangLampuAawal = lampuBohlam.intensity;
        float ambientAwal = RenderSettings.ambientIntensity;
        
        float waktuBerjalan = 0f;
        
        while (waktuBerjalan < durasiTransisi)
        {
            waktuBerjalan += Time.deltaTime;
            float persentase = waktuBerjalan / durasiTransisi;
            float smoothT = Mathf.SmoothStep(0f, 1f, persentase);
            
            lampuMatahari.intensity = Mathf.Lerp(intensitasLampuAwal, targetIntensitasLampu, smoothT);
            RenderSettings.ambientIntensity = Mathf.Lerp(ambientAwal, targetAmbient, smoothT);
            lampuBohlam.intensity = Mathf.Lerp(padangLampuAawal, targetPadangLampu, smoothT);
            
            if (isPagi && persentase > 0.8f && RenderSettings.skybox != skyBoxMaterialMalam)
            {
                RenderSettings.skybox = skyBoxMaterialMalam;
            }
            
            yield return null;
        }
        
        if (!isPagi)
        {
            RenderSettings.skybox = skyBoxMaterialPagi;
        }

        lampuMatahari.intensity = targetIntensitasLampu;
        RenderSettings.ambientIntensity = targetAmbient;
        isPagi = !isPagi;
        sedangTransisi = false;
    }
}
using UnityEngine;
using System.Collections.Generic;

public class TutorialHighlight : MonoBehaviour
{
    public static List<TutorialHighlight> daftarHighlight = new List<TutorialHighlight>();

    public string idTutorial;
    public Renderer targetRenderer;
    public Color warnaHighlight = Color.yellow;
    public float intensitasGlow = 2.5f;
    public float kecepatanKedip = 4f;

    private Color warnaAsli;
    private Color warnaEmisiAsli;
    private bool punyaEmisiAwal = false;
    private bool sedangHighlight = false;
    private Material materialAktif;

    void Awake()
    {
        daftarHighlight.Add(this);
    }

    void OnDestroy()
    {
        daftarHighlight.Remove(this);
    }

    void Start()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            materialAktif = targetRenderer.material;

            if (materialAktif.HasProperty("_Color"))
            {
                warnaAsli = materialAktif.color;
            }

            if (materialAktif.HasProperty("_EmissionColor"))
            {
                punyaEmisiAwal = materialAktif.IsKeywordEnabled("_EMISSION");
                warnaEmisiAsli = materialAktif.GetColor("_EmissionColor");
            }
        }
    }

    void Update()
    {
        if (sedangHighlight && materialAktif != null)
        {
            float kilap = Mathf.PingPong(Time.time * kecepatanKedip, 1f);

            if (materialAktif.HasProperty("_Color"))
            {
                materialAktif.color = Color.Lerp(warnaAsli, warnaHighlight, kilap);
            }

            if (materialAktif.HasProperty("_EmissionColor"))
            {
                Color warnaGlow = warnaHighlight * Mathf.LinearToGammaSpace(kilap * intensitasGlow);
                materialAktif.SetColor("_EmissionColor", warnaGlow);
            }
        }
    }

    public void MulaiHighlight()
    {
        sedangHighlight = true;
        if (materialAktif != null && materialAktif.HasProperty("_EmissionColor"))
        {
            materialAktif.EnableKeyword("_EMISSION");
        }
    }

    public void HentikanHighlight()
    {
        sedangHighlight = false;

        if (materialAktif != null)
        {
            if (materialAktif.HasProperty("_Color"))
            {
                materialAktif.color = warnaAsli;
            }

            if (materialAktif.HasProperty("_EmissionColor"))
            {
                materialAktif.SetColor("_EmissionColor", warnaEmisiAsli);

                if (!punyaEmisiAwal)
                {
                    materialAktif.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    public static void NyalakanBerdasarkanID(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        foreach (TutorialHighlight t in daftarHighlight)
        {
            if (t != null && t.idTutorial == id)
            {
                t.MulaiHighlight();
            }
        }
    }

    public static void MatikanBerdasarkanID(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        foreach (TutorialHighlight t in daftarHighlight)
        {
            if (t != null && t.idTutorial == id)
            {
                t.HentikanHighlight();
            }
        }
    }
}
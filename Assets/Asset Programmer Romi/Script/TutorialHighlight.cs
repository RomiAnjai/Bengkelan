using UnityEngine;
using System.Collections.Generic;

public class TutorialHighlight : MonoBehaviour
{
    public static List<TutorialHighlight> daftarHighlight = new List<TutorialHighlight>();

    public string idTutorial;
    public Renderer targetRenderer;
    public Color warnaHighlight = Color.yellow;
    public float kecepatanKedip = 4f;

    private Color warnaAsli;
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
        }
    }

    void Update()
    {
        if (sedangHighlight && materialAktif != null && materialAktif.HasProperty("_Color"))
        {
            float kilap = Mathf.PingPong(Time.time * kecepatanKedip, 1f);
            materialAktif.color = Color.Lerp(warnaAsli, warnaHighlight, kilap);
        }
    }

    public void MulaiHighlight()
    {
        sedangHighlight = true;
    }

    public void HentikanHighlight()
    {
        sedangHighlight = false;
        if (materialAktif != null && materialAktif.HasProperty("_Color"))
        {
            materialAktif.color = warnaAsli;
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
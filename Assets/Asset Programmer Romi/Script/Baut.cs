using UnityEngine;

public class Baut : MonoBehaviour
{
    public string namaObjek = "Baut";
    public DataSparepart partInduk;
    public SphereCollider colliderBan;
    public Rigidbody rbBanDepan;

    public TipeAksi tipeAksiDibutuhkan = TipeAksi.LepasBaut;

    [Header("Kustomisasi Gerakan Baut")]
    public float kecepatanPutar = 500f;
    public float jarakKeluar = 0.2f;
    public Vector3 arahKeluar = new Vector3(0, 0, 1);
    public Vector3 arahputar;

    [Header("Rotasi Awal & Dikunci")]
    public float rotasiKunciY = 90f;
    public float rotasiKunciZ = 90f;
    public char arahPutar;

    public float progres = 0f;
    private Vector3 posisiAwal;

    void Start()
    {
        posisiAwal = transform.localPosition;

        if (partInduk != null)
        {
            partInduk.bisaDiambil = false;
            if (colliderBan != null)
            {
                colliderBan.enabled = false;   
            }
        }
    }

    public void ProsesLepas()
    {
        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah == null || langkah.jenisAksi != tipeAksiDibutuhkan)
        {
            return;
        }

        progres += Time.deltaTime;

        Vector3 deltaputar = arahputar * kecepatanPutar * Time.deltaTime;
        transform.localEulerAngles = transform.localEulerAngles + deltaputar;

        Vector3 deltaPosisi = arahKeluar * (progres * jarakKeluar);
        transform.localPosition = posisiAwal + deltaPosisi;

        if (progres >= 1f)
        {
            LepasTotal();
        }
    }

    void LepasTotal()
    {
        Baut[] semuaBaut = partInduk.GetComponentsInChildren<Baut>();
        
        int sisaBautTipeIni = 0;
        foreach (Baut b in semuaBaut)
        {
            if (b.tipeAksiDibutuhkan == this.tipeAksiDibutuhkan)
            {
                sisaBautTipeIni++;
            }
        }

        if (sisaBautTipeIni <= 1)
        {
            DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
            if (langkah != null && langkah.jenisAksi == tipeAksiDibutuhkan)
            {
                SequenceService.instance.SelesaikanLangkah();
            }
        }

        if (semuaBaut.Length <= 1)
        {
            partInduk.bisaDiambil = true;
            if (colliderBan != null)
            {
                colliderBan.enabled = true;
            }
            
            if (rbBanDepan != null)
            {
                rbBanDepan.isKinematic = false;
                colliderBan.enabled = true;
                rbBanDepan.useGravity = true;
            }
        }
        
        Destroy(gameObject);
    }
}
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
    public bool modePasang = false;

    [Header("Rotasi Awal & Dikunci")]
    public float rotasiKunciY = 90f;
    public float rotasiKunciZ = 90f;
    public char arahPutar;
    public float progres = 0f;

    private Vector3 posisiDalam;
    private Vector3 posisiLuar;
    private Vector3 rotasiSaatIni;
    private bool selesai = false;

    void Start()
    {
        posisiDalam = transform.localPosition;
        posisiLuar = posisiDalam + (arahKeluar * jarakKeluar);
        rotasiSaatIni = transform.localEulerAngles;
        
        if (modePasang)
        {
            transform.localPosition = posisiLuar;
        }

        if (partInduk != null && !modePasang)
        {
            partInduk.bisaDiambil = false;
            if (colliderBan != null)
            {
                colliderBan.enabled = false;
            }
        }
    }

    public void ProsesInteraksi()
    {
        if (selesai) return;

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        if (langkah == null || langkah.jenisAksi != tipeAksiDibutuhkan) return;

        progres += Time.deltaTime;
        
        if (!modePasang)
        {
            Vector3 deltaputar = arahputar * kecepatanPutar * Time.deltaTime;
            rotasiSaatIni = rotasiSaatIni + deltaputar;
            transform.localEulerAngles = rotasiSaatIni;
            transform.localPosition = Vector3.Lerp(posisiDalam, posisiLuar, progres);
        }
        else
        {
            Vector3 deltaputar = -arahputar * kecepatanPutar * Time.deltaTime;
            rotasiSaatIni = rotasiSaatIni + deltaputar;
            transform.localEulerAngles = rotasiSaatIni;
            transform.localPosition = Vector3.Lerp(posisiLuar, posisiDalam, progres);
        }

        if (progres >= 1f)
        {
            SelesaiInteraksi();
        }
    }

    void SelesaiInteraksi()
    {
        selesai = true;
        transform.localPosition = modePasang ? posisiDalam : posisiLuar;

        DetailLangkah langkah = SequenceService.instance.GetLangkahSaatIni();
        bool bolehLanjut = false;

        if (modePasang)
        {
            bolehLanjut = true; 
        }
        else
        {
            if (partInduk != null)
            {
                Baut[] semuaBaut = partInduk.GetComponentsInChildren<Baut>();
                int sisaBautTipeIni = 0;
                foreach (Baut b in semuaBaut)
                {
                    if (b.tipeAksiDibutuhkan == this.tipeAksiDibutuhkan && !b.selesai) sisaBautTipeIni++;
                }
                if (sisaBautTipeIni <= 1) bolehLanjut = true;
            }
            else
            {
                bolehLanjut = true;
            }
        }

        if (bolehLanjut && langkah != null && langkah.jenisAksi == tipeAksiDibutuhkan)
        {
            SequenceService.instance.SelesaikanLangkah();
        }

        if (!modePasang)
        {
            if (tipeAksiDibutuhkan == TipeAksi.LepasAsDepan)
            {
                if (partInduk != null) partInduk.bisaDiambil = true;
                if (colliderBan != null) colliderBan.enabled = true;
                if (rbBanDepan != null)
                {
                    rbBanDepan.isKinematic = false;
                    rbBanDepan.useGravity = true;
                }
            }
            Destroy(gameObject);
        }
    }
}
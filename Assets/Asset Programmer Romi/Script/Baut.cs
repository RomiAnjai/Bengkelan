using UnityEngine;

public class Baut : MonoBehaviour
{
    public string namaObjek = "Baut";
    public DataSparepart partInduk;
    public SphereCollider colliderBan;
    public Rigidbody rbBanDepan;

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
    private float rotasiXSaatIni;
    private float rotasiYSaatIni;
    private float rotasiZSaatIni;

    void Start()
    {
        posisiAwal = transform.localPosition;

        if (arahPutar == 'x')
        {
            rotasiXSaatIni = transform.localEulerAngles.x;
        } else if (arahPutar == 'y')
        {
            rotasiXSaatIni = transform.localEulerAngles.y;
        } else if (arahPutar == 'z')
        {
            rotasiXSaatIni = transform.localEulerAngles.z;
        }

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
        Baut[] sisaBaut = partInduk.GetComponentsInChildren<Baut>();
        if (sisaBaut.Length <= 1)
        {
            partInduk.bisaDiambil = true;
            if (colliderBan != null)
            {
                colliderBan.enabled = true;
            }
            
            rbBanDepan.isKinematic = false;
            rbBanDepan.useGravity = true;
        }
        Destroy(gameObject);
    }
}
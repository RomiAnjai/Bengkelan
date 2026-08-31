using UnityEngine;

public class Baut : MonoBehaviour
{
    public string namaObjek = "Baut";
    public DataSparepart partInduk;

    [Header("Kustomisasi Gerakan Baut")]
    public float kecepatanPutar = 500f;
    public float jarakKeluar = 0.2f;
    public Vector3 arahKeluar = new Vector3(0, 0, 1);

    [Header("Rotasi Awal & Dikunci")]
    public float rotasiKunciY = 90f;
    public float rotasiKunciZ = 90f;

    public float progres = 0f;
    private Vector3 posisiAwal;
    private float rotasiXSaatIni;

    void Start()
    {
        posisiAwal = transform.localPosition;

        rotasiXSaatIni = transform.localEulerAngles.x;

        if (partInduk != null)
        {
            partInduk.bisaDiambil = false;
        }
    }

    public void ProsesLepas()
    {
        progres += Time.deltaTime;

        rotasiXSaatIni -= kecepatanPutar * Time.deltaTime;

        transform.localEulerAngles = new Vector3(rotasiXSaatIni, rotasiKunciY, rotasiKunciZ);

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
        }
        Destroy(gameObject);
    }
}
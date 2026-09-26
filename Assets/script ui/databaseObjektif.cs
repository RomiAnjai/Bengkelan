using System.Collections.Generic;
using UnityEngine;

// Enum untuk jenis motor
public enum JenisMotor
{
    Matic,
    Bebek,
    Sport
}

[System.Serializable]
public class DataLangkahObjektif
{
    public string namaLangkah; // Hanya untuk penanda di Inspector agar mudah dibaca
    [TextArea(2, 4)]
    public string teksObjektifDiLayar; // Teks yang akan muncul di HUD
}

[System.Serializable]
public class DataMasalahObjektif
{
    public string namaMasalah; // Misal: "Ban Bocor" (Harus sama dengan di SequenceService)
    public List<DataLangkahObjektif> daftarLangkah;
}

[System.Serializable]
public class DataMotorObjektif
{
    public JenisMotor jenisMotor;
    public List<DataMasalahObjektif> daftarMasalah;
}

[CreateAssetMenu(fileName = "DataObjektifBaru", menuName = "Simulasi Bengkel/Database Objektif")]
public class DatabaseObjektif : ScriptableObject
{
    public List<DataMotorObjektif> databaseMotor;

    // Fungsi untuk mencari teks berdasarkan Motor, Masalah, dan Index Langkah
    public string AmbilTeksObjektif(JenisMotor motor, string masalah, int indexLangkah)
    {
        // Cari jenis motor
        DataMotorObjektif dataMotor = databaseMotor.Find(m => m.jenisMotor == motor);
        if (dataMotor == null) return "Data Motor tidak ditemukan!";

        // Cari masalahnya
        DataMasalahObjektif dataMasalah = dataMotor.daftarMasalah.Find(m => m.namaMasalah == masalah);
        if (dataMasalah == null) return "Data Masalah tidak ditemukan!";

        // Ambil teks berdasarkan urutan (index) langkah
        if (indexLangkah >= 0 && indexLangkah < dataMasalah.daftarLangkah.Count)
        {
            return dataMasalah.daftarLangkah[indexLangkah].teksObjektifDiLayar;
        }

        return "Objektif Selesai!";
    }
}
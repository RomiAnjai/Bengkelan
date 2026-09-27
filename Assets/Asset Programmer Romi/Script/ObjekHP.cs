using UnityEngine;

public class ObjekHP : MonoBehaviour, IInteraksi
{
    public string DapatkanNamaPetunjuk()
    {
        return "Ambil HP";
    }

    public void EksekusiAksi()
    {
        GameManagerScript.instance.AmbilHP();
        Destroy(gameObject);
    }
}
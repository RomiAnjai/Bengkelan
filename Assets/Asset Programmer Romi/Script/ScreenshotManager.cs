using UnityEngine;
using System;

public class ScreenshotManager : MonoBehaviour
{
    public KeyCode tombolScreenshot = KeyCode.F1;
    public int resolusiBerapaKaliLipat = 1;

    void Update()
    {
        if (Input.GetKeyDown(tombolScreenshot))
        {
            AmbilScreenshot();
        }
    }

    private void AmbilScreenshot()
    {
        string waktu = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string namaFile = "Screenshot_" + waktu + ".png";
        
        ScreenCapture.CaptureScreenshot(namaFile, resolusiBerapaKaliLipat);
        
        Debug.Log("Jepret! Tersimpan dengan nama: " + namaFile);
    }
}
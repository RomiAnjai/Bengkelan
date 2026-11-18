using UnityEngine;
using System.Collections;

public class FlickerScript : MonoBehaviour
{
    public Light flicker;
    public bool isOn;
    public float minWaktu = 0.05f;
    public float maxWaktu = 0.5f;
    public float intensitasNyala = 2.5f;

    void Start()
    {
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight()
    {
        while (true)
        {
            float waktuAcak = Random.Range(minWaktu, maxWaktu);
            yield return new WaitForSecondsRealtime(waktuAcak);

            if (isOn)
            {
                flicker.intensity = 0f;
                isOn = false;
            }
            else
            {
                flicker.intensity = Random.Range(1.0f, intensitasNyala);
                isOn = true;
            }
        }
    }
}
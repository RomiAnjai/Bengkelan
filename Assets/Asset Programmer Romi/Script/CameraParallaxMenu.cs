using UnityEngine;

public class CameraParallaxMenu : MonoBehaviour
{
    public float sudutPutar = 10f;
    public float kehalusan = 5f;

    private Quaternion rotasiAwal;

    void Start()
    {
        rotasiAwal = transform.localRotation;
    }

    void Update()
    {
        float mouseX = Mathf.Clamp((Input.mousePosition.x / Screen.width) - 0.5f, -0.5f, 0.5f);
        float mouseY = Mathf.Clamp((Input.mousePosition.y / Screen.height) - 0.5f, -0.5f, 0.5f);

        Quaternion targetRotasi = rotasiAwal * Quaternion.Euler(-mouseY * sudutPutar, mouseX * sudutPutar, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotasi, Time.deltaTime * kehalusan);
    }
}
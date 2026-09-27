using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MekanikController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 200f;
    public Transform playerCamera;
    public bool bisaGerak = true;
    public bool bisaRotasiKamera = true;

    float xRotation = 0f;
    CharacterController controller;
    Vector3 velocity;
    float gravity = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateStatusGerak();
        MulaiGerak();
    }

    void UpdateStatusGerak()
    {
        bool fokusMotor = InteraksiMotor.instance != null && InteraksiMotor.instance.sedangFokus;
        bool fokusMeja = StationInteraction.instance != null && StationInteraction.instance.sedangFokusMeja;
        bool hpBuka = GameManagerScript.instance != null && GameManagerScript.instance.hpSedangBuka;

        if (fokusMotor || fokusMeja || hpBuka)
        {
            bisaGerak = false;
            bisaRotasiKamera = false;
        }
        else
        {
            bisaGerak = true;
            bisaRotasiKamera = true;
        }
    }

    public void KunciKursor(bool dikunci)
    {

    }

    public void MulaiGerak()
    {
        if (bisaGerak)
        {
            if (bisaRotasiKamera)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * mouseX);
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
}